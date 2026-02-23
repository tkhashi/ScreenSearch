# Quickstart: macOSキーボードクリック支援MVP

## 目的
最小スコープ（S0-1〜S0-5）の検証サイクルを、短時間で再現できる手順を示す。

## 前提
- macOS 環境
- .NET SDK（原則 .NET 10）
- Accessibility 権限を付与可能であること

## 手順（標準）
1. プロジェクトルートでビルドを実行する。  
   `dotnet build`
2. ツールを起動し、権限判定ログを確認する。  
   `dotnet run`
3. 対象アプリ（Finder など）を前面にして候補抽出を実行する。
4. 候補ラベルを指定して左クリック/右クリックを実行する。
5. 実行結果と診断ログを収集する。

## 期待結果
- 権限判定が `true/false` で表示される。
- 前面アプリとフォーカスウィンドウ状態が表示される。
- 候補件数とサンプルフレームが表示される。
- クリック実行時に座標と種別が表示される。

## US1 手動検証（候補抽出のみ）
1. 対象アプリ（Finder など）を前面にする。
2. `SCREENSEARCH_ACCESSIBILITY_TRUSTED=true dotnet run` を実行する。
3. `Accessibility Trusted` と `Active App` が出力されることを確認する。
4. `Elements Extracted` と `Sample Frames (top 5)` が出力されることを確認する。

### US1 期待結果
- `Focused Window: present|absent` が表示される。
- 抽出候補が0件でも処理が異常終了しない。
- サンプルフレームは最大5件で出力される。

## US2 手動検証（左/右クリック実行）
1. `SCREENSEARCH_CLICK_TYPE=left dotnet run --project src/ScreenSearch.UI/ScreenSearch.UI.csproj -- --verify-us2` を実行する。
2. `Click Target` ログの座標と `Type: left` を確認する。
3. `SCREENSEARCH_CLICK_TYPE=right dotnet run --project src/ScreenSearch.UI/ScreenSearch.UI.csproj -- --verify-us2` を実行し、`Type: right` を確認する。
4. 実行ログに `Result: success|failed` が出ることを確認する。

### US2 期待結果
- 候補が一意解決できる場合のみクリックが実行される。
- left/right の種別がログで識別できる。
- 失敗時でもログ欠損なく終了できる。

## US3 手動検証（ホットキー起点フロー）
1. ツールを起動し、ホットキー受信待機状態にする。
2. ホットキーを押して候補表示（オーバーレイ）を確認する。
3. ラベル入力で候補が絞り込まれることを確認する。
4. 候補確定後にクリック実行され、セッション終了ログが出ることを確認する。
5. 同じ手順を再実行し、再起動可能状態へ復帰していることを確認する。

### US3 期待結果
- 開始から終了まで1回の流れが異常終了せず完走する。
- キャンセル時も終了処理が実行される。
- 完了後に次回ホットキーを受け付ける。

## 最新通しフロー（US1→US2→US3）
1. `SCREENSEARCH_ACCESSIBILITY_TRUSTED=true dotnet run` で起動する。
2. ホットキーを押し、候補抽出ログ（`Elements Extracted`）を確認する。
3. ラベル入力で候補を1件へ絞り込む。
4. `left` または `right` を選択し、`Click Target` ログを確認する。
5. セッション終了後に再度ホットキーを押し、再起動可能状態を確認する。

## 必須ログ形式
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

## 失敗時の切り分け
1. Accessibility 権限が未付与か確認
2. 前面アプリ/フォーカスウィンドウ取得ログを確認
3. 候補件数とフレーム取得結果を確認
4. クリック座標が有効範囲か確認
5. 再現継続時は要件再交渉条件に従いスコープ調整を提案
