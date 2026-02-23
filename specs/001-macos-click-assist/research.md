# Research: macOSキーボードクリック支援MVP

## Decision 1: 要素抽出はAXUIElementを採用
- Decision: 操作候補の取得は macOS Accessibility API（AXUIElement）を使う。
- Rationale: macOS標準APIであり、UI要素の役割・名称・フレームを取得でき、MVPで必要な候補抽出に直接対応できる。
- Alternatives considered:
  - OCR/画像認識: アクセシビリティ非対応領域を扱える可能性はあるが、MVPの非目的であり実装コストが高い。
  - 画面座標の固定定義: アプリごとの差異に弱く、再現性が低い。

## Decision 2: クリック注入はCGEventを採用
- Decision: 左/右クリック実行は CoreGraphics の CGEvent で注入する。
- Rationale: 座標ベースで左/右クリックを統一的に扱え、S0-4/S0-5 の検証要件に直結する。
- Alternatives considered:
  - AppleScript/UI Scripting: 実行対象により一貫性が低く、座標制御に制約が出やすい。
  - アプリ別の独自自動化API: 汎用性が低くMVPの範囲を超える。

## Decision 3: UI方針は段階導入
- Decision: Epic 0 ではUIなし/最小UIで可否検証し、Epic 1 でAvaloniaオーバーレイ統合を行う。
- Rationale: 先に要素抽出とクリック注入の成立可否を確認したほうが再作業を減らせる。
- Alternatives considered:
  - 初期からフルUI実装: 見た目の完成度は上がるが、根本機能が未成立のまま実装コストが増える。

## Decision 4: .NETバージョン運用
- Decision: 開発ターゲットは .NET 10 とし、環境制約がある場合は .NET 8 を一時フォールバックとして許容する。
- Rationale: 顧客指定（dotnet10）を満たしつつ、実行不能時に開発停止を回避する。
- Alternatives considered:
  - .NET 8固定: 安定性は高いが顧客指定と不一致。
  - .NET 10固定でフォールバックなし: 環境差分で初期検証が停止するリスクが高い。

## Decision 5: 実現不能時の再交渉条件
- Decision: 以下のいずれかを満たした時点で顧客へ要件再交渉する。
  1. 権限付与済みでも前面ウィンドウ取得が安定しない。
  2. 候補抽出が対象2アプリで連続して基準未達。
  3. 左右クリック注入が同一条件で再現失敗。
- Rationale: 強引な前進を避け、切り分け結果にもとづくスコープ調整を行うため。
- Alternatives considered:
  - 問題を抱えたまま次機能へ進む: 後工程で不具合原因が拡散し、修正コストが増える。

## 既知制約（2026-02-23時点）
- AXUIElement 結果は対象アプリ実装差の影響を受け、候補件数は一定ではない。
- CGEvent 注入は Accessibility 権限が未付与の場合に失敗する。
- MVP はアクティブウィンドウ限定であり、バックグラウンド要素は対象外。

## 再交渉条件の運用ルール
- 同一条件で2回連続失敗したカテゴリを再交渉候補とする。
- 再交渉時は「再現手順」「実測ログ」「代替案（縮小スコープ）」をセットで提示する。
