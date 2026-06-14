# MessageTrigger コンポーネント設計書

## 1. 概要

`MessageTrigger` は、メッセージ表示をコードから外部化するためのコンポーネント。
オブジェクトにアタッチし、インスペクターからメッセージ内容・表示モード・GazeGuide連携を設定可能。

---

## 2. 用途

- シーン内の各所にメッセージトリガーを配置
- コードを変更せずにメッセージ内容を調整
- デザイナーがプログラマーの手を借りずにテキストを編集可能

---

## 3. パブリックプロパティ

### メッセージ設定

| プロパティ | 型 | 説明 | デフォルト |
|-----------|------|------|------------|
| `message` | string | 表示するメッセージ（複数行対応） | "" |
| `displayMode` | int | 表示モード (0/1/2) | 0 |

### Mode 2 用設定

| プロパティ | 型 | 説明 | デフォルト |
|-----------|------|------|------------|
| `anchor` | Transform | World Fixed 時のアンカー | null |

### GazeGuide 連携

| プロパティ | 型 | 説明 | デフォルト |
|-----------|------|------|------------|
| `useGazeGuide` | bool | GazeGuide を使用するか | false |
| `gazeTarget` | Transform | 注視対象 | null |

### トリガー設定

| プロパティ | 型 | 説明 | デフォルト |
|-----------|------|------|------------|
| `triggerOnInteract` | bool | インタラクト時に表示するか | true |
| `autoHideDelay` | float | 自動非表示までの時間（秒、0で無効） | 0 |

### 参照

| プロパティ | 型 | 説明 |
|-----------|------|------|
| `messageWindow` | MessageWindow | メッセージウィンドウへの参照 |

---

## 4. パブリックメソッド

### TriggerMessage()

メッセージを表示する。他のスクリプトから呼び出し可能。

```csharp
// 使用例: イベントからメッセージを表示
messageTrigger.TriggerMessage();
```

### HideMessage()

メッセージを非表示にする。

```csharp
messageTrigger.HideMessage();
```

### SetMessage(string newMessage)

メッセージ内容を動的に変更する。

```csharp
messageTrigger.SetMessage("新しいメッセージ");
messageTrigger.TriggerMessage();
```

### SetDisplayMode(int mode)

表示モードを動的に変更する。

```csharp
messageTrigger.SetDisplayMode(2); // World Fixed に変更
```

---

## 5. 使用例

### 5.1. 基本的なメッセージトリガー

```
オブジェクト: IntroPanelTrigger
├── MessageTrigger
│   ├── message: "ようこそ、石州口の戦いへ。"
│   ├── displayMode: 0 (Always On)
│   └── messageWindow: [MessageWindow への参照]
```

### 5.2. World Fixed モードのトリガー

```
オブジェクト: SignboardTrigger
├── MessageTrigger
│   ├── message: "この先は戦場です。"
│   ├── displayMode: 2 (World Fixed)
│   ├── anchor: [SignboardAnchor への参照]
│   └── messageWindow: [MessageWindow への参照]
```

### 5.3. GazeGuide 連携トリガー

```
オブジェクト: GunExplanationTrigger
├── MessageTrigger
│   ├── message: "手元にあるのはミニエー銃です。"
│   ├── displayMode: 0 (Always On)
│   ├── useGazeGuide: true
│   ├── gazeTarget: [Gun_Minie への参照]
│   └── messageWindow: [MessageWindow への参照]
```

---

## 6. 他のスクリプトとの連携

### EventSequencer からの呼び出し

```csharp
// EventSequencer.cs
public MessageTrigger[] sequenceTriggers;

public void RunStep(int stepIndex)
{
    if (stepIndex < sequenceTriggers.Length)
    {
        sequenceTriggers[stepIndex].TriggerMessage();
    }
}
```

### コライダートリガーからの呼び出し

```csharp
// TriggerZone.cs
public MessageTrigger messageTrigger;

public override void OnPlayerTriggerEnter(VRCPlayerApi player)
{
    if (player.isLocal)
    {
        messageTrigger.TriggerMessage();
    }
}
```

---

## 7. 設計判断

### Q: なぜ MessageWindow に直接メッセージを持たせないのか？

A: 単一責任の原則。MessageWindow は表示ロジックに専念し、メッセージ内容はトリガー側が持つことで、シーン設計者が柔軟にメッセージを配置できる。

### Q: なぜ配列ではなく単一メッセージなのか？

A: シンプルさを優先。複数メッセージのシーケンスは EventSequencer が担当する。

---

## 更新履歴

| 日付 | 内容 |
|------|------|
| 2026-01-25 | 初版作成 |
