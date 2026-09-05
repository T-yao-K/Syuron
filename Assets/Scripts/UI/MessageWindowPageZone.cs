using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

/// <summary>
/// MessageWindow 左右の Interact ゾーン。VR のみページ送りする。
/// </summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class MessageWindowPageZone : UdonSharpBehaviour
{
    [Tooltip("シーン上の BeatSequencer")]
    public BeatSequencer sequencer;

    [Tooltip("true=次へ / false=戻る")]
    public bool isNext = true;

    public override void Interact()
    {
        VRCPlayerApi player = Networking.LocalPlayer;
        if (player == null || !player.IsUserInVR()) return;
        if (sequencer == null) return;

        if (isNext)
        {
            sequencer.TryPageNext();
        }
        else
        {
            sequencer.TryPageBack();
        }
    }
}
