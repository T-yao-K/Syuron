# 変更履歴 (CHANGELOG)

このドキュメントは、プロジェクトの主要な変更を記録します。

---

## [Unreleased]

### 追加予定
- GazeGuide: 注視誘導システム
- EventSequencer: 汎用イベントシーケンス制御
- EnemyController: 敵兵AI
- DialogueSystem: VN形式会話システム

---

## [2026-03-02] - ドキュメント最新化

### 変更
- **SystemDesign.md**: MessageTriggerの実装状態を「📝 設計中」から「✅ 実装済」に更新
- **SystemDesign.md**: 実装優先順位を最新化（GazeGuide/EnemyController/EventSequencerを追加）
- **TODO.md**: MessageTrigger.cs を完了済みに移動
- **CHANGELOG.md**: 1月以降の変更履歴を追記

### 追加
- **GazeGuide.cs**: 注視誘導システム
  - ハイライトエフェクト（パルスアニメーション付き）
  - 矢印インジケータ（視界外誘導）
  - 照準ガイド（ビルボード対応）
  - MessageWindow.ShowWithGaze() との連携

---

## [2026-01-25] - ドキュメント整理・機能拡張

### 追加
- **MessageTrigger.cs**: メッセージ外部化コンポーネント
  - インスペクターからメッセージ内容・モード・アンカーを設定可能
  - GazeGuide 連携オプション
  - 自動非表示タイマー

- **MessageWindow 機能拡張**:
  - `ShowWithGaze()`: GazeGuide 連携メソッド
  - `SetWorldFixedAnchor()`: アンカーインデックス切り替え
  - `SetWorldFixedAnchorDirect()`: アンカー直接指定
  - `worldFixedAnchors[]`: 複数アンカー対応

- **ドキュメント構造**:
  - `Docs/Design/`: 設計ドキュメント
  - `Docs/Components/`: コンポーネント設計
  - `Docs/Setup/`: セットアップ手順
  - `Docs/Testing/`: テストドキュメント

### 変更
- **Re_plan.md**: 3フェーズ → 4フェーズ構成に統一
- **SceneDesign.md**: スポーン地点名修正 (Spawn_Info → Spawn_Intro)
- **SystemDesign.md**: 実装状態を最新化、MessageTrigger・EventSequencer 追加
- **GazeGuide_BattleSequencer.md** → **EventSequencer.md** に改名

---

## [2026-01-18] - MessageWindow 実装

### 追加
- **MessageWindow.cs**: 視点追従UIシステム
  - Mode 0: 常時表示 (Always On)
  - Mode 1: ポップアップ (Pop-up)
  - Mode 2: 完全固定 (World Fixed)
  - VR/デスクトップ両対応
  - フェードアニメーション

- **ドキュメント**:
  - SystemDesign.md
  - SceneDesign.md
  - UI.md
  - MessageWindow_API.md
  - Setup_MessageWindow.md
  - GazeGuide_BattleSequencer.md
  - Re_plan.md

---

## [2026-01-XX] - 初期実装

### 追加
- **GameManager.cs**: フェーズ管理
- **NextButton.cs**: フェーズ遷移トリガー
- **WeaponController.cs**: 武器の発砲・リロード

---

## 凡例

- **追加**: 新機能
- **変更**: 既存機能の変更
- **非推奨**: 将来削除予定
- **削除**: 削除された機能
- **修正**: バグ修正
