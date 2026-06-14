# AGENTS.md — 銀座煉瓦街 VRChatワールド（修士研究）

> このファイルはCursor/AIエージェント向けの恒久コンテキスト。リポジトリのルートに置く。
> 変更前に必ず読むこと。仕様の正本は `Docs/Design/` 配下（後述）。

---

## このプロジェクトは何か

- **VRChatワールド1本**（VRChat Worlds SDK 3.10.x / UdonSharp）。
- 修士研究の実験刺激。**desktopモードとVRモードを単一ワールドで両提示**し、3条件（教科書／desktop／VR）比較の操作変数になる。
- 題材＝文明開化、舞台＝**銀座煉瓦街・明治9〜10年（1876〜77）**。視点＝在野の元武士の一人称。
- ⚠️ このリポジトリには前題材（石州口の戦い）の残骸がある。**今回題材へ移行中**。前題材の記述・コードは正本ではない。

## 絶対に壊してはいけない不変条件（研究の生命線）

1. **desktop/VR パリティ**：提示装置（HMDかフラット画面か）以外は完全に同一。
   - VR限定の物理インタラクション（手で掴む等）を**入れない**。入力は Head ポインティング＋クリックのみ。
   - 同一シーン・同一脚本/字幕・同一ボイス・同一進行ロジック・同一体験長。
2. **単一環境厳守**：別シーン/別部屋へのテレポートで場面転換しない（※旧 `GameManager` のフェーズ＝環境テレポート方式は今回は使わない）。1つの銀座の通りで、発光した対象を順に使い、全部使い終えたら暗転・「体験終了」。回想シーン（別環境）も建てない。
3. **情報等価性**：`Docs/Design/FactList.md` の事実13行すべてを、世界内で読めるテキスト（MessageWindowが主担体）として提示する。テストはこの13行からのみ出題。
4. **時代考証**：明治9〜10年の窓の外。**鉄道馬車（1882）は出さない**。錦絵は1:1複製しない。「ハイカラ」は流行語成立が約1898年で時代矛盾＝テスト項目に使わない／窓内の地味語に差し替え。
5. **計測制約**：Udonはログのファイル書き出し・外部送信ができない。行動ログに依存する実装を提案しない。計測は質問紙＋実験者立会い前提。

## 流用するコード（再実装しない）

- `Assets/Scripts/UI/MessageWindow.cs`（＋ `Assets/Prefabs/MessageWindow.prefab`）— 視点下テキストウィンドウ。`IsUserInVR()` でdesktop/VR判定済み、Mode0=遅延フォロー追従、Mode2=ワールド固定。**これを土台に使う**。
- `Assets/Scripts/UI/MessageTrigger.cs` — オブジェクト使用→ウィンドウ表示。各インタラクト対象に付ける。
- `Assets/Scripts/MainSystem/NextButton.cs` — `SendCustomEvent` で進行を呼ぶ。
- `Assets/Scripts/GlowHighlight.cs` — 次対象を発光させる導線（旧TODOのGazeGuideの代替）。
- 退避対象（今回無関係・消さず脇へ）：`Assets/Scripts/Gun/*`、戦闘/敵まわり。

## UdonSharp の制約（AIが最も間違える所）

**UdonSharpは通常のC#ではなくサブセット。以下は使わない／コンパイルできないことが多い：**
- `async`/`await`、コルーチン（`IEnumerator`）→ 代わりに `SendCustomEventDelayedSeconds/Frames` を使う。
- LINQ全般、`List<T>` など多くのジェネリックコレクション → **配列**を使う。
- `try`/`catch`（例外処理）、`interface`/抽象クラス、多くの `static` フィールド。
- 他スクリプト呼び出しは、型付き `UdonSharpBehaviour` 参照への直接メソッド呼び出し、または `SendCustomEvent("MethodName")`。
- Inspector公開はpublicフィールド（または `[SerializeField] private`）。
- 単独体験なので同期は基本不要：`[UdonBehaviourSyncMode(BehaviourSyncMode.None)]` ＋ローカル状態で書く。

**コンパイルの真実はUnity側。** AIが書いたコードは、Unityで再コンパイル→ClientSimでdesktop再生確認するまで「動く」と見なさない。`.cs` を編集したら必ずこのループを回す。

## ドキュメント運用

- 仕様の正本：`Docs/Design/Brief.md`（研究方針）/ `WorldDesign.md`（実装設計）/ `WindowText.md`（窓文言）/ `FactList.md`（事実13行）。
- 作業リスト：`Docs/TODO.md`（今回題材で書き直す）。
- システムの挙動を変えたら、同じコミットで該当docを更新する。
- 旧題材の文書は `Docs/Archive/` に退避済み。**参照・流用しない**。

## 進行モデル（実装の指針）

OP（自動表示）→ 各ビート：次対象が発光→使用(Interact)→MessageWindowにボイス＋字幕→ボイス終了まで次進行を無効→ユーザートリガーで次へ→次対象が発光 … →全対象使用で終了状態（暗転・「体験終了」）→退出。基準は5オブジェクト構成。