# TODO リスト

未実装機能と今後の作業項目。

---

## 🔴 高優先度

### コンポーネント実装

- [x] **GazeGuide.cs** - 注視誘導システム
  - ハイライトエフェクト
  - 矢印インジケータ（視界外誘導）
  - 照準ガイド
  - パルスアニメーション

- [ ] **EventSequencer.cs** - 汎用イベントシーケンス制御
  - 複数ステップの順序制御
  - 各ステップでのメッセージ・GazeGuide 連携
  - Phase 3 (Battle) のサブフェーズ制御

### シーンセットアップ

- [ ] **MessageAnchor** を Phase0_Intro と Phase3_Outro に追加
- [ ] **MessageWindow** をインスペクターで設定確認
- [ ] **worldFixedAnchors[]** に Phase 0, 3 のアンカーを設定

---

## 🟡 中優先度

### コンポーネント実装

- [ ] **EnemyController.cs** - 敵兵AI
  - 発砲アニメーション
  - 被弾処理（TakeDamage）
  - BattleSequencer との連携

- [ ] **DialogueSystem.cs** - VN形式会話システム
  - Phase 2 (Strategy) での大村益次郎との会話

### エフェクト

- [ ] **MuzzleFlash** - 発砲エフェクト
- [ ] **HitEffect** - 命中エフェクト
- [ ] **弾道表示** - 弾の軌道visualizer

---

## 🟢 低優先度

### コンテンツ

- [ ] ジオラマシーンの構築
- [ ] 大村益次郎のアバター導入
- [ ] 電脳空間の背景デザイン
- [ ] 戦場（山道）の3Dモデリング

### サウンド

- [ ] 発砲音
- [ ] 環境音
- [ ] BGM

### UI/UX

- [ ] チュートリアルUI
- [ ] 進捗表示UI

---

## ✅ 完了

- [x] GameManager.cs - フェーズ管理
- [x] NextButton.cs - フェーズ遷移トリガー
- [x] WeaponController.cs - 武器システム
- [x] MessageWindow.cs - 視点追従UI
- [x] MessageTrigger.cs - メッセージ外部化トリガー
- [x] ドキュメント構造整理
- [x] 4フェーズ構成への統一

---

## 更新履歴

| 日付 | 内容 |
|------|------|
| 2026-01-25 | 初版作成 |
| 2026-03-02 | MessageTrigger.cs を完了に移動、ドキュメント最新化 |
| 2026-03-02 | GazeGuide.cs 実装完了 |
