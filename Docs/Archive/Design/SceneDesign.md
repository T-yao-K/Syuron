# シーン設計書: Syuron.unity

## 1. 概要

本ドキュメントでは、VRChat ワールド「Syuron」のシーン構成と設計方針を定義する。

---

## 2. シーン階層構造

```
Syuron (Scene Root)
│
├── [環境・システム基盤]
│   ├── WorldDescriptor
│   ├── Main Camera (Editor用、実行時はVRChatが上書き)
│   ├── Directional Light
│   ├── Fog
│   ├── EventSystem
│   └── VRCWorld
│
├── [SYSTEM] ─────────────────────── システム管理用
│   ├── GameManager                  フェーズ管理
│   ├── MessageWindow                視点追従UI
│   ├── BattleSequencer              戦闘シーケンス制御 (Phase 3用)
│   └── GazeGuide                    注視誘導システム (Phase 3用)
│
├── [CONTENT_ROOT] ───────────────── コンテンツ本体
│   ├── NextButton                   フェーズ遷移ボタン
│   │
│   ├── Phase0_Intro ─────────────── 導入フェーズ
│   │   ├── Environment              電脳空間の背景
│   │   ├── MessageAnchor            World Fixed UI用アンカー
│   │   └── Interactables            インタラクト可能オブジェクト
│   │
│   ├── Phase1_Strategy ──────────── 作戦室フェーズ
│   │   ├── Environment              作戦室の背景
│   │   ├── Jiorama                  戦場のジオラマ
│   │   ├── OmuraMasujiro            大村益次郎のアバター
│   │   └── Props                    小道具
│   │
│   ├── Phase2_Battle ────────────── 戦闘フェーズ
│   │   ├── Environment              戦場の背景（山道）
│   │   ├── Gun_Hinawa               火縄銃（参考用/敵が使用）
│   │   ├── Gun_Minie                ミニエー銃（プレイヤー用）
│   │   ├── Enemy                    敵兵
│   │   └── Effects                  エフェクト類
│   │
│   └── Phase3_Outro ─────────────── まとめフェーズ
│       ├── Environment              電脳空間の背景
│       └── MessageAnchor            World Fixed UI用アンカー
│
└── [SPAWN_POINTS] ───────────────── スポーン地点
    ├── Spawn_Intro                  Phase 0 開始位置
    ├── Spawn_Strategy               Phase 1 開始位置
    ├── Spawn_Battle                 Phase 2 開始位置
    └── Spawn_Outro                  Phase 3 開始位置
```

---

## 3. オブジェクト命名規則

### 3.1. ルールオブジェクト

| 種類 | 形式 | 例 |
|------|------|-----|
| システム管理 | `[NAME]` (大括弧) | [SYSTEM], [CONTENT_ROOT] |
| フェーズ | `Phase{N}_{Name}` | Phase0_Intro, Phase2_Battle |
| スポーン地点 | `Spawn_{Name}` | Spawn_Intro, Spawn_Battle |

### 3.2. コンテンツオブジェクト

| 種類 | 形式 | 例 |
|------|------|-----|
| 環境・背景 | `Environment` または具体名 | Environment, Jiorama |
| キャラクター | キャラクター名 | OmuraMasujiro, Enemy |
| 武器 | `Gun_{Type}` | Gun_Hinawa, Gun_Minie |
| UIアンカー | `{Purpose}Anchor` | MessageAnchor |
| エフェクト | `{Name}Effect` または専用フォルダ | Effects/MuzzleFlash |

---

## 4. レイヤー設計

| レイヤー | 用途 | 対象オブジェクト |
|---------|------|-----------------|
| Default | 一般オブジェクト | 環境、小道具 |
| UI | UIキャンバス | MessageWindow |
| Pickup | 持てるオブジェクト | 武器 |
| MirrorReflection | 鏡に映るもの | キャラクター |

---

## 5. フェーズ別アクティブ状態

GameManager が各フェーズの親オブジェクト (`phaseRoots`) を切り替える。

| フェーズ | Phase0 | Phase1 | Phase2 | Phase3 |
|---------|--------|--------|--------|--------|
| Phase0_Intro | ✅ Active | ❌ | ❌ | ❌ |
| Phase1_Strategy | ❌ | ✅ Active | ❌ | ❌ |
| Phase2_Battle | ❌ | ❌ | ✅ Active | ❌ |
| Phase3_Outro | ❌ | ❌ | ❌ | ✅ Active |
| [SYSTEM] | ✅ 常時 | ✅ 常時 | ✅ 常時 | ✅ 常時 |
| [SPAWN_POINTS] | ✅ 常時 | ✅ 常時 | ✅ 常時 | ✅ 常時 |

---

## 6. GameManager 設定

### 6.1. phaseRoots 配列

| Index | 参照先 |
|-------|--------|
| 0 | Phase0_Intro |
| 1 | Phase1_Strategy |
| 2 | Phase2_Battle |
| 3 | Phase3_Outro |

### 6.2. spawnPoints 配列

| Index | 参照先 |
|-------|--------|
| 0 | Spawn_Intro |
| 1 | Spawn_Strategy |
| 2 | Spawn_Battle |
| 3 | Spawn_Outro |

---

## 7. 光源設定

### 7.1. メインライト (Directional Light)

| 設定項目 | 推奨値 |
|----------|--------|
| Mode | Mixed または Baked |
| Intensity | 1.0 |
| Shadow Type | Soft Shadows |

### 7.2. フェーズ別ライティング

| フェーズ | ライティング方針 |
|---------|-----------------|
| Phase 0, 3 (電脳空間) | 環境光のみ、幻想的な青系 |
| Phase 1 (作戦室) | 暖色系、室内照明 |
| Phase 2 (戦場) | 自然光、やや暗め（緊張感） |

---

## 8. パフォーマンス考慮

### 8.1. オクルージョンカリング

- 各フェーズの Environment に Occluder Static を設定
- 見えないオブジェクトは描画をスキップ

### 8.2. LOD (Level of Detail)

- 遠距離のオブジェクトには LOD Group を設定
- 特に Phase 2 の戦場環境

### 8.3. ライトマップ

- 静的オブジェクトは Lightmap Static に設定
- リアルタイムライトは最小限に

---

## 9. VRChat 固有設定

### 9.1. VRC_World

| 設定項目 | 推奨値 |
|----------|--------|
| Respawn Height | -10 (落下時のリスポーン) |
| Object Behaviour At Respawn | Default |

### 9.2. スポーン地点

- 各スポーン地点の向き（Rotation Y）を調整
- プレイヤーが最初に見るべき方向を向かせる

---

## 10. 現在のシーン状態（スクリーンショット参照）

スクリーンショットから確認できる現在の構成:

```
Syuron
├── WorldDescriptor ✅
├── Main Camera ✅
├── Directional Light ✅
├── Fog ✅
├── EventSystem ✅
├── VRCWorld ✅
├── [SYSTEM]
│   ├── GameManager ✅
│   └── MessageWindow ✅
├── [CONTENT_ROOT]
│   ├── NextButton ✅
│   ├── Phase0_Intro ✅
│   ├── Phase1_Strategy ✅
│   ├── Phase2_Battle ✅
│   │   └── Gun_Hinawa ✅
│   ├── Mui.ne
│   ├── matsu
│   ├── DimensionDoor
│   ├── hako(1)Dim
│   └── Phase_LConro
└── [SPAWN_POINTS]
    ├── Spawn_Intro ✅
    ├── Spawn_Strategy ✅
    ├── Spawn_Battle ✅
    └── Spawn_Outro ✅
```

### 10.1. 追加が必要なもの

| オブジェクト | 配置場所 | 用途 | 状態 |
|-------------|---------|------|------|
| MessageAnchor | Phase0_Intro | World Fixed UI用 | ✗ 未追加 |
| MessageAnchor | Phase3_Outro | World Fixed UI用 | ✗ 未追加 |
| EventSequencer | [SYSTEM] | 汎用イベントシーケンス | 📝 設計中 |
| GazeGuide | [SYSTEM] | 注視誘導 | ✅ 実装済（シーン配置は未完了） |
| MessageTrigger | 各フェーズ | メッセージトリガー | ✅ 実装済（シーン配置は未完了） |

---

## 更新履歴

| 日付 | 内容 |
|------|------|
| 2026-01-18 | 初版作成 |
| 2026-03-02 | MessageTrigger の状態を実装済に更新 |
