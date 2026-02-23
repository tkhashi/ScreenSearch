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
