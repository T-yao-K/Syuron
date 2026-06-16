using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// 単一環境のビート進行コントローラ。
///
/// 流れ:
///   OP ページを解放 → Next/Back でページング
///   → OP 最終ページ到達で obj1 が発光
///   → オブジェクト Interact でそのビートのページを末尾に追加し先頭へジャンプ
///   → そのビート最終ページ到達で次オブジェクトが発光
///   → 全ビート後 ED ページ → ED 最終ページで「次へ」→ 終了案内ページ（ページング可能・戻れる）
///
/// NextButton / BackButton はページング専用。ビート進行は Interact と最終ページ到達。
/// 使用済みオブジェクトへの Interact は該当ビート先頭ページへジャンプのみ。
/// 全 interactable は常時表示（非 Active 切替なし）。
/// </summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class BeatSequencer : UdonSharpBehaviour
{
    private const int MAX_PAGES = 32;
    private const int BEAT_COUNT = 5;

    [Header("UI / 参照")]
    [Tooltip("既存の MessageWindow")]
    public MessageWindow messageWindow;

    [Header("OP ページ（1行＝1ページ）")]
    public string[] opPages;
    [TextArea(2, 5)]
    [Tooltip("opPages 未設定時のフォールバック（1ページ扱い）")]
    public string opText;

    [Header("各ビートのページ（obj1〜obj5・1行＝1ページ）")]
    public string[] obj1Pages;
    public string[] obj2Pages;
    public string[] obj3Pages;
    public string[] obj4Pages;
    public string[] obj5Pages;

    [Header("発光導線（obj1〜obj5）")]
    [Tooltip("各ビートで光らせる GlowHighlight")]
    public GlowHighlight glowObj1;
    public GlowHighlight glowObj2;
    public GlowHighlight glowObj3;
    public GlowHighlight glowObj4;
    public GlowHighlight glowObj5;

    [Header("レガシー（objNPages 未設定時・1ページ扱い）")]
    [TextArea(2, 6)]
    public string[] windowTexts;

    [Header("ED ページ（全ビート完了後）")]
    public string[] edPages;
    [TextArea(2, 4)]
    [Tooltip("edPages 未設定時のフォールバック")]
    public string edText;

    [Header("終了案内（ED 最終ページの次へで解放）")]
    public string[] endPagesDesktop;
    public string[] endPagesVR;
    [TextArea(2, 4)]
    [Tooltip("endPages 未設定時のフォールバック（1ページ扱い）")]
    public string endTextVR = "以上で体験終了です。\nヘッドセットを外してください。";
    [TextArea(2, 4)]
    public string endTextDesktop = "以上で体験終了です。\n実験者の指示をお待ちください。";

    [Header("Desktop ページ操作")]
    [Tooltip("desktop のみ E=次へ / Q=戻る")]
    public bool enableDesktopKeys = true;

    // 解放済みページ履歴
    private string[] pageHistory = new string[MAX_PAGES];
    private int pageCount = 0;
    private int viewPageIndex = 0;

    // ビート状態
    private int nextInteractBeat = 0;
    private bool interactGlowEnabled = false;
    private int awaitingFinalPageBeat = -1;
    private int[] beatFirstPage = new int[BEAT_COUNT];
    private int[] beatLastPage = new int[BEAT_COUNT];
    private bool[] beatUsed = new bool[BEAT_COUNT];

    private int opLastPageIndex = -1;
    private bool inEdPhase = false;
    private int edFirstPage = -1;
    private int edLastPage = -1;
    private bool inEndPhase = false;
    private bool interactLocked = false;

    void Start()
    {
        for (int i = 0; i < BEAT_COUNT; i++)
        {
            beatUsed[i] = false;
            beatFirstPage[i] = -1;
            beatLastPage[i] = -1;
        }

        StopAllGlows();
        SendCustomEventDelayedFrames(nameof(Bootstrap), 1);
    }

    public void Bootstrap()
    {
        AppendPages(GetOpPages());
        opLastPageIndex = pageCount - 1;
        viewPageIndex = 0;
        nextInteractBeat = 0;
        interactGlowEnabled = false;
        awaitingFinalPageBeat = -1;
        inEdPhase = false;
        inEndPhase = false;
        interactLocked = false;
        edLastPage = -1;

        ShowCurrentPage();
        CheckOpCompleteAndEnableGlow();
    }

    void Update()
    {
        if (!enableDesktopKeys) return;

        VRCPlayerApi player = Networking.LocalPlayer;
        if (player == null || player.IsUserInVR()) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPageNext();
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            TryPageBack();
        }
    }

    // ---------- ページング（NextButton / BackButton / Desktop E・Q） ----------

    public void TryPageNext()
    {
        if (viewPageIndex < pageCount - 1)
        {
            viewPageIndex++;
            ShowCurrentPage();
            OnPageViewChanged();
            return;
        }

        // ED 最終ページで「次へ」→ 終了案内ページを解放
        if (inEdPhase && !inEndPhase && viewPageIndex == edLastPage)
        {
            BeginEndPhase();
        }
    }

    public void TryPageBack()
    {
        if (viewPageIndex <= 0) return;

        viewPageIndex--;
        ShowCurrentPage();
    }

    private void OnPageViewChanged()
    {
        if (inEndPhase) return;

        CheckOpCompleteAndEnableGlow();

        if (awaitingFinalPageBeat >= 0 && viewPageIndex == beatLastPage[awaitingFinalPageBeat])
        {
            OnBeatFinalPageReached(awaitingFinalPageBeat);
        }
    }

    // ---------- オブジェクト Interact（BeatInteract から） ----------

    public void UseBeatByGlow(GlowHighlight glow)
    {
        if (glow == null) return;

        for (int i = 0; i < BEAT_COUNT; i++)
        {
            if (GetGlow(i) == glow)
            {
                UseBeat(i);
                return;
            }
        }
    }

    public void UseBeat(int beatIndex)
    {
        if (beatIndex < 0 || beatIndex >= BEAT_COUNT) return;

        if (beatUsed[beatIndex])
        {
            JumpToBeatPages(beatIndex);
            return;
        }

        if (interactLocked || !interactGlowEnabled || beatIndex != nextInteractBeat) return;

        beatUsed[beatIndex] = true;
        int firstNew = pageCount;
        AppendPages(GetBeatPages(beatIndex));
        beatFirstPage[beatIndex] = firstNew;
        beatLastPage[beatIndex] = pageCount - 1;

        StopGlowForBeat(beatIndex);
        interactGlowEnabled = false;
        awaitingFinalPageBeat = beatIndex;
        viewPageIndex = firstNew;
        ShowCurrentPage();

        if (viewPageIndex == beatLastPage[beatIndex])
        {
            OnBeatFinalPageReached(beatIndex);
        }
    }

    private void JumpToBeatPages(int beatIndex)
    {
        if (beatFirstPage[beatIndex] < 0) return;
        viewPageIndex = beatFirstPage[beatIndex];
        ShowCurrentPage();
    }

    // ---------- ビート完了・ED・終了 ----------

    private void OnBeatFinalPageReached(int beatIndex)
    {
        awaitingFinalPageBeat = -1;

        if (beatIndex < BEAT_COUNT - 1)
        {
            nextInteractBeat = beatIndex + 1;
            interactGlowEnabled = true;
            StartGlowForBeat(nextInteractBeat);
            return;
        }

        BeginEdPhase();
    }

    private void BeginEdPhase()
    {
        if (inEdPhase) return;

        inEdPhase = true;
        interactGlowEnabled = false;
        StopAllGlows();

        edFirstPage = pageCount;
        AppendPages(GetEdPages());
        edLastPage = pageCount - 1;
        if (edFirstPage > edLastPage) return;

        viewPageIndex = edFirstPage;
        ShowCurrentPage();
    }

    private void BeginEndPhase()
    {
        if (inEndPhase) return;

        inEndPhase = true;
        interactLocked = true;
        interactGlowEnabled = false;
        StopAllGlows();

        int endFirstPage = pageCount;
        AppendPages(GetEndPages());
        if (endFirstPage >= pageCount) return;

        viewPageIndex = endFirstPage;
        ShowCurrentPage();
    }

    // ---------- OP 完了 → 最初の発光 ----------

    private void CheckOpCompleteAndEnableGlow()
    {
        if (interactGlowEnabled || inEdPhase || inEndPhase || interactLocked) return;
        if (opLastPageIndex < 0) return;
        if (viewPageIndex < opLastPageIndex) return;
        // いずれかのビート使用後は OP 用の再発光をしない
        if (IsAnyBeatUsed()) return;

        interactGlowEnabled = true;
        StartGlowForBeat(nextInteractBeat);
    }

    private bool IsAnyBeatUsed()
    {
        for (int i = 0; i < BEAT_COUNT; i++)
        {
            if (beatUsed[i]) return true;
        }
        return false;
    }

    // ---------- ページ履歴 ----------

    private void AppendPages(string[] pages)
    {
        if (pages == null) return;

        for (int i = 0; i < pages.Length; i++)
        {
            if (pageCount >= MAX_PAGES) return;
            if (string.IsNullOrEmpty(pages[i])) continue;
            pageHistory[pageCount] = pages[i];
            pageCount++;
        }
    }

    private void ShowCurrentPage()
    {
        if (messageWindow == null) return;
        if (viewPageIndex < 0 || viewPageIndex >= pageCount) return;

        messageWindow.SetMode(0);
        messageWindow.ShowPage(pageHistory[viewPageIndex], viewPageIndex + 1, pageCount);
    }

    private string[] GetOpPages()
    {
        if (opPages != null && opPages.Length > 0) return opPages;
        return SinglePage(opText);
    }

    private string[] GetEdPages()
    {
        if (edPages != null && edPages.Length > 0) return edPages;
        return SinglePage(edText);
    }

    private string[] GetEndPages()
    {
        VRCPlayerApi player = Networking.LocalPlayer;
        bool inVR = player != null && player.IsUserInVR();

        if (inVR)
        {
            if (endPagesVR != null && endPagesVR.Length > 0) return endPagesVR;
            return SplitLinesToPages(endTextVR);
        }

        if (endPagesDesktop != null && endPagesDesktop.Length > 0) return endPagesDesktop;
        return SplitLinesToPages(endTextDesktop);
    }

    private string[] SplitLinesToPages(string text)
    {
        if (string.IsNullOrEmpty(text)) return new string[0];
        return text.Split(new char[] { '\n' });
    }

    private string[] GetBeatPages(int beatIndex)
    {
        string[] pages = null;
        switch (beatIndex)
        {
            case 0: pages = obj1Pages; break;
            case 1: pages = obj2Pages; break;
            case 2: pages = obj3Pages; break;
            case 3: pages = obj4Pages; break;
            case 4: pages = obj5Pages; break;
        }

        if (pages != null && pages.Length > 0) return pages;

        if (windowTexts != null && beatIndex >= 0 && beatIndex < windowTexts.Length)
        {
            return SinglePage(windowTexts[beatIndex]);
        }

        return new string[0];
    }

    private string[] SinglePage(string text)
    {
        if (string.IsNullOrEmpty(text)) return new string[0];
        string[] one = new string[1];
        one[0] = text;
        return one;
    }

    // ---------- 発光 ----------

    private void StartGlowForBeat(int beatIndex)
    {
        GlowHighlight glow = GetGlow(beatIndex);
        if (glow != null) glow.StartGlow();
    }

    private void StopGlowForBeat(int beatIndex)
    {
        GlowHighlight glow = GetGlow(beatIndex);
        if (glow != null) glow.StopGlow();
    }

    private void StopAllGlows()
    {
        for (int i = 0; i < BEAT_COUNT; i++)
        {
            StopGlowForBeat(i);
        }
    }

    private GlowHighlight GetGlow(int index)
    {
        switch (index)
        {
            case 0: return glowObj1;
            case 1: return glowObj2;
            case 2: return glowObj3;
            case 3: return glowObj4;
            case 4: return glowObj5;
            default: return null;
        }
    }
}
