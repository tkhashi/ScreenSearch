# Interaction Contract: macOSキーボードクリック支援MVP

## 対象
本契約は、利用者操作とシステム挙動の最小インタラクションを定義する（外部HTTP APIは対象外）。

## セッション開始
- Trigger: ホットキー押下
- Preconditions:
  - 対象アプリが前面に存在
- System MUST:
  - 権限状態を確認してログ出力
  - 前面アプリ情報とフォーカスウィンドウ状態を取得

## 候補抽出
- Input: セッション開始済み状態
- System MUST:
  - 候補一覧を抽出し、候補件数を出力
  - 候補ごとに `label/role/name/frame` を保持
  - 上位5件のサンプルをログ出力
- Failure Contract:
  - 抽出0件時はクリック遷移せず、候補なしとして終了可能

## 候補選択
- Input: ラベル接頭辞または完全一致ラベル
- System MUST:
  - 一意に候補を決定する
  - 一意決定不可時は再入力を要求する

## クリック実行
- Input:
  - `label`: 対象候補
  - `clickType`: `left` または `right`
- System MUST:
  - 候補中心座標を計算してクリック注入
  - `Click Target: x, y / Type: left|right` を出力
- Failure Contract:
  - 実行失敗時に `permission|focus|extraction|click` のカテゴリを出力

## セッション終了
- Trigger: クリック完了、キャンセル、または復旧不能エラー
- System MUST:
  - 最終結果をログ出力
  - 次回起動可能な初期状態へ戻す

## 診断ログ契約（必須）
- System MUST:
  - 以下の順でログを出力する

```text
[ScreenSearch Diagnostic Log]
macOS Version: ...
Architecture: arm64|x64
Accessibility Trusted: true|false
Active App: bundle_id / pid
Focused Window: present|absent
Elements Extracted: N
Sample Frames (top 5): role / name / frame (x, y, w, h)
Click Target: x, y / Type: left|right
```

- Failure Contract:
  - 失敗時は `permission|focus|extraction|click` のいずれかを必ず記録する
  - 失敗時でも欠損なしでログを出力する
