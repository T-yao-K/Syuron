# GazeGuide セットアップ手順書

本ドキュメントでは、GazeGuide（注視誘導システム）を Unity シーン上でセットアップする手順を説明します。

---

## 前提条件

- Unity 2022.x
- VRChat SDK3 (Worlds) インストール済み
- UdonSharp インストール済み
- MessageWindow セットアップ済み（[Setup_MessageWindow.md](Setup_MessageWindow.md) 参照）

---

## 1. GazeGuide オブジェクトの作成

### 1.1. 空の GameObject を作成

1. Hierarchy で `[SYSTEM]` を右クリック → `Create Empty`
2. 名前を `GazeGuide` に変更
3. Transform をリセット（Position: 0, 0, 0）

```
[SYSTEM]
├── GameManager
├── MessageWindow
└── GazeGuide  ← ここに作成
```

### 1.2. スクリプトのアタッチ

1. GazeGuide オブジェクトを選択
2. Inspector → `Add Component` → `Udon Behaviour`
3. Program Source → `GazeGuide`

---

## 2. エフェクト子オブジェクトの作成

GazeGuide の子に3つのエフェクトオブジェクトを作成します。  
**全て初期状態で非アクティブ** にしてください（Inspector の名前横のチェックボックスを OFF）。

### 2.1. HighlightEffect （ハイライトエフェクト）

注視対象を光らせて目立たせるためのエフェクトです。

1. GazeGuide を右クリック → `Create Empty`
2. 名前を `HighlightEffect` に変更
3. **チェックボックスを OFF** にして非アクティブに
4. エフェクトの見た目を追加:

**方法A: Particle System を使用する場合（推奨）**

| 設定項目 | 推奨値 |
|----------|--------|
| Shape | Sphere |
| Start Color | 黄色〜オレンジ (255, 200, 50) |
| Start Size | 0.5 |
| Emission Rate | 20 |
| Renderer の Material | 発光系マテリアル（Additive） |

**方法B: 発光メッシュを使用する場合**

1. HighlightEffect の子に `Sphere` や `Quad` を追加
2. Emission 付きのマテリアルを適用
3. Scale を適切に調整

> **注意**: GazeGuide はこのオブジェクトの位置を毎フレーム対象に追従させます。HighlightEffect 自体のローカル位置は `(0, 0, 0)` のままにしてください。

### 2.2. ArrowIndicator （矢印インジケータ）

対象が視界外にある時に、画面端に表示される方向指示矢印です。

1. GazeGuide を右クリック → `Create Empty`
2. 名前を `ArrowIndicator` に変更
3. **チェックボックスを OFF** にして非アクティブに
4. 矢印の見た目を追加:

**推奨: 3D メッシュで矢印を作成**

1. ArrowIndicator の子に `3D Object` → 矢印型のメッシュ（Cone や作成した矢印モデル）を追加
2. Scale を `(0.1, 0.1, 0.2)` 程度に調整
3. マテリアル設定:

| 設定項目 | 推奨値 |
|----------|--------|
| Color | 白〜黄色 |
| Emission | ON（背景を問わず見えるように） |
| Rendering Mode | Fade または Transparent |

> **注意**: 矢印は `+Z (forward)` 方向が対象を指すように向きを設定してください。GazeGuide が `Quaternion.LookRotation` で対象方向に向けます。

### 2.3. AimingGuide （照準ガイド）

発砲を促す場面で敵の位置に表示される照準マーカーです。

1. GazeGuide を右クリック → `Create Empty`
2. 名前を `AimingGuide` に変更
3. **チェックボックスを OFF** にして非アクティブに
4. 照準の見た目を追加:

**推奨: Quad + 照準テクスチャ**

1. AimingGuide の子に `3D Object` → `Quad` を追加
2. 照準マーカーのテクスチャ（円形リング等）をマテリアルに設定
3. Scale を `(0.5, 0.5, 0.5)` 程度に調整

| 設定項目 | 推奨値 |
|----------|--------|
| Color | 赤〜オレンジ |
| Rendering Mode | Transparent |
| Emission | ON |

> **注意**: 照準ガイドはビルボード処理により常にプレイヤーの方を向きます。Quad の表面（`+Z` 方向）がプレイヤーに面するように配置してください。

### Hierarchy 完成図（GazeGuide部分）

```
[SYSTEM]
├── GameManager
├── MessageWindow
└── GazeGuide              ← UdonBehaviour (GazeGuide.cs)
    ├── HighlightEffect    ← 非アクティブ ☐
    ├── ArrowIndicator     ← 非アクティブ ☐
    └── AimingGuide        ← 非アクティブ ☐
```

---

## 3. インスペクターの設定

GazeGuide オブジェクトを選択し、Inspector で以下を設定します。

### 3.1. エフェクト参照

| プロパティ | 参照先 |
|-----------|--------|
| Highlight Effect | `HighlightEffect` オブジェクト |
| Arrow Indicator | `ArrowIndicator` オブジェクト |
| Aiming Guide | `AimingGuide` オブジェクト |

### 3.2. パルスアニメーション設定

| プロパティ | 推奨値 | 説明 |
|-----------|--------|------|
| Pulse Speed | 2.0 | パルスの速度（大きいほど速い） |
| Pulse Scale | 1.2 | 最大拡大率（1.0 = 変化なし） |

### 3.3. 視界判定設定

| プロパティ | 推奨値 | 説明 |
|-----------|--------|------|
| View Angle Threshold | 60.0 | 視界内とみなす角度（度）。VRでは狭め（50-60）、デスクトップでは広め（70-80）が推奨 |

### 3.4. 矢印インジケータ設定

| プロパティ | 推奨値 | 説明 |
|-----------|--------|------|
| Arrow Distance | 1.5 | 頭部からの前方距離（m） |
| Arrow Edge Offset | 0.6 | 画面端へのオフセット距離（m） |

---

## 4. MessageWindow との連携

GazeGuide を MessageWindow と連携させることで、`ShowWithGaze()` メソッドから注視誘導を開始できます。

### 4.1. MessageWindow の設定

1. `[SYSTEM]` → `MessageWindow` オブジェクトを選択
2. Inspector の `GazeGuide連携` セクションを探す
3. `Gaze Guide` フィールドに `GazeGuide` オブジェクトをドラッグ＆ドロップ

### 4.2. 連携の動作

```
MessageWindow.ShowWithGaze(text, target)
  ↓
gazeGuide.SetProgramVariable("target", target)  ← 対象を設定
gazeGuide.SendCustomEvent("StartGuide")         ← 誘導開始
  ↓
GazeGuide: ハイライト表示 + 矢印表示
```

---

## 5. EventSequencer との連携（将来実装）

EventSequencer からは以下のように GazeGuide を呼び出します。

```csharp
// EventSequencer.cs からの呼び出し例
gazeGuide.StartGuideWithTarget(enemyTransform);   // 敵をハイライト
gazeGuide.StartAimingGuide(enemyTransform);       // 照準ガイド表示
gazeGuide.StopGuide();                            // ハイライト停止
gazeGuide.StopAimingGuide();                      // 照準ガイド停止
gazeGuide.StopAll();                              // 全停止
```

EventSequencer の Inspector で `GazeGuide` フィールドを設定する必要があります。

---

## 6. 動作確認

### 6.1. VRChat Client Sim でテスト

1. メニュー → `VRChat SDK` → `Utilities` → `Open ClientSim`
2. Play Mode を開始
3. 以下を確認:

| # | テスト項目 | 確認方法 | 期待結果 |
|---|-----------|----------|----------|
| 1 | ハイライト表示 | `StartGuide` を呼び出す | 対象位置にハイライトが表示される |
| 2 | ハイライト追従 | 対象オブジェクトを移動する | ハイライトが追従する |
| 3 | パルスアニメ | ハイライト表示中に観察 | 拡大縮小がスムーズに繰り返される |
| 4 | 矢印表示（視界外） | 対象から視線を外す | 矢印が画面端に表示される |
| 5 | 矢印非表示（視界内） | 対象に視線を向ける | 矢印が消える |
| 6 | 照準ガイド | `StartAimingGuide` を呼び出す | 対象位置に照準マーカーが表示される |
| 7 | 照準ビルボード | プレイヤーが移動する | 照準が常にプレイヤーの方を向く |
| 8 | 停止 | `StopGuide` / `StopAll` を呼ぶ | 全エフェクトが消え、スケールがリセットされる |
| 9 | MessageWindow連携 | `ShowWithGaze` を呼び出す | メッセージ表示とハイライトが同時に動作する |

### 6.2. デバッグ方法

Console に以下のログが出力されます:

```
[GazeGuide] 注視誘導を開始: (対象名)
[GazeGuide] 注視誘導を停止
[GazeGuide] 照準ガイドを表示: (対象名)
[GazeGuide] 照準ガイドを停止
[GazeGuide] target が設定されていません     ← エラー時
```

---

## 7. トラブルシューティング

### Q: ハイライトが表示されない

- `highlightEffect` がインスペクターで正しくアサインされているか確認
- エフェクトオブジェクトにレンダラー（MeshRenderer, ParticleSystem等）があるか確認
- Console に `[GazeGuide] target が設定されていません` が出ていないか確認

### Q: 矢印が表示されない / 常に表示される

- `arrowIndicator` がインスペクターで正しくアサインされているか確認
- `viewAngleThreshold` の値が適切か確認（小さすぎると常に矢印が出る）
- 矢印のスケールが小さすぎないか確認

### Q: パルスアニメーションが動かない

- `pulseSpeed` が 0 になっていないか確認
- `pulseScale` が 1.0 になっていないか確認（1.0 だと変化量が 0）

### Q: 照準ガイドが裏返しに見える

- Quad の向きが `+Z (forward)` 方向になっているか確認
- マテリアルの Cull Mode を `Off` にすると両面表示で回避可能

### Q: エフェクトが GazeGuide の位置に一瞬表示される

- 子オブジェクトが**初期非アクティブ**になっているか確認（チェックボックス OFF）
- `Start()` で自動的に非アクティブ化されますが、エディタ上でも非アクティブにしておくのが安全

---

## 更新履歴

| 日付 | 内容 |
|------|------|
| 2026-03-02 | 初版作成 |
