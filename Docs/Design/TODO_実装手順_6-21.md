# 今日の実装手順 — 6/21（小物・人物・鏡・文字・昼夜・TTS）

> **対象日**：2026-06-21  
> **正本仕様**：`VRChatワールド実装設計書_v8.md` / `ウィンドウ文言_5obj版.md` / `銀座実装セットアップ.md`  
> **シーン**：`Assets/Scenes/Syuron.unity`  
> **進行コード**：`BeatSequencer`（音声連携は**未実装** — §7 参照）

---

## 全体の進め方（推奨順）

依存関係を踏まえた作業順。並行できるものは `(並)` と記す。

```
1. 小物導入（§1）          … 2〜3h   ← 先に置くと位置調整しやすい
2. 人物モデル導入（§2）    … 1〜2h   ← 鏡用モデルと NPC を同時調達可 (並)
3. 鏡ギミック（§3）        … 3〜5h   ← §2 の鏡用モデルが必要
4. 文字サイズ確認（§4）    … 0.5〜1h ← 小物配置後がよい (並)
5. アバター要否判断（§5）  … 15分    ← §3 方針確定後
6. 昼夜切り替え（§6）      … 1〜2h   ← obj5 ガス灯とセット
7. 音声読み上げ（§7）      … 2〜4h   ← コード追加が必要。最後でも可
```

**今日の最低ライン（MVP）**：§1 小物＋§3 鏡（暗転まで）＋§4 文字が読める＋§6 obj5 だけ点灯  
**余力**：NPC（#4 ざんぎり頭）・TTS・腰に手の演出

---

## 1. 小物導入

### 目的

5 オブジェクト＋OP 背景が「グレーボックス」から一段上がり、FactList の事実が**視覚的にも**辿れる状態にする。  
テスト主軸語（廃刀令・安愚楽鍋・横浜毎日新聞・本木昌造）は**窓テキストで担保**済み。小物はフレーバー＋導線の補強。

### 既にあるアセット（リポジトリ内）

| 用途 | パス | 備考 |
|---|---|---|
| 街並み prefab | `Assets/Syuron/Third-party/Taisho_Modern/Prefabs/` | boxA〜E, lantan, chochin 等。(C)BellaPasta |
| ガス灯 3D | `Assets/Syuron/Third-party/gastou/` | obj5 用 |
| 鉄道掲示テクスチャ | `Assets/Syuron/Images/tetudou.jpg` | obj2 |
| 新聞画像 | `Assets/Syuron/Images/Shimbun.jpg`, `shimbun.png` | obj4 |
| 暦掲示 | `Assets/Syuron/Images/koyomi.jpg` | obj5 |

### シーン上の配置先（Hierarchy）

`Interactables` 配下（既存）：

| オブジェクト | Hierarchy 名 | 置くもの |
|---|---|---|
| obj1 | `obj1_NabeNoren` | 暖簾（Plane＋「牛鍋」テキスト or chochin 流用）。`BeatInteract`＋`GlowHighlight` 済み想定 |
| obj2 | `obj2_YouthRailPoster` | 掲示板 Quad ＋ `tetudou.jpg`。若者 NPC は §2 |
| obj3 | `obj3_Mirror` | 鏡枠＋硝子 Quad（§3） |
| obj4 | `obj4_NewsUkiyoe` | 新聞 `Shimbun.jpg` ＋錦絵風 Plane（1:1 複製禁止＝雰囲気のみ） |
| obj5 | `obj5_GasLampCalendar` | `gastou` prefab ＋ `koyomi.jpg` 掲示 |

背景：`gasutou` 配下にガス灯インスタンス複数、`Taisho_Modern` の box 系で通りを囲む（探索範囲は狭いまま）。

### 手順（Unity）

1. **シーンを開く** → `Syuron.unity`
2. **各 obj 親の子にメッシュを追加**
   - 3D：`GameObject > 3D Object > Quad` または Third-party prefab をドラッグ
   - テクスチャ：`Assets/Syuron/Images/` を Quad の Material に割り当て
3. **Collider を付ける**（Interact 必須）
   - `Box Collider` を Interact 対象のメッシュに。`BeatInteract` と同じ GameObject か、親に付与
4. **発光対象を指定**
   - 各 obj の `GlowHighlight.targetRenderer` に、プレイヤーが見る Renderer を割り当て
5. **史実ガード**
   - 鉄道**車両・馬車**（1882）を出さない（掲示の「話題」のみ）
   - 錦絵は既存画像の雰囲気利用。有名作品の 1:1 複製はしない
6. **ClientSim で desktop 再生** → OP から obj1 まで通し、発光→Interact→窓表示を確認

### 受け入れ基準

- [ ] 5 obj すべてに「それと分かる」見た目がある（最低限 Quad＋画像でも可）
- [ ] Interact → 窓ページ追加 → 発光が次 obj に移る（既存 `BeatSequencer` フロー）
- [ ] 探索範囲が極端に広がっていない

---

## 2. 人物モデルの導入

### 目的（2 種類を混同しない）

| 種類 | 用途 | VRChat アバター化 | 優先度 |
|---|---|---|---|
| **A. 鏡用ポーズ固定モデル** | obj3 RenderTexture に映す「腰から下」 | **不要**（FBX で可） | **必須** |
| **B. 環境 NPC** | OP の #4 ざんぎり頭・obj2 の若者・雑踏 | 不要（静止メッシュで可） | 推奨 |

鏡は頭を暗転するため、**#4 ざんぎり頭は NPC（環境）側**で見せる（設計書 v8 §4）。

### A. 鏡用モデル — 要件

- **見える範囲**：腰から下（袴・刀のない腰・革靴）
- **見えなくてよい**：顔・頭・上半身の作り込み
- **刀**：腰に帯刀なし（廃刀令）
- **足元**：革靴（#6 洋服の手がかり）

#### 調達候補

1. **VRoid Hub / Booth** で「和装」「袴」検索 → FBX エクスポート（下半身だけ使う）
2. **無料和装アバター** をダウンロード → Unity に FBX インポート（Humanoid 不要）
3. **自作**：VRoid Studio で地味な着物＋袴、靴だけ洋風

#### Unity 配置

1. FBX を `Assets/Syuron/Models/` 等にインポート
2. 空オブジェクト `MirrorReflectionModel` を **鏡の向こう**（§3）に配置
3. レイヤーを **`MirrorOnly`** に変更（§3 で作成）
4. ポーズ：T-pose / 立ち。腰に手をやるポーズは Animator または Blender で後付け（余力）
5. **メインカメラからは見えない位置**（壁裏・専用部屋）に置く

### B. 環境 NPC — 要件

| NPC | 配置 | 最低限 |
|---|---|---|
| ざんぎり頭通行人（#4） | 通り上、`NPCs` 配下 | 静止 1 体。短髪＋洋装 or 和洋折衷 |
| ハイカラ若者（obj2） | obj2 近く | 高い襟の洋装。走るアニメは余力 |
| 雑踏 | 通り | 同 prefab を 2〜3 体並べるだけでも可 |

#### 手順

1. `Hierarchy > NPCs`（空）配下にモデルを配置
2. **Collider / Interact は付けない**（背景のみ。操作は obj2 の掲示板）
3. 短髪が分かる角度に向ける（OP 字幕「皆ざんぎり頭」と一致）

### 受け入れ基準

- [ ] 鏡用モデルの腰〜足が「元武士っぽい」（袴＋革靴＋刀なし）
- [ ] OP 付近から NPC の短髪が視認できる（#4）
- [ ] NPC に Interact が付いていない（操作対象は 5 obj のみ）

---

## 3. 鏡ギミックの実装

### 目的

obj3 の中核演出：**下半分のみ可視・上半分暗転**。desktop/VR で**同一の像**（RenderTexture ＋ 固定モデル推奨）。

> 詳細手順の正本：`銀座実装セットアップ.md` §B  
> ⚠️ **VRC Mirror（標準鏡）は使わない** — プレイヤー自身のアバターを映すため、研究のパリティ要件と合わない。

### 手順

#### Step 1 — レイヤー

1. `Edit > Project Settings > Tags and Layers` → User Layer に `MirrorOnly` を追加
2. §2 の `MirrorReflectionModel` を `MirrorOnly` レイヤーに

#### Step 2 — RenderTexture

1. `Assets/Syuron/` 右クリック → `Create > Render Texture`
2. 名前：`MirrorRT`、Size **512 または 1024**
3. PC ワールド前提。Quest 最適化は対象外

#### Step 3 — Mirror Camera

1. `obj3_Mirror` の子に `Camera` を作成（名前：`MirrorCamera`）
2. 設定：
   - **Target Texture** ← `MirrorRT`
   - **Culling Mask** ← `MirrorOnly` のみ
   - **Clear Flags** ← Solid Color（暗色）
   - 位置・回転：固定モデルの下半身がフレームに入るよう調整
3. 左右反転：映像が逆なら Camera の Scale X = -1、または Quad の UV を反転

#### Step 4 — 鏡面 Quad

1. `obj3_Mirror` に Quad（名前：`MirrorSurface`）
2. Material：`Unlit/Texture`（Built-in）または URP Unlit
3. Main Texture ← `MirrorRT`
4. プレイヤーが立つ位置から「硝子」として自然な高さ・角度に

#### Step 5 — 上半分暗転（推奨：方法 A）

1. `MirrorSurface` の子に Quad `MirrorMaskTop` を追加
2. 上半分を覆うようスケール・位置調整
3. Material：黒・不透明（Unlit Color `#000000`）
4. 顔・頭が一切見えないことを Scene ビューと Game ビューで確認

#### Step 6 — カメラの ON/OFF（余力）

- 常時 ON でも MVP は成立
- 最適化：`MirrorCamera` を初期 **Disabled**
- obj3 Interact 時だけ Enable → Udon で `BeatInteract` から `SendCustomEvent`（パイロット前でも可）

#### Step 7 — パリティ確認

1. **ClientSim（desktop）** で obj3 まで進み、下半身像を確認
2. **VR 実機**（余力）で同一像か確認
3. RenderTexture 方式なら原理上 desktop/VR 同一

### 受け入れ基準

- [ ] 上半分が暗転し、**顔・頭・髷が見えない**
- [ ] 下半分に袴・刀のない腰・革靴が映る
- [ ] desktop ClientSim で確認済み
- [ ] `BeatInteract` → obj3 ページ（廃刀令・革靴）が表示される

---

## 4. 文字の大きさの確認

### 目的

可読性研究指標に沿い、**視野角 0.84°（約 50 分角）** を目標、**最低 0.32°（20 分角）** を下回らないこと（設計書 v8 §5.1）。

### 現状（MessageWindow prefab）

| 項目 | 値 |
|---|---|
| Canvas SizeDelta | 800 × 200（ワールド単位 mm 相当） |
| Canvas localScale | 0.001 → 実サイズ **約 0.8m × 0.2m** |
| TMP fontSize | 24 |
| desktop 距離 | `desktopDistance = 1.2` m |
| VR 距離 | `vrDistance = 1.5` m |

### 視野角の測り方

**方法 A — 実測（推奨）**

1. ClientSim または HMD で体験中、窓の **1 文字の高さ** \(h\)（メートル）を目視または Scene で測る  
2. 視点から窓までの距離 \(d\)（`MessageWindow` の `desktopDistance` / `vrDistance`）  
3. 視野角（度）≈ \(2 \times \arctan\bigl(\dfrac{h/2}{d}\bigr) \times \dfrac{180}{\pi}\)  
   - 近似：\(\text{度} \approx \dfrac{h}{d} \times 57.3\)

**方法 B — 調整手順**

1. **第一調整**：`MessageWindow` の `messageText`（TMP）の **Font Size**
2. **第二調整**：Canvas の **Scale**（0.001 を微調整）
3. **第三調整**：`desktopDistance` / `vrDistance`（近すぎ＜0.5m は避ける）
4. desktop と VR で**同じ視野角**になるよう両方の距離を揃える

### チェックリスト

- [ ] ゴシック体（明朝不使用）
- [ ] 暗パネル不透明度 ≥ 50%（prefab Background α ≈ 0.76）
- [ ] 1 ページ 2〜4 行（`ウィンドウ文言_5obj版.md` 準拠）
- [ ] テスト主軸語（**廃刀令・安愚楽鍋・横浜毎日新聞・本木昌造**）が読める
- [ ] 賑やかな背景（街並み）の上でも窓文字が読める
- [ ] desktop **と** VR の両方で確認（VR は実機 or ClientSim VR モード）

### 目安

| 判定 | 視野角 |
|---|---|
| 不可 | &lt; 0.32° |
| 目標 | **0.84° 付近** |
| 常用上限 | &gt; 0.95° にしない |

---

## 5. アバター用意？

### 結論（先に読む）

| 誰のアバター？ | 要否 | 理由 |
|---|---|---|
| **被験者（プレイヤー）** | **不要** | 鏡は RenderTexture＋固定モデル。実アバター反射はパリティリスク |
| **鏡に映すモデル** | **要**（§2-A） | FBX で可。VRChat 公開・アップロード不要 |
| **NPC** | **推奨** | 静止メッシュで可。アバター化不要 |
| **実験者用テストアカウント** | **不要**（RenderTexture 採用時） | 実アバター反射方式を取る場合のみ和装を事前読込 |

### 判断フロー

```
鏡を RenderTexture + 固定モデルで実装する？
  YES → プレイヤーアバター調達・統制は不要（推奨）
  NO  → 全被験者に同一和装アバターを事前読込させる運用（非推奨・工数増）
```

---

## 6. 昼夜切り替えの演出

### 目的（5obj 構成の時間軸）

| タイミング | 時間帯 | 演出 |
|---|---|---|
| OP | **昼** | 明るい空・ガス灯は消灯 |
| obj1〜4 | 昼〜夕方 | 大きく変えなくてよい |
| obj5 | **夕暮れ→夜** | ガス灯に**順次点灯**（#3 再提示） |
| ED | **夜** | 暗転 →「体験終了」 |

### 実装オプション（簡単な順）

#### オプション A — ガス灯 Emission のみ（MVP・推奨）

1. `gasutou` の Material に Emission を有効化
2. 初期：Emission Intensity = 0
3. obj5 Interact 時（または BeatSequencer が obj5 ビートに入った時）に Udon で順次点灯  
   - `SendCustomEventDelayedSeconds` で 0.5s 間隔など
4. Directional Light の Intensity を 0.3 程度まで下げて「夜」を表現

#### オプション B — Aether スカイ（リポジトリ内）

- パス：`Assets/Syuron/Third-party/ACM/Shader/Aether/`
- `TimeController.prefab` ＋ `AetherTime.cs` で太陽高度をアニメ
- 学習コスト高め。**今日は A 優先**

#### オプション C — スカイボックス差し替え

- 昼用 / 夜用 Material を 2 枚用意
- obj5 到達時に `RenderSettings.skybox` を差し替え（Udon から可能）

### 手順（オプション A）

1. シーン内の各ガス灯 Renderer を配列で参照できる親 `GasLampController` を作成
2. UdonSharp スクリプト（例）：
   - `LightUpLamps()` — Emission を ON にするループ
   - obj5 の `BeatInteract` から Interact 時に呼ぶ、または `BeatSequencer` 拡張
3. `Directional Light`（Hierarchy 既存）の Intensity を 1.0 → 0.2 に変更するイベントを同時発火
4. OP 開始時は Intensity 1.0・Emission OFF に戻す

### 史実・演出メモ

- **ガス灯**は明治期の銀座に合致（#3）
- **太陽暦**掲示は obj5 窓テキストで担保。小物として `koyomi.jpg` を obj5 に
- 鉄道馬車（1882）や過度な夜景ネオンは出さない

### 受け入れ基準

- [ ] OP は昼間として明るい
- [ ] obj5 付近でガス灯が 1 灯以上点灯する
- [ ] ED の「夜の銀座」雰囲気と矛盾しない

---

## 7. 音声読み上げ（TTS）

### 目的

窓テキストの**一字一句同一**のボイスオーバー。desktop/VR で**同一 AudioClip**（パリティ §2）。

### 現状のギャップ

- `BeatSequencer` は**テキストページングのみ**。`AudioSource` / ボイス終了待ちは**未実装**
- 設計書要件：「**ボイス終了まで次進行を無効**」→ §7.3 のコード追加が必要

### 7.1 音声生成（Unity 外）

**台本正本**：`ウィンドウ文言_5obj版.md`（確定テキストから生成。改変しない）

#### 推奨ツール（いずれか）

| ツール | 長所 | 注意 |
|---|---|---|
| **VOICEVOX** | 無料・ローカル・日本語 | 抑制的な声質を選ぶ（ナレーション向け） |
| **COEIROINK** | 無料・ローカル | 同上 |
| **Azure Neural TTS** | 品質安定 | キー必要・従量課金 |
| **ElevenLabs** | 自然 | 日本語・料金確認 |

#### 固有名詞の読み（必ず確認）

| 表記 | 確認する読み |
|---|---|
| 安愚楽鍋 | あぐらなべ |
| 仮名垣魯文 | かながき としもと |
| 廃刀令 | はいとうれい |
| 本木昌造 | もとき しょうぞう |
| 横浜毎日新聞 | よこはま まいにち しんぶん |
| 錦絵 | にしきえ |
| 太陽暦 | たいようれき |

VOICEVOX 等で辞書登録 → 生成 → **耳で確認**。

#### ファイル分割案

**1 ページ = 1 wav/mp3** が管理しやすい（`BeatSequencer` のページ index と対応）。

```
Assets/Syuron/Audio/
  OP_01.wav … OP_03.wav
  obj1_01.wav … obj1_03.wav
  obj2_01.wav …
  obj3_01.wav … obj3_04.wav
  obj4_01.wav … obj4_03.wav
  obj5_01.wav … obj5_03.wav
  ED_01.wav … ED_02.wav
```

- 形式：**WAV または Vorbis**（VRChat 推奨に合わせる）
- Import Settings：`Force To Mono` 推奨、3D Sound は **OFF**（2D UI ナレーション）

### 7.2 Unity への取り込み

1. 上記フォルダにドラッグ
2. 空オブジェクト `NarrationAudio` に `AudioSource` を追加
   - Play On Awake：**OFF**
   - Spatial Blend：**0**（2D）
3. （任意）各 Clip を `BeatSequencer` から参照できるよう配列化

### 7.3 コード連携（要実装）

`BeatSequencer` または別 `NarrationController`（UdonSharp）に追加する最小機能：

1. **ページ表示時** — 対応する `AudioClip` を `AudioSource.Play()`  
2. **再生中** — `TryPageNext()` / E キーを**無効**（`interactLocked` と同様の `voiceLocked`）  
3. **再生終了** — `Update` で `!audioSource.isPlaying` を検知 → `voiceLocked = false`  
4. **ページ戻し** — 再生中 Clip を Stop → 前ページの Clip を再生

UdonSharp 制約：`async`/コルーチン不可 → `Update` ポーリング or `SendCustomEventDelayedSeconds`（Clip 長を事前に秒数で持つ）。

#### 配列の例（Inspector）

```
opClips[]      // opPages と同長
obj1Clips[] … obj5Clips[]
edClips[]
```

### 7.4 検証

- [ ] 字幕（TMP）と音声が**一字一句一致**
- [ ] ボイス再生中は E / Next で進めない
- [ ] ボイス終了後にのみ次ページへ進める
- [ ] desktop / VR で同一 Clip が鳴る

---

## 今日終了時のスモークテスト

1. Play（ClientSim desktop）→ OP ページ送り（E）→ obj1〜5 を順に Interact  
2. obj3 で鏡の下半身＋暗転を確認  
3. obj5 でガス灯点灯（実装していれば）  
4. ED → 終了案内まで到達  
5. 窓文字が読める距離・サイズであること  
6. （TTS 実装後）音声と字幕の一致・進行ロック

---

## 参照リンク（リポジトリ内）

| 文档 | 内容 |
|---|---|
| `銀座実装セットアップ.md` §A | BeatSequencer 配線 |
| `銀座実装セットアップ.md` §B | 鏡 RenderTexture |
| `VRChatワールド実装設計書_v8.md` §2, §5, §10 | パリティ・可読性・チェックリスト |
| `ウィンドウ文言_5obj版.md` | 台本・TTS 原稿 |
| `TODO_週次_6-15-6-22.md` | 週次スコープ・オミット一覧 |
| `AGENTS.md` | UdonSharp 制約・不変条件 |
