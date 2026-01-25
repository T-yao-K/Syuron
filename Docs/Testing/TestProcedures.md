# 動作確認手順書

VRChat Client Sim および Unity エディタでの動作確認手順。

---

## 1. テスト環境の準備

### 1.1. 必要なもの
- Unity 2022.x
- VRChat SDK3 (Worlds)
- UdonSharp
- VRChat Client Sim

### 1.2. Client Sim の起動

1. Unity メニュー → **VRChat SDK** → **Utilities** → **Open ClientSim**
2. **Play Mode** を開始

---

## 2. MessageWindow テスト

### 2.1. Mode 0: 常時表示 (Always On)

| # | 手順 | 期待結果 |
|---|------|---------|
| 1 | Play Mode を開始 | MessageWindow が非表示状態で起動 |
| 2 | `GameManager.GoToNextPhase()` を呼び出し (Phase 1へ) | メッセージが表示される |
| 3 | 頭を左右に動かす | UI が遅延して追従する |
| 4 | 頭を上下に動かす | UI が視界下部に維持される |

### 2.2. Mode 2: 完全固定 (World Fixed)

| # | 手順 | 期待結果 |
|---|------|---------|
| 1 | Phase 0 (Intro) に移動 | メッセージがアンカー位置に固定表示 |
| 2 | プレイヤーが移動する | UI の位置は変わらない |
| 3 | 別のアンカーに切り替え | UI が新しいアンカー位置に移動 |

### 2.3. ShowWithGaze テスト

| # | 手順 | 期待結果 |
|---|------|---------|
| 1 | GazeGuide が設定されていることを確認 | Inspector で参照が設定されている |
| 2 | `messageWindow.ShowWithGaze("テスト", target)` を呼び出し | メッセージが表示され、GazeGuide が起動 |
| 3 | GazeGuide の対象がハイライトされる | 対象オブジェクトが光る |

---

## 3. MessageTrigger テスト

### 3.1. 基本動作

| # | 手順 | 期待結果 |
|---|------|---------|
| 1 | テスト用オブジェクトに MessageTrigger をアタッチ | Inspector が表示される |
| 2 | `message` フィールドにテキストを入力 | - |
| 3 | `messageWindow` 参照を設定 | - |
| 4 | Play Mode でオブジェクトをインタラクト | メッセージが表示される |

### 3.2. モード別テスト

| # | displayMode | 期待結果 |
|---|-------------|---------|
| 1 | 0 (Always On) | 視点追従でメッセージ表示 |
| 2 | 1 (Pop-up) | 一定時間後に自動非表示 |
| 3 | 2 (World Fixed) + anchor設定 | アンカー位置に固定表示 |

### 3.3. GazeGuide 連携

| # | 手順 | 期待結果 |
|---|------|---------|
| 1 | `useGazeGuide` を ON | - |
| 2 | `gazeTarget` にターゲットを設定 | - |
| 3 | オブジェクトをインタラクト | メッセージ表示 + ターゲットがハイライト |

---

## 4. フェーズ遷移テスト

| # | 手順 | 期待結果 |
|---|------|---------|
| 1 | Phase 0 (Intro) でスタート | 電脳空間で固定メッセージ表示 |
| 2 | NextButton をインタラクト | Phase 1 に遷移、メッセージがMode 0に |
| 3 | 再度 NextButton をインタラクト | Phase 2 に遷移 |
| 4 | 再度 NextButton をインタラクト | Phase 3 に遷移、固定メッセージ表示 |

---

## 5. デバッグ方法

### 5.1. Console ログ

以下のログが出力されることを確認:

```
[MessageWindow] メッセージ表示: (テキスト)
[MessageWindow] ウィンドウ非表示
[MessageWindow] モード変更: (数値)
[MessageWindow] アンカー切り替え: (インデックス)
[MessageTrigger] メッセージ表示: (テキストの先頭30文字)
```

### 5.2. よくある問題

| 症状 | 原因 | 対処 |
|-----|------|-----|
| UI が表示されない | `ShowMessage()` が呼ばれていない | Console でログを確認 |
| UI が裏返し | ビルボード処理の問題 | 180度回転が適用されているか確認 |
| Mode 2 で位置が変わらない | `worldFixedAnchors` が空 | Inspector でアンカーを設定 |
| GazeGuide が動かない | 参照が未設定 | `gazeGuide` フィールドを確認 |

---

## 6. パフォーマンス確認

| 項目 | 目標値 |
|-----|-------|
| FPS | 60 以上 |
| UI 追従のラグ | 体感不自然でない |
| VR酔い | 5分間使用で発生しない |

---

## 更新履歴

| 日付 | 内容 |
|------|------|
| 2026-01-25 | 初版作成 |
