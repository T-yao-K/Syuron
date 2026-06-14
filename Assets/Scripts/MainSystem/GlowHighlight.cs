using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// 発光導線（GazeGuide の最小代替）。次に使うオブジェクトを Emission パルスで誘導する。
/// StartGlow / StopGlow を SendCustomEvent または直接呼び出しで制御（BeatSequencer から使う）。
///
/// ※ 重要：対象マテリアルのインスペクタで「Emission」を有効にしておくこと。
///    （これで _EMISSION キーワードが焼かれ、実行時の SetColor("_EmissionColor", ...) が効く。
///     コードから EnableKeyword を呼ばない方式にしてある＝Udon で確実に動く。）
/// </summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class GlowHighlight : UdonSharpBehaviour
{
    [Header("対象")]
    [Tooltip("発光させる Renderer（未指定なら自身の Renderer）")]
    public Renderer targetRenderer;

    [Header("見た目")]
    [Tooltip("発光色（ガス灯っぽい暖色を既定に）")]
    public Color glowColor = new Color(1f, 0.85f, 0.4f);

    [Tooltip("パルスの速さ")]
    public float pulseSpeed = 2.0f;

    [Tooltip("最大エミッション強度")]
    public float maxIntensity = 2.0f;

    [Tooltip("起動時から光らせておくか")]
    public bool glowOnStart = false;

    private bool isGlowing = false;
    private Material mat;
    private float t = 0f;

    void Start()
    {
        if (targetRenderer == null) targetRenderer = GetComponent<Renderer>();
        if (targetRenderer != null)
        {
            mat = targetRenderer.material; // インスタンス化される（共有マテリアルを汚さない）
            SetEmission(0f);
        }

        if (glowOnStart) StartGlow();
    }

    /// <summary>発光開始</summary>
    public void StartGlow()
    {
        isGlowing = true;
        t = 0f;
    }

    /// <summary>発光停止</summary>
    public void StopGlow()
    {
        isGlowing = false;
        SetEmission(0f);
    }

    void Update()
    {
        if (!isGlowing || mat == null) return;

        t += Time.deltaTime * pulseSpeed;
        float v = (Mathf.Sin(t) * 0.5f + 0.5f) * maxIntensity; // 0..maxIntensity
        SetEmission(v);
    }

    private void SetEmission(float intensity)
    {
        if (mat == null) return;
        mat.SetColor("_EmissionColor", glowColor * intensity);
    }
}