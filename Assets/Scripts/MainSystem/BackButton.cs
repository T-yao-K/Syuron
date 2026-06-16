using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BackButton : UdonSharpBehaviour
{
    [Tooltip("シーン上の BeatSequencer を割り当てる")]
    public BeatSequencer sequencer;

    public override void Interact()
    {
        if (sequencer != null)
        {
            sequencer.TryPageBack();
        }
    }
}
