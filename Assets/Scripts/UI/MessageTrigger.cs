using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// メッセージトリガーコンポーネント
/// オブジェクトにアタッチして、インタラクト時やイベント時にメッセージを表示する
/// メッセージ内容を外部から設定可能にし、コードからメッセージをハードコードする必要をなくす
/// </summary>
public class MessageTrigger : UdonSharpBehaviour
{
    [Header("メッセージ設定")]
    [Tooltip("表示するメッセージ")]
    [TextArea(3, 10)]
    public string message;

    [Tooltip("使用する表示モード (0: Always On, 1: Pop-up, 2: World Fixed)")]
    [Range(0, 2)]
    public int displayMode = 0;

    [Header("Mode 2用: World Fixed Anchor")]
    [Tooltip("このトリガー用のアンカー（Mode 2 使用時に設定）")]
    public Transform anchor;

    [Header("GazeGuide連携")]
    [Tooltip("注視誘導を使用するか")]
    public bool useGazeGuide = false;

    [Tooltip("注視対象（GazeGuide使用時に設定）")]
    public Transform gazeTarget;

    [Header("トリガー設定")]
    [Tooltip("インタラクト時にメッセージを表示するか")]
    public bool triggerOnInteract = true;

    [Tooltip("トリガー後にメッセージウィンドウを非表示にするまでの時間（0で自動非表示しない）")]
    public float autoHideDelay = 0f;

    [Header("参照")]
    [Tooltip("MessageWindowへの参照")]
    public MessageWindow messageWindow;

    /// <summary>
    /// VRChatのインタラクトイベント
    /// </summary>
    public override void Interact()
    {
        if (triggerOnInteract)
        {
            TriggerMessage();
        }
    }

    /// <summary>
    /// メッセージを表示する
    /// 他のスクリプトから呼び出し可能
    /// </summary>
    public void TriggerMessage()
    {
        if (messageWindow == null)
        {
            Debug.LogWarning("[MessageTrigger] MessageWindowが設定されていません");
            return;
        }

        if (string.IsNullOrEmpty(message))
        {
            Debug.LogWarning("[MessageTrigger] メッセージが設定されていません");
            return;
        }

        // モードを設定
        messageWindow.SetMode(displayMode);

        // Mode 2 の場合、アンカーを設定
        if (displayMode == 2 && anchor != null)
        {
            messageWindow.SetWorldFixedAnchorDirect(anchor);
        }

        // GazeGuide連携がある場合
        if (useGazeGuide && gazeTarget != null)
        {
            messageWindow.ShowWithGaze(message, gazeTarget);
        }
        else
        {
            messageWindow.ShowMessage(message);
        }

        Debug.Log($"[MessageTrigger] メッセージ表示: {message.Substring(0, Mathf.Min(30, message.Length))}...");

        // 自動非表示が設定されている場合
        if (autoHideDelay > 0f)
        {
            SendCustomEventDelayedSeconds(nameof(HideMessage), autoHideDelay);
        }
    }

    /// <summary>
    /// メッセージを非表示にする
    /// </summary>
    public void HideMessage()
    {
        if (messageWindow != null)
        {
            messageWindow.HideWindow();
        }
    }

    /// <summary>
    /// 外部からメッセージ内容を設定する
    /// </summary>
    public void SetMessage(string newMessage)
    {
        message = newMessage;
    }

    /// <summary>
    /// 外部からモードを設定する
    /// </summary>
    public void SetDisplayMode(int mode)
    {
        displayMode = Mathf.Clamp(mode, 0, 2);
    }
}
