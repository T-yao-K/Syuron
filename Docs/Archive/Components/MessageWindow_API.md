# MessageWindow API リファレンス

本ドキュメントでは、MessageWindow スクリプトの使い方とコードでの呼び出し方法を説明します。

---

## 1. 概要

`MessageWindow` は VR 空間内でプレイヤーにテキストメッセージを表示するための UI システムです。
3つの表示モードをサポートし、フェーズや状況に応じて柔軟に切り替え可能です。

---

## 2. パブリックメソッド

### 2.1. ShowMessage(string text)

**テキストを表示してウィンドウをアクティブにする**

```csharp
// 基本的な使い方
messageWindow.ShowMessage("表示したいテキスト");

// 改行を含める場合
messageWindow.ShowMessage("1行目のテキスト\n2行目のテキスト");

// 実際の使用例
messageWindow.ShowMessage("敵が発砲しました。しかし火縄銃の射程は約50m。あなたまで届きません。");
```

**動作:**
1. `messageText.text` にテキストを設定
2. `isVisible = true` に設定
3. `gameObject.SetActive(true)` でオブジェクトをアクティブ化
4. Mode 1（ポップアップ）の場合、タイマーをリセット

---

### 2.2. HideWindow()

**ウィンドウを非表示にする**

```csharp
messageWindow.HideWindow();
```

**動作:**
1. `isVisible = false` に設定
2. フェード処理により徐々に透明になる
3. 完全に透明になったら `gameObject.SetActive(false)` で非アクティブ化

---

### 2.3. SetMode(int mode)

**表示モードを切り替える**

```csharp
// 常時表示モード（視点追従）
messageWindow.SetMode(0);

// ポップアップモード（一定時間後に自動非表示）
messageWindow.SetMode(1);

// 完全固定モード（ワールド座標に固定）
messageWindow.SetMode(2);
```

| モード | 値 | 説明 | 使用場面 |
|-------|---|------|---------|
| 常時表示 (Always On) | 0 | プレイヤーの視点に遅延追従 | 作戦室、戦闘中の解説 |
| ポップアップ (Pop-up) | 1 | 一定時間後に自動で消滅 | 短い通知、敵撃破通知 |
| 完全固定 (World Fixed) | 2 | ワールド座標に固定表示 | 電脳空間での看板形式 |

---

### 2.4. ShowPopup(string text)

**ポップアップメッセージを表示する（Mode 1 専用）**

```csharp
messageWindow.ShowPopup("敵を撃破しました！");
```

**動作:**
1. モードを Mode 1 に切り替え
2. `ShowMessage()` を実行
3. `popupDuration` 秒後に自動で非表示

---

## 3. よくある使用パターン

### 3.1. フェーズ遷移時にメッセージを表示

```csharp
public void SetPhase(int nextIndex)
{
    // ... フェーズ切り替え処理 ...

    if (messageWindow != null)
    {
        switch (nextIndex)
        {
            case 0: // Intro
                messageWindow.SetMode(2); // World Fixed
                messageWindow.ShowMessage("ようこそ、関ヶ原の戦いへ");
                break;
            case 1: // Strategy
                messageWindow.SetMode(0); // Always On
                messageWindow.ShowMessage("作戦を立てましょう");
                break;
        }
    }
}
```

### 3.2. イベント発生時にポップアップ表示

```csharp
public void OnEnemyDefeated()
{
    messageWindow.ShowPopup("敵を撃破！");
}
```

### 3.3. シーケンシャルなメッセージ表示

```csharp
// BattleSequencer.cs での例
public void StartIntroduction()
{
    messageWindow.SetMode(0);
    messageWindow.ShowMessage("あなたは山道の物陰に潜んでいます。");
}

public void OnEnemyFire()
{
    messageWindow.ShowMessage("敵が発砲しました。しかし弾は届きません。");
}

public void PromptPlayerFire()
{
    messageWindow.ShowMessage("トリガーを引いて敵を狙ってください。");
}
```

---

## 4. Inspector 設定

### 4.1. 必須の参照設定

| プロパティ | 設定するオブジェクト | 説明 |
|-----------|-------------------|------|
| Background Panel | BackgroundPanel | 背景パネルの GameObject |
| Message Text | Text [TMP] | TextMeshPro の UGUI |
| Canvas Group | Canvas の CanvasGroup | フェード用コンポーネント |

### 4.2. モード別設定

#### Mode 0 (Always On) の調整

| プロパティ | 推奨値 | 説明 |
|-----------|--------|------|
| Distance | 1.5 | カメラからの距離 (m) |
| Follow Speed | 5.0 | 追従のスムーズさ |
| View Offset | (0, -0.3, 0) | 視界下部に配置 |

#### Mode 2 (World Fixed) の調整

| プロパティ | 説明 |
|-----------|------|
| World Fixed Anchor | 固定表示位置の Transform |

---

## 5. 注意事項

### 5.1. 非表示状態について

`MessageWindow` は初期状態で非アクティブです。`ShowMessage()` を呼ばないと表示されません。

```csharp
// ❌ これだけではウィンドウは表示されない
messageWindow.SetMode(0);

// ✅ ShowMessage() を呼ぶ必要がある
messageWindow.SetMode(0);
messageWindow.ShowMessage("テキスト");
```

### 5.2. Mode 2 (World Fixed) を使用する場合

`worldFixedAnchor` が設定されていないと、ウィンドウの位置が更新されません。
事前に空の GameObject を作成し、表示したい位置に配置してください。

```csharp
// Inspector で worldFixedAnchor を設定してから使用
messageWindow.SetMode(2);
messageWindow.ShowMessage("固定位置に表示されるテキスト");
```

---

## 6. デバッグ

Console に以下のログが出力されます：

```
[MessageWindow] メッセージ表示: (テキスト内容)
[MessageWindow] ウィンドウ非表示
[MessageWindow] モード変更: (モード番号)
```

---

## 更新履歴

| 日付 | 内容 |
|------|------|
| 2026-01-25 | 初版作成 |
