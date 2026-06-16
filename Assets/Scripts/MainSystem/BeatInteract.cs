using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// 各インタラクト対象に付ける薄い転送スクリプト。
/// 同一オブジェクトの GlowHighlight からビート番号を解決する（beatIndex 手入力より確実）。
/// </summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class BeatInteract : UdonSharpBehaviour
{
    [Tooltip("シーン上の BeatSequencer を割り当てる")]
    public BeatSequencer sequencer;

    [Tooltip("同一オブジェクトの GlowHighlight（未設定なら自動取得）")]
    public GlowHighlight glowHighlight;

    [Tooltip("glowHighlight が解決できない場合のフォールバック（0=obj1 … 4=obj5）")]
    public int beatIndex;

    void Start()
    {
        if (glowHighlight == null)
        {
            glowHighlight = GetComponent<GlowHighlight>();
        }
    }

    public override void Interact()
    {
        if (sequencer == null) return;

        if (glowHighlight != null)
        {
            sequencer.UseBeatByGlow(glowHighlight);
        }
        else
        {
            sequencer.UseBeat(beatIndex);
        }
    }
}
