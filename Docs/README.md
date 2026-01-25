# Syuron ドキュメントガイド

VRChat教育ワールド「Syuron」のドキュメント一覧と参照ガイド。

---

## 📁 ドキュメント構成

```
Docs/
├── 📄 README.md         ← このファイル
├── 📄 CHANGELOG.md      変更履歴
├── 📄 TODO.md           未実装機能リスト
│
├── 📁 Design/           設計ドキュメント
│   ├── SystemDesign.md  全体システム設計書
│   ├── SceneDesign.md   シーン構成設計書
│   └── Re_plan.md       企画書・方針
│
├── 📁 Components/       コンポーネント設計
│   ├── UI.md            MessageWindow設計書
│   ├── MessageWindow_API.md  API リファレンス
│   ├── EventSequencer.md     イベントシーケンス設計
│   └── MessageTrigger.md     メッセージトリガー設計
│
├── 📁 Setup/            セットアップ手順
│   └── Setup_MessageWindow.md
│
└── 📁 Testing/          テストドキュメント
    └── TestProcedures.md  動作確認手順書
```

---

## 🚀 クイックスタート

### 初めてプロジェクトに参加する場合

1. **[Re_plan.md](Design/Re_plan.md)** - プロジェクトの目的と方針を理解
2. **[SystemDesign.md](Design/SystemDesign.md)** - システム全体像を把握
3. **[SceneDesign.md](Design/SceneDesign.md)** - Unityシーンの構成を確認

### 特定の機能を実装する場合

| 実装したい機能 | 参照すべきドキュメント |
|--------------|---------------------|
| メッセージ表示 | [UI.md](Components/UI.md), [MessageWindow_API.md](Components/MessageWindow_API.md) |
| メッセージトリガー設置 | [MessageTrigger.md](Components/MessageTrigger.md) |
| イベントシーケンス | [EventSequencer.md](Components/EventSequencer.md) |
| シーンセットアップ | [Setup_MessageWindow.md](Setup/Setup_MessageWindow.md) |

### テスト・検証する場合

- **[TestProcedures.md](Testing/TestProcedures.md)** - 動作確認手順

---

## 📋 ドキュメント詳細

### Design/ - 設計ドキュメント

| ファイル | 内容 | 更新頻度 |
|---------|------|---------|
| **SystemDesign.md** | フェーズ設計、コンポーネント一覧、実装状態 | 機能追加時 |
| **SceneDesign.md** | シーン階層、命名規則、配置ガイドライン | シーン変更時 |
| **Re_plan.md** | プロジェクトの方針、スケジュール | 大きな方針変更時 |

### Components/ - コンポーネント設計

| ファイル | 内容 | 対象スクリプト |
|---------|------|--------------|
| **UI.md** | MessageWindow の詳細設計 | MessageWindow.cs |
| **MessageWindow_API.md** | メソッド・プロパティのリファレンス | MessageWindow.cs |
| **EventSequencer.md** | 汎用イベントシーケンス設計 | EventSequencer.cs (設計中) |
| **MessageTrigger.md** | メッセージ外部化トリガー設計 | MessageTrigger.cs |

### Setup/ - セットアップ手順

| ファイル | 内容 |
|---------|------|
| **Setup_MessageWindow.md** | MessageWindow の Unity セットアップ手順 |

### Testing/ - テストドキュメント

| ファイル | 内容 |
|---------|------|
| **TestProcedures.md** | VRChat Client Sim での動作確認手順 |

---

## 🔄 ドキュメント更新のルール

1. **コード変更時**: 関連するドキュメントも一緒に更新する
2. **新機能追加時**: CHANGELOG.md に記録する
3. **設計変更時**: SystemDesign.md の実装状態テーブルを更新する

---

## 更新履歴

| 日付 | 内容 |
|------|------|
| 2026-01-25 | 初版作成、ディレクトリ構造整理 |
