using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// 各インタラクト対象に付ける薄い転送スクリプト。
/// VRChat の Interact（Head ポインティング＋クリック）を BeatSequencer.UseCurrentBeat に転送する。
/// current ビートのオブジェクトのみ有効化されているため、index 管理は BeatSequencer 側に集約される。
/// </summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class BeatInteract : UdonSharpBehaviour
{
    [Tooltip("シーン上の BeatSequencer を割り当てる")]
    public BeatSequencer sequencer;

    public override void Interact()
    {
        if (sequencer != null)
        {
            sequencer.UseCurrentBeat();
        }
    }
}