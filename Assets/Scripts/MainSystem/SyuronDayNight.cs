using UdonSharp;
using UnityEngine;

/// <summary>
/// Aether 空の昼夜切り替え。BeatSequencer の obj4 / obj5 から呼ぶ。
/// TimeController_Light 上の AetherTime と同じ GameObject、または子に置く。
/// </summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class SyuronDayNight : UdonSharpBehaviour
{
    [Header("参照")]
    [Tooltip("TimeController_Light の AetherTime")]
    public AetherTime aetherTime;

    [Header("時刻（Aether Manual Time / _TimeOfDay）")]
    [Tooltip("OP〜obj3：昼")]
    public float dayTime = 0.42f;
    [Tooltip("obj4：夕方")]
    public float duskTime = 0.76f;
    [Tooltip("obj5〜ED：夜")]
    public float nightTime = 0.88f;

    [Tooltip("現在値から目標まで補間する秒数")]
    public float transitionSeconds = 40f;

    private float _current;
    private float _target;

    void Start()
    {
        _current = dayTime;
        _target = dayTime;
        Apply();
    }

    void Update()
    {
        if (Mathf.Approximately(_current, _target))
            return;

        float step = Time.deltaTime / Mathf.Max(transitionSeconds, 0.1f);
        _current = Mathf.MoveTowards(_current, _target, step);
        Apply();
    }

    void Apply()
    {
        if (aetherTime == null)
            return;

        aetherTime.SetManualTimeOfDay(_current);
    }

    public void GoDay()
    {
        _target = dayTime;
    }

    public void GoDusk()
    {
        _target = duskTime;
    }

    public void GoNight()
    {
        _target = nightTime;
    }
}
