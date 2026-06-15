using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// 単一環境のビート進行コントローラ（旧 GameManager のテレポート式フェーズの置き換え）。
///
/// 流れ:
///   OP(自動) → 各ビート: 発光 → 使用(Interact) → 窓に文言＋ボイス
///            → ボイス終了まで進行無効 → 「次へ」トリガーで次へ → 次を発光
///            → 全ビート完了 → 窓に終了案内を表示（実験者誘導）
///
/// 依存:
///   - MessageWindow（既存）: 窓表示
///   - GlowHighlight（既存）: 各ビートの発光
///   - BeatInteract（同梱）: 各インタラクト対象に付け、Interact を UseCurrentBeat に転送
///   - NextButton（既存・要変更）: 参照を本スクリプトに、呼ぶイベントを "TryAdvance" に
///
/// 配列(glows / interactables / windowTexts / voices)は同じ長さ・同じ順番で登録すること。
/// ボイス未割り当て(null)の間は noVoiceGuardSeconds 後に進行可能になる（グレーボックス検証用）。
/// </summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class BeatSequencer : UdonSharpBehaviour
{
    [Header("UI / 参照")]
    [Tooltip("既存の MessageWindow")]
    public MessageWindow messageWindow;
    [Tooltip("ボイス再生用 AudioSource（1つを使い回す）")]
    public AudioSource voiceSource;

    [Header("OP（自動）")]
    [TextArea(2, 5)] public string opText;
    public AudioClip opVoice;

    [Header("ビート（順番に・配列は同じ長さ）")]
    [Tooltip("各ビートで光らせる対象")]
    public GlowHighlight[] glows;
    [Tooltip("各ビートのインタラクト対象（コライダ付き）。current のみ有効化される")]
    public GameObject[] interactables;
    [TextArea(2, 6)] public string[] windowTexts;
    public AudioClip[] voices;

    [Header("ED（全使用後・ナレーション）")]
    [TextArea(2, 4)]
    [Tooltip("5obj 使い切った直後に窓へ表示（一人称エピローグ）")]
    public string edText;
    [Tooltip("ED ボイス（未割り当てなら edVoice なしで end 案内へ）")]
    public AudioClip edVoice;

    [Header("終了案内（ED のあと・実験者誘導）")]
    [TextArea(2, 4)]
    [Tooltip("VR条件：体験完了後に窓へ表示")]
    public string endTextVR = "以上で体験終了です。\nヘッドセットを外してください。";
    [TextArea(2, 4)]
    [Tooltip("desktop条件：体験完了後に窓へ表示")]
    public string endTextDesktop = "以上で体験終了です。\n実験者の指示をお待ちください。";

    [Header("調整")]
    [Tooltip("ボイス終了後、進行可能になるまでの余白（秒）")]
    public float advanceGuardExtra = 0.3f;
    [Tooltip("ボイス未割り当て時の進行ガード（秒）")]
    public float noVoiceGuardSeconds = 1.5f;

    private int currentBeat = -1; // -1 = OP前/OP中
    private bool canAdvance = false;

    void Start()
    {
        // 全インタラクト対象を無効化（current のみ後で有効化）
        if (interactables != null)
        {
            for (int i = 0; i < interactables.Length; i++)
            {
                if (interactables[i] != null) interactables[i].SetActive(false);
            }
        }
        // MessageWindow.Start が後から走って非表示にするのを避けるため1フレーム遅延
        SendCustomEventDelayedFrames(nameof(PlayOp), 1);
    }

    // ---------- OP ----------
    public void PlayOp()
    {
        canAdvance = false;
        if (messageWindow != null)
        {
            messageWindow.SetMode(0);
            messageWindow.ShowMessage(opText);
        }
        float dur = PlayVoice(opVoice);
        SendCustomEventDelayedSeconds(nameof(BeginFirstBeat), dur + advanceGuardExtra);
    }

    public void BeginFirstBeat()
    {
        currentBeat = 0;
        EnterBeat();
    }

    // ---------- 各ビート ----------
    // current のインタラクト対象を有効化し発光（文言はまだ出さない）
    private void EnterBeat()
    {
        if (currentBeat < 0 || currentBeat >= interactables.Length) return;
        canAdvance = false;
        if (interactables[currentBeat] != null) interactables[currentBeat].SetActive(true);
        GlowHighlight glow = GetGlow(currentBeat);
        if (glow != null) glow.StartGlow();
    }

    // BeatInteract.Interact から呼ばれる（current のみ有効なので index は自動で正しい）
    public void UseCurrentBeat()
    {
        if (currentBeat < 0 || currentBeat >= interactables.Length) return;

        GlowHighlight glow = GetGlow(currentBeat);
        if (glow != null) glow.StopGlow();
        if (messageWindow != null) messageWindow.ShowMessage(GetWindowText(currentBeat));
        float dur = PlayVoice(GetVoiceClip(currentBeat));

        canAdvance = false;
        SendCustomEventDelayedSeconds(nameof(EnableAdvance), dur + advanceGuardExtra);

        // 再使用防止
        if (interactables[currentBeat] != null) interactables[currentBeat].SetActive(false);
    }

    public void EnableAdvance()
    {
        canAdvance = true;
    }

    // 「次へ」トリガー（NextButton）から呼ぶ
    public void TryAdvance()
    {
        if (!canAdvance) return; // ボイス中・未使用中は無効
        currentBeat++;
        if (currentBeat >= interactables.Length)
        {
            EndExperience();
        }
        else
        {
            EnterBeat();
        }
    }

    private void EndExperience()
    {
        canAdvance = false;
        if (messageWindow == null) return;

        if (!string.IsNullOrEmpty(edText))
        {
            messageWindow.ShowMessage(edText);
            float dur = PlayVoice(edVoice);
            SendCustomEventDelayedSeconds(nameof(ShowEndInstructions), dur + advanceGuardExtra);
        }
        else
        {
            ShowEndInstructions();
        }
    }

    public void ShowEndInstructions()
    {
        if (messageWindow == null) return;

        VRCPlayerApi player = Networking.LocalPlayer;
        bool inVR = player != null && player.IsUserInVR();
        messageWindow.ShowMessage(inVR ? endTextVR : endTextDesktop);
    }

    // clip を再生し、進行ガード長（秒）を返す。clip が null の時は noVoiceGuardSeconds。
    private float PlayVoice(AudioClip clip)
    {
        if (voiceSource == null || clip == null) return noVoiceGuardSeconds;
        voiceSource.Stop();
        voiceSource.clip = clip;
        voiceSource.Play();
        return clip.length;
    }

    private GlowHighlight GetGlow(int index)
    {
        if (glows == null || index < 0 || index >= glows.Length) return null;
        return glows[index];
    }

    private string GetWindowText(int index)
    {
        if (windowTexts == null || index < 0 || index >= windowTexts.Length) return "";
        return windowTexts[index];
    }

    private AudioClip GetVoiceClip(int index)
    {
        if (voices == null || index < 0 || index >= voices.Length) return null;
        return voices[index];
    }
}