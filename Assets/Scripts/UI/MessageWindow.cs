using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;

/// <summary>
/// VR可変式メッセージウィンドウシステム
/// 3つの表示モードをサポート:
/// - Mode 0: 常時表示 (Always On) - 視点追従
/// - Mode 1: ポップアップ (Pop-up) - 一定時間後に消滅
/// - Mode 2: 完全固定 (World Fixed) - ワールド座標に固定
/// </summary>
public class MessageWindow : UdonSharpBehaviour
{
    [Header("表示モード設定")]
    [Tooltip("0: 常時表示, 1: ポップアップ, 2: 完全固定")]
    public int displayMode = 0;

    [Header("追従設定 (Mode 0, 1) — Desktop")]
    [Tooltip("desktop：視点からウィンドウまでの距離 (m)")]
    public float desktopDistance = 1.2f;
    [Tooltip("desktop：追従のスムーズさ")]
    public float desktopFollowSpeed = 8.0f;
    [Tooltip("desktop：視線方向からの位置オフセット")]
    public Vector3 desktopViewOffset = new Vector3(0f, -0.4f, 0f);

    [Header("追従設定 (Mode 0, 1) — VR")]
    [Tooltip("VR：視点からウィンドウまでの距離 (m)")]
    public float vrDistance = 1.5f;
    [Tooltip("VR：追従のスムーズさ")]
    public float vrFollowSpeed = 5.0f;
    [Tooltip("VR：視線方向からの位置オフセット")]
    public Vector3 vrViewOffset = new Vector3(0f, -0.3f, 0f);

    [Header("ポップアップ設定 (Mode 1)")]
    [Tooltip("ポップアップの表示時間 (秒)")]
    public float popupDuration = 5.0f;

    [Header("完全固定設定 (Mode 2)")]
    [Tooltip("フェーズごとの固定表示アンカー位置")]
    public Transform[] worldFixedAnchors;
    private int currentAnchorIndex = 0;

    [Header("GazeGuide連携")]
    [Tooltip("連携する注視誘導システム")]
    public UdonSharpBehaviour gazeGuide;

    [Header("UI参照")]
    [Tooltip("背景パネル")]
    public GameObject backgroundPanel;

    [Tooltip("メッセージテキスト (TextMeshPro)")]
    public TextMeshProUGUI messageText;

    [Tooltip("フェード用 CanvasGroup")]
    public CanvasGroup canvasGroup;

    [Header("フェード設定")]
    [Tooltip("フェードの持続時間 (秒)")]
    public float fadeDuration = 0.3f;

    // 内部状態
    private bool isVisible = false;
    private float popupTimer = 0f;
    private bool isVRMode = false;
    private float distance = 1.5f;
    private float followSpeed = 5.0f;
    private Vector3 viewOffset = new Vector3(0f, -0.3f, 0f);

    void Start()
    {
        // VRモードかどうかを判定
        VRCPlayerApi player = Networking.LocalPlayer;
        if (player != null)
        {
            isVRMode = player.IsUserInVR();
        }

        ApplyPlatformFollowSettings();

        // 初期状態は非表示（BeatSequencer 等が Start より先に ShowMessage した場合は維持）
        if (!isVisible)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }
            gameObject.SetActive(false);
        }
    }

    void LateUpdate()
    {
        // 表示中のみ処理
        if (!isVisible) return;

        // モードに応じた位置更新
        switch (displayMode)
        {
            case 0: // 常時表示
                UpdatePositionAlwaysOn();
                break;
            case 1: // ポップアップ
                UpdatePopup();
                break;
            case 2: // 完全固定
                UpdatePositionWorldFixed();
                break;
        }

        // フェード処理
        HandleFade();
    }

    #region Public Methods

    /// <summary>
    /// テキストを更新してウィンドウを表示する
    /// </summary>
    public void ShowMessage(string text)
    {
        if (messageText != null)
        {
            messageText.text = text;
        }

        isVisible = true;
        gameObject.SetActive(true);

        // ポップアップモードの場合、タイマーをリセット
        if (displayMode == 1)
        {
            popupTimer = popupDuration;
        }

        Debug.Log($"[MessageWindow] メッセージ表示: {text}");
    }

    /// <summary>
    /// ウィンドウを非表示にする
    /// </summary>
    public void HideWindow()
    {
        isVisible = false;
        Debug.Log("[MessageWindow] ウィンドウ非表示");
    }

    /// <summary>
    /// 動作モードを切り替える
    /// </summary>
    public void SetMode(int mode)
    {
        displayMode = mode;
        Debug.Log($"[MessageWindow] モード変更: {mode}");
    }

    /// <summary>
    /// ポップアップメッセージを表示 (Mode 1用)
    /// </summary>
    public void ShowPopup(string text)
    {
        int previousMode = displayMode;
        displayMode = 1;
        ShowMessage(text);
        // 元のモードに戻さない（ポップアップ終了後も現在のモードを維持）
    }

    /// <summary>
    /// 注視誘導と同時にメッセージを表示する
    /// </summary>
    /// <param name="text">表示するメッセージ</param>
    /// <param name="target">注視対象のTransform</param>
    public void ShowWithGaze(string text, Transform target)
    {
        if (gazeGuide != null && target != null)
        {
            gazeGuide.SetProgramVariable("target", target);
            gazeGuide.SendCustomEvent("StartGuide");
        }
        ShowMessage(text);
    }

    /// <summary>
    /// World Fixedモードのアンカーをインデックスで切り替える
    /// </summary>
    /// <param name="index">アンカーのインデックス</param>
    public void SetWorldFixedAnchor(int index)
    {
        if (worldFixedAnchors != null && index >= 0 && index < worldFixedAnchors.Length)
        {
            currentAnchorIndex = index;
            Debug.Log($"[MessageWindow] アンカー切り替え: {index}");
        }
    }

    /// <summary>
    /// World Fixedモードのアンカーを直接指定する（MessageTrigger向け）
    /// </summary>
    /// <param name="anchor">使用するアンカーTransform</param>
    public void SetWorldFixedAnchorDirect(Transform anchor)
    {
        if (worldFixedAnchors != null && worldFixedAnchors.Length > 0)
        {
            // 配列の最初の要素を上書き（一時的）
            worldFixedAnchors[0] = anchor;
            currentAnchorIndex = 0;
        }
        Debug.Log($"[MessageWindow] アンカー直接設定: {(anchor != null ? anchor.name : "null")}");
    }

    #endregion

    #region Private Methods

    private void ApplyPlatformFollowSettings()
    {
        if (isVRMode)
        {
            distance = vrDistance;
            followSpeed = vrFollowSpeed;
            viewOffset = vrViewOffset;
        }
        else
        {
            distance = desktopDistance;
            followSpeed = desktopFollowSpeed;
            viewOffset = desktopViewOffset;
        }
    }

    /// <summary>
    /// Mode 0: 常時表示の位置更新
    /// </summary>
    private void UpdatePositionAlwaysOn()
    {
        VRCPlayerApi player = Networking.LocalPlayer;
        if (player == null) return;

        // 頭のトラッキングデータを取得
        var headData = player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
        Vector3 headPos = headData.position;
        Quaternion headRot = headData.rotation;

        // 目標位置を計算
        Vector3 forward = headRot * Vector3.forward;
        Vector3 offset = headRot * viewOffset;
        Vector3 targetPos = headPos + forward * distance + offset;

        // 滑らかに追従
        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            Time.deltaTime * followSpeed
        );

        // プレイヤーの方を向く（ビルボード処理）
        transform.LookAt(headPos);
        transform.Rotate(0, 180f, 0);
    }

    /// <summary>
    /// Mode 1: ポップアップの更新
    /// </summary>
    private void UpdatePopup()
    {
        // 位置は常時表示と同じ
        UpdatePositionAlwaysOn();

        // タイマー更新
        popupTimer -= Time.deltaTime;
        if (popupTimer <= 0f)
        {
            HideWindow();
        }
    }

    /// <summary>
    /// Mode 2: 完全固定の位置更新
    /// </summary>
    private void UpdatePositionWorldFixed()
    {
        if (worldFixedAnchors == null || worldFixedAnchors.Length == 0) return;
        if (currentAnchorIndex < 0 || currentAnchorIndex >= worldFixedAnchors.Length) return;
        
        Transform anchor = worldFixedAnchors[currentAnchorIndex];
        if (anchor == null) return;

        // アンカー位置に固定
        transform.position = anchor.position;
        transform.rotation = anchor.rotation;
    }

    /// <summary>
    /// フェード処理
    /// </summary>
    private void HandleFade()
    {
        if (canvasGroup == null) return;

        float targetAlpha = isVisible ? 1f : 0f;
        float current = canvasGroup.alpha;

        // 目標に向かって徐々に変化
        canvasGroup.alpha = Mathf.MoveTowards(current, targetAlpha, Time.deltaTime / fadeDuration);

        // 完全に透明になったら非アクティブ化（パフォーマンス）
        if (canvasGroup.alpha == 0f && !isVisible)
        {
            gameObject.SetActive(false);
        }
    }

    #endregion
}
