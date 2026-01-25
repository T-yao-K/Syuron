using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GameManager : UdonSharpBehaviour
{
    [Header("フェーズ管理")]
    [Tooltip("各フェーズの親オブジェクトを順番に登録 (0:Intro, 1:Strategy, 2:Battle, 3:Outro)")]
    public GameObject[] phaseRoots;

    [Tooltip("各フェーズのスタート地点 (Transform) を順番に登録")]
    public Transform[] spawnPoints;

    [Header("UI参照")]
    [Tooltip("メッセージウィンドウへの参照")]
    public MessageWindow messageWindow;

    private int currentPhaseIndex = 0; // 現在のフェーズ番号

    void Start()
    {
        // 念のため、開始時にフェーズ0を強制実行
        SetPhase(0);
    }

    // 次のフェーズに進む関数（UIのボタンなどから呼ぶ）
    public void GoToNextPhase()
    {
        if (currentPhaseIndex < phaseRoots.Length - 1)
        {
            SetPhase(currentPhaseIndex + 1);
        }
        else
        {
            Debug.Log("これ以上先のフェーズはありません（体験終了）");
        }
    }

    // 指定したフェーズに切り替える処理
    public void SetPhase(int nextIndex)
    {
        // 1. 全部のフェーズを一旦非表示にする
        foreach (GameObject root in phaseRoots)
        {
            if (root != null) root.SetActive(false);
        }

        // 2. 次のフェーズだけ表示する
        if (phaseRoots[nextIndex] != null)
        {
            phaseRoots[nextIndex].SetActive(true);
        }

        // 3. プレイヤーを移動させる
        VRCPlayerApi player = Networking.LocalPlayer;
        if (player != null && spawnPoints[nextIndex] != null)
        {
            player.TeleportTo(spawnPoints[nextIndex].position, spawnPoints[nextIndex].rotation);
        }

        // 4. フェーズごとにUIモードを切り替え
        if (messageWindow != null)
        {
            switch (nextIndex)
            {
                case 0: // Intro
                    messageWindow.SetMode(2); // World Fixed
                    // ============================================
                    // [テスト用] 本番では削除または適切なテキストに置換
                    // ============================================
                    messageWindow.ShowMessage("[テスト] Phase 0: Intro\nWorld Fixed モードのテスト表示です");
                    break;
                case 1: // Strategy
                    messageWindow.SetMode(0); // Always On
                    // ============================================
                    // [テスト用] 本番では削除または適切なテキストに置換
                    // ============================================
                    messageWindow.ShowMessage("[テスト] Phase 1: Strategy\nAlways On モードのテスト表示です");
                    break;
                case 2: // Battle
                    messageWindow.SetMode(0); // Always On
                    // ============================================
                    // [テスト用] 本番では削除または適切なテキストに置換
                    // ============================================
                    messageWindow.ShowMessage("[テスト] Phase 2: Battle\nAlways On モードのテスト表示です");
                    break;
                case 3: // Outro
                    messageWindow.SetMode(2); // World Fixed
                    // ============================================
                    // [テスト用] 本番では削除または適切なテキストに置換
                    // ============================================
                    messageWindow.ShowMessage("[テスト] Phase 3: Outro\nWorld Fixed モードのテスト表示です");
                    break;
            }
        }

        // インデックス更新
        currentPhaseIndex = nextIndex;
        Debug.Log($"フェーズ {nextIndex} に移行しました");
    }
}