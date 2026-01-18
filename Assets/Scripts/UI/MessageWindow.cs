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

    [Header("追従設定 (Mode 0, 1)")]
    [Tooltip("カメラからウィンドウまでの距離 (m)")]
    public float distance = 1.5f;

    [Tooltip("追従のスムーズさ (大きいほど速い)")]
    public float followSpeed = 5.0f;

    [Tooltip("画面中央からの位置オフセット")]
    public Vector3 viewOffset = new Vector3(0f, -0.3f, 0f);

    [Header("ポップアップ設定 (Mode 1)")]
    [Tooltip("ポップアップの表示時間 (秒)")]
    public float popupDuration = 5.0f;

    [Header("完全固定設定 (Mode 2)")]
    [Tooltip("固定表示時のアンカー位置")]
    public Transform worldFixedAnchor;

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

    void Start()
    {
        // VRモードかどうかを判定
        VRCPlayerApi player = Networking.LocalPlayer;
        if (player != null)
        {
            isVRMode = player.IsUserInVR();
        }

        // デスクトップモードの場合、パラメーターを調整
        if (!isVRMode)
        {
            distance = 2.0f;
            followSpeed = 8.0f;
            viewOffset = new Vector3(0f, -0.4f, 0f);
        }

        // 初期状態は非表示
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
        gameObject.SetActive(false);
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

    #endregion

    #region Private Methods

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
        if (worldFixedAnchor == null) return;

        // アンカー位置に固定
        transform.position = worldFixedAnchor.position;
        transform.rotation = worldFixedAnchor.rotation;
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
