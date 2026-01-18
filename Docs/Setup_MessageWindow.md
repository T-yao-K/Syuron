# MessageWindow セットアップ手順書

本ドキュメントでは、MessageWindow システムを Unity シーン上でセットアップする手順を説明します。

---

## 前提条件

- Unity 2022.x
- VRChat SDK3 (Worlds) インストール済み
- UdonSharp インストール済み
- TextMeshPro インストール済み

---

## 1. Prefab の作成

### 1.1. 空の GameObject を作成

1. Hierarchy で右クリック → `Create Empty`
2. 名前を `MessageWindow` に変更
3. Transform をリセット（Position: 0, 0, 0）

### 1.2. Canvas の追加（子オブジェクト）

1. MessageWindow を右クリック → `UI` → `Canvas`
2. Canvas の設定:

| 設定項目 | 値 |
|----------|-----|
| Render Mode | **World Space** |
| Event Camera | None（VRChatが自動設定） |
| Sorting Order | 100 |

3. Rect Transform の設定:

| 設定項目 | 値 |
|----------|-----|
| Width | 800 |
| Height | 200 |
| Scale | (0.001, 0.001, 0.001) |

### 1.3. Canvas Group の追加

1. Canvas を選択
2. Inspector → `Add Component` → `Canvas Group`

### 1.4. BackgroundPanel の追加

1. Canvas を右クリック → `UI` → `Image`
2. 名前を `BackgroundPanel` に変更
3. Image の設定:

| 設定項目 | 値 |
|----------|-----|
| Color | (0, 0, 0, 0.7) 半透明黒 |
| Raycast Target | OFF |

4. Rect Transform の設定:

| 設定項目 | 値 |
|----------|-----|
| Anchor | Stretch - Stretch |
| Left/Right/Top/Bottom | 0 |

### 1.5. MessageText の追加

1. BackgroundPanel を右クリック → `UI` → `Text - TextMeshPro`
2. 名前を `MessageText` に変更
3. TextMeshPro の設定:

| 設定項目 | 値 |
|----------|-----|
| Font Size | 24 |
| Alignment | Center / Middle |
| Color | White |
| Overflow | Ellipsis |

4. Rect Transform の設定:

| 設定項目 | 値 |
|----------|-----|
| Anchor | Stretch - Stretch |
| Left/Right/Top/Bottom | 20（パディング） |

---

## 2. スクリプトのアタッチ

### 2.1. MessageWindow.cs をアタッチ

1. MessageWindow オブジェクトを選択
2. Inspector → `Add Component` → `Udon Behaviour`
3. Program Source → `MessageWindow`

### 2.2. 参照の設定

Inspector で以下を設定:

| プロパティ | 参照先 |
|-----------|--------|
| Background Panel | BackgroundPanel オブジェクト |
| Message Text | MessageText (TextMeshPro) |
| Canvas Group | Canvas の CanvasGroup コンポーネント |

---

## 3. シーンへの配置

### 3.1. 推奨配置場所

```
Syuron (Scene Root)
├── [SYSTEM]
│   ├── GameManager
│   └── MessageWindow  ← ここに配置
├── [CONTENT_ROOT]
│   └── ...
└── [SPAWN_POINTS]
    └── ...
```

### 3.2. 配置手順

1. MessageWindow を `[SYSTEM]` の子オブジェクトに移動
2. Transform は (0, 0, 0) のまま（動的に位置が決まるため）

---

## 4. GameManager との連携

### 4.1. GameManager に参照を追加

1. `GameManager.cs` を編集して以下を追加:

```csharp
public class GameManager : UdonSharpBehaviour
{
    // 既存のプロパティ...

    [Header("UI参照")]
    public MessageWindow messageWindow;  // ← 追加

    public void SetPhase(int nextIndex)
    {
        // 既存の処理...

        // フェーズごとにUIモードを切り替え
        if (messageWindow != null)
        {
            switch (nextIndex)
            {
                case 0: // Intro
                    messageWindow.SetMode(2); // World Fixed
                    break;
                case 1: // Strategy
                    messageWindow.SetMode(0); // Always On
                    break;
                case 2: // Battle
                    messageWindow.SetMode(0); // Always On
                    break;
                case 3: // Outro
                    messageWindow.SetMode(2); // World Fixed
                    break;
            }
        }
    }
}
```

### 4.2. インスペクターで参照を設定

1. GameManager オブジェクトを選択
2. `Message Window` フィールドに MessageWindow をドラッグ＆ドロップ

---

## 5. モード別設定

### 5.1. Mode 0: 常時表示 (Always On)

| プロパティ | 推奨値 | 説明 |
|-----------|--------|------|
| Display Mode | 0 | |
| Distance | 1.5 | カメラからの距離 (m) |
| Follow Speed | 5.0 | 追従の滑らかさ |
| View Offset | (0, -0.3, 0) | 視界下部に配置 |

### 5.2. Mode 1: ポップアップ (Pop-up)

| プロパティ | 推奨値 | 説明 |
|-----------|--------|------|
| Display Mode | 1 | |
| Popup Duration | 5.0 | 表示時間 (秒) |

### 5.3. Mode 2: 完全固定 (World Fixed)

| プロパティ | 推奨値 | 説明 |
|-----------|--------|------|
| Display Mode | 2 | |
| World Fixed Anchor | (Transform) | 固定位置のTransform |

**World Fixed Anchor の設定:**

1. 固定表示したい場所に空の GameObject を作成
2. 名前を `MessageAnchor_Phase1` などに
3. MessageWindow の `World Fixed Anchor` に設定

---

## 6. フェーズ別アンカー設定

Phase 1 と Phase 4 では World Fixed モードを使用するため、アンカーが必要です。

### 6.1. アンカーの作成

```
[CONTENT_ROOT]
├── Phase0_Intro
│   └── MessageAnchor  ← 作成
├── Phase1_Strategy
├── Phase2_Battle
└── Phase3_Outro
    └── MessageAnchor  ← 作成
```

### 6.2. アンカーの配置

| フェーズ | 推奨位置 |
|---------|---------|
| Phase 0 (Intro) | プレイヤーの正面、目の高さ、2m先 |
| Phase 3 (Outro) | プレイヤーの正面、目の高さ、2m先 |

---

## 7. 動作確認

### 7.1. VRChat Client Sim でテスト

1. メニュー → `VRChat SDK` → `Utilities` → `Open ClientSim`
2. Play Mode を開始
3. 以下を確認:
   - [ ] Mode 0: 頭を動かすとUIが遅延追従するか
   - [ ] Mode 1: 一定時間後に消えるか
   - [ ] Mode 2: アンカー位置に固定されるか
   - [ ] フェード: 表示/非表示が滑らかか

### 7.2. デバッグ方法

Console に以下のログが出力されます:
```
[MessageWindow] メッセージ表示: (テキスト)
[MessageWindow] ウィンドウ非表示
[MessageWindow] モード変更: (モード番号)
```

---

## 8. トラブルシューティング

### Q: UIが表示されない
- Canvas Group の Alpha が 0 になっていないか確認
- gameObject.SetActive の状態を確認

### Q: UIが裏返しになる
- LookAt 後の 180度回転が正しく行われているか確認

### Q: 追従がガタつく
- `Follow Speed` を下げる（推奨: 5.0）
- `LateUpdate` で処理されているか確認

### Q: World Fixed モードで表示されない
- `World Fixed Anchor` が設定されているか確認
- アンカーの Transform が正しいか確認

---

## Hierarchy 完成図

```
Syuron
├── WorldDescriptor
├── Main Camera
├── Directional Light
├── Fog
├── EventSystem
├── VRCWorld
├── [SYSTEM]
│   ├── GameManager
│   │   └── (MessageWindow への参照を設定)
│   └── MessageWindow  ← 新規追加
│       └── Canvas (World Space)
│           ├── CanvasGroup
│           └── BackgroundPanel
│               └── MessageText (TMP)
├── [CONTENT_ROOT]
│   ├── NextButton
│   ├── Phase0_Intro
│   │   └── MessageAnchor  ← 新規追加
│   ├── Phase1_Strategy
│   ├── Phase2_Battle
│   └── Phase3_Outro
│       └── MessageAnchor  ← 新規追加
└── [SPAWN_POINTS]
    └── ...
```

---

## 更新履歴

| 日付 | 内容 |
|------|------|
| 2026-01-18 | 初版作成 |
