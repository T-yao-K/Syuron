using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// 注視誘導システム (GazeGuide)
/// プレイヤーの視線を重要なオブジェクトに誘導するためのシステム。
/// 
/// 機能:
/// - ハイライト表示: 対象オブジェクトの位置に発光エフェクトを表示
/// - 矢印インジケータ: 視界外の対象への方向を示す矢印を表示
/// - 照準ガイド: 発砲を促す時に敵の位置に照準マーカーを表示
/// - パルスアニメーション: ハイライトの拡大縮小で注目を引く
/// 
/// Unity側のセットアップ:
/// 1. GazeGuideオブジェクトの子に HighlightEffect, ArrowIndicator, AimingGuide を配置
/// 2. 各エフェクトは初期状態で非アクティブにしておく
/// 3. インスペクターで各フィールドにアサインする
/// </summary>
public class GazeGuide : UdonSharpBehaviour
{
    // =========================================
    // インスペクター設定
    // =========================================

    [Header("エフェクト参照")]
    [Tooltip("ハイライトエフェクト（事前配置・初期非アクティブ）")]
    public GameObject highlightEffect;

    [Tooltip("矢印インジケータ（事前配置・初期非アクティブ）")]
    public GameObject arrowIndicator;

    [Tooltip("照準ガイド（事前配置・初期非アクティブ）")]
    public GameObject aimingGuide;

    [Header("パルスアニメーション設定")]
    [Tooltip("パルスの速度（大きいほど速い）")]
    public float pulseSpeed = 2.0f;

    [Tooltip("パルスの最大拡大率")]
    public float pulseScale = 1.2f;

    [Header("視界判定設定")]
    [Tooltip("視界内とみなす角度の閾値（度）")]
    public float viewAngleThreshold = 60.0f;

    [Header("矢印インジケータ設定")]
    [Tooltip("矢印の頭部からの前方距離（m）")]
    public float arrowDistance = 1.5f;

    [Tooltip("矢印の画面端へのオフセット（m）")]
    public float arrowEdgeOffset = 0.6f;

    // =========================================
    // パブリック変数（外部から設定可能）
    // =========================================

    [HideInInspector]
    public Transform target;

    // =========================================
    // 内部状態
    // =========================================

    private bool _isGuiding = false;
    private bool _isAimingGuideActive = false;
    private Vector3 _highlightOriginalScale = Vector3.one;
    private Vector3 _aimingGuideOriginalScale = Vector3.one;

    // =========================================
    // 初期化
    // =========================================

    void Start()
    {
        // 元のスケールを記録
        if (highlightEffect != null)
        {
            _highlightOriginalScale = highlightEffect.transform.localScale;
            highlightEffect.SetActive(false);
        }

        if (arrowIndicator != null)
        {
            arrowIndicator.SetActive(false);
        }

        if (aimingGuide != null)
        {
            _aimingGuideOriginalScale = aimingGuide.transform.localScale;
            aimingGuide.SetActive(false);
        }
    }

    // =========================================
    // 毎フレーム更新
    // =========================================

    void LateUpdate()
    {
        if (_isGuiding && target != null)
        {
            // ハイライトを対象位置に追従
            UpdateHighlightPosition();

            // 視界判定 → 矢印の表示/非表示
            if (IsTargetInView())
            {
                HideArrowIndicator();
            }
            else
            {
                ShowArrowIndicator();
                UpdateArrowPosition();
            }

            // パルスアニメーション
            PlayPulseAnimation();
        }

        if (_isAimingGuideActive && target != null)
        {
            // 照準ガイドを対象位置に追従
            UpdateAimingGuidePosition();
        }
    }

    // =========================================
    // パブリックメソッド
    // =========================================

    /// <summary>
    /// 指定した対象への注視誘導を開始する。
    /// ハイライト表示 + 視界外なら矢印表示。
    /// MessageWindow.ShowWithGaze() から SendCustomEvent("StartGuide") で呼ばれる。
    /// その場合、事前に target が SetProgramVariable で設定される。
    /// </summary>
    public void StartGuide()
    {
        if (target == null)
        {
            Debug.LogWarning("[GazeGuide] target が設定されていません");
            return;
        }

        _isGuiding = true;

        // ハイライトを表示
        if (highlightEffect != null)
        {
            highlightEffect.SetActive(true);
            highlightEffect.transform.position = target.position;
        }

        Debug.Log($"[GazeGuide] 注視誘導を開始: {target.name}");
    }

    /// <summary>
    /// Transform引数付きのStartGuide。
    /// EventSequencer等から直接呼び出す場合に使用。
    /// </summary>
    public void StartGuideWithTarget(Transform newTarget)
    {
        target = newTarget;
        StartGuide();
    }

    /// <summary>
    /// 照準ガイドを指定した対象の位置に表示する。
    /// 発砲を促す場面で使用。
    /// </summary>
    public void StartAimingGuide(Transform newTarget)
    {
        target = newTarget;
        _isAimingGuideActive = true;

        if (aimingGuide != null)
        {
            aimingGuide.transform.position = target.position;
            aimingGuide.SetActive(true);
        }

        Debug.Log($"[GazeGuide] 照準ガイドを表示: {(newTarget != null ? newTarget.name : "null")}");
    }

    /// <summary>
    /// 注視誘導（ハイライト＋矢印）を停止する。
    /// </summary>
    public void StopGuide()
    {
        _isGuiding = false;

        if (highlightEffect != null)
        {
            highlightEffect.SetActive(false);
            // スケールをリセット
            highlightEffect.transform.localScale = _highlightOriginalScale;
        }

        HideArrowIndicator();

        Debug.Log("[GazeGuide] 注視誘導を停止");
    }

    /// <summary>
    /// 照準ガイドのみを停止する。
    /// </summary>
    public void StopAimingGuide()
    {
        _isAimingGuideActive = false;

        if (aimingGuide != null)
        {
            aimingGuide.SetActive(false);
            aimingGuide.transform.localScale = _aimingGuideOriginalScale;
        }

        Debug.Log("[GazeGuide] 照準ガイドを停止");
    }

    /// <summary>
    /// 全てのガイドを停止する（便利メソッド）。
    /// </summary>
    public void StopAll()
    {
        StopGuide();
        StopAimingGuide();
        target = null;
    }

    // =========================================
    // プライベートメソッド
    // =========================================

    /// <summary>
    /// ハイライトを対象位置に追従させる
    /// </summary>
    private void UpdateHighlightPosition()
    {
        if (highlightEffect == null || target == null) return;

        highlightEffect.transform.position = target.position;
    }

    /// <summary>
    /// 照準ガイドを対象位置に追従させる
    /// </summary>
    private void UpdateAimingGuidePosition()
    {
        if (aimingGuide == null || target == null) return;

        aimingGuide.transform.position = target.position;

        // プレイヤーの方を向く（ビルボード処理）
        VRCPlayerApi player = Networking.LocalPlayer;
        if (player != null)
        {
            var headData = player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
            aimingGuide.transform.LookAt(headData.position);
            aimingGuide.transform.Rotate(0, 180f, 0);
        }
    }

    /// <summary>
    /// 対象がプレイヤーの視界内にあるかどうかを判定する
    /// </summary>
    private bool IsTargetInView()
    {
        VRCPlayerApi player = Networking.LocalPlayer;
        if (player == null || target == null) return true; // 安全のため視界内扱い

        var headData = player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
        Vector3 headPos = headData.position;
        Vector3 headForward = headData.rotation * Vector3.forward;

        Vector3 toTarget = (target.position - headPos).normalized;
        float angle = Vector3.Angle(headForward, toTarget);

        return angle <= viewAngleThreshold;
    }

    /// <summary>
    /// 矢印インジケータを表示する
    /// </summary>
    private void ShowArrowIndicator()
    {
        if (arrowIndicator != null && !arrowIndicator.activeSelf)
        {
            arrowIndicator.SetActive(true);
        }
    }

    /// <summary>
    /// 矢印インジケータを非表示にする
    /// </summary>
    private void HideArrowIndicator()
    {
        if (arrowIndicator != null && arrowIndicator.activeSelf)
        {
            arrowIndicator.SetActive(false);
        }
    }

    /// <summary>
    /// 矢印インジケータの位置と向きを更新する。
    /// プレイヤーの視界の端に配置し、対象の方向を指す。
    /// </summary>
    private void UpdateArrowPosition()
    {
        if (arrowIndicator == null || target == null) return;

        VRCPlayerApi player = Networking.LocalPlayer;
        if (player == null) return;

        var headData = player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
        Vector3 headPos = headData.position;
        Quaternion headRot = headData.rotation;

        // 頭部の前方方向
        Vector3 headForward = headRot * Vector3.forward;

        // 対象への方向ベクトル
        Vector3 toTarget = target.position - headPos;

        // 頭部の前方平面へ投影し、方向成分のみ取り出す
        Vector3 projectedDirection = Vector3.ProjectOnPlane(toTarget, headForward);

        // 投影ベクトルがゼロに近い場合（対象が真正面or真後ろ）は上方向をフォールバック
        if (projectedDirection.sqrMagnitude < 0.001f)
        {
            projectedDirection = headRot * Vector3.up;
        }

        projectedDirection.Normalize();

        // 矢印を視界の端に配置
        // 頭部位置 + 前方(arrowDistance) + 方向(arrowEdgeOffset)
        Vector3 arrowPos = headPos
            + headForward * arrowDistance
            + projectedDirection * arrowEdgeOffset;

        arrowIndicator.transform.position = arrowPos;

        // 矢印を対象の方向に向ける
        Vector3 lookDirection = target.position - arrowPos;
        if (lookDirection.sqrMagnitude > 0.001f)
        {
            arrowIndicator.transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }

    /// <summary>
    /// ハイライトエフェクトのパルスアニメーション。
    /// Sin波でスケールを拡大縮小させ、注目を引く。
    /// </summary>
    private void PlayPulseAnimation()
    {
        if (highlightEffect == null) return;

        // Sin波で 1.0 ～ pulseScale の範囲をスムーズに変化
        float pulse = 1.0f + (pulseScale - 1.0f) * (Mathf.Sin(Time.time * pulseSpeed) * 0.5f + 0.5f);

        highlightEffect.transform.localScale = _highlightOriginalScale * pulse;
    }
}
