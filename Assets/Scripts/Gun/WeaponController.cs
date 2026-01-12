using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class WeaponController : UdonSharpBehaviour
{
    [Header("武器の性能設定")]
    [Tooltip("射程距離(メートル)")]
    public float maxRange = 50f; // ここを火縄銃なら50、ミニエー銃なら500にする
    
    [Tooltip("リロード時間(秒)")]
    public float reloadTime = 3.0f;

    [Header("参照設定")]
    [Tooltip("弾が出る場所(銃口)のTransform")]
    public Transform muzzlePoint;

    private bool isReady = true; // 撃てる状態か？

    public override void OnPickupUseDown()
    {
        // VRChatで「Use(トリガー)」を引いた瞬間に呼ばれるイベント
        if (isReady)
        {
            Fire();
        }
        else
        {
            Debug.Log("リロード中...");
        }
    }

    private void Fire()
    {
        // 1. 発砲処理（音やエフェクトはあとでここに追加）
        Debug.Log("発砲！");
        isReady = false;
        
        // 2. リロード開始（指定時間後にResetGunを呼ぶ）
        SendCustomEventDelayedSeconds(nameof(ResetGun), reloadTime);

        // 3. 判定処理 (Raycast)
        // 銃口の位置から、前方にレイ(見えない光線)を飛ばす
        RaycastHit hit;
        if (Physics.Raycast(muzzlePoint.position, muzzlePoint.forward, out hit, maxRange))
        {
            // 何かに当たった！
            Debug.Log($"命中: {hit.collider.gameObject.name} (距離: {hit.distance:F1}m)");

            // デバッグ用：当たった場所に赤い線を3秒間表示
            Debug.DrawLine(muzzlePoint.position, hit.point, Color.red, 3f);

            // 【重要】もし当たった相手が「敵」なら、ダメージを与える処理をここに書く
            // var enemy = hit.collider.GetComponent<EnemyController>();
            // if (enemy != null) enemy.TakeDamage();
        }
        else
        {
            // 射程外、またはハズレ
            Debug.Log("ミス！届きませんでした。");
            
            // デバッグ用：射程ギリギリまで緑の線を表示
            Debug.DrawRay(muzzlePoint.position, muzzlePoint.forward * maxRange, Color.green, 3f);
        }
    }

    public void ResetGun()
    {
        isReady = true;
        Debug.Log("リロード完了！");
    }
}