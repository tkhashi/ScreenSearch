# Data Model: macOSキーボードクリック支援MVP

## 1. 操作セッション
- 説明: 1回のホットキー起動から終了までの処理単位。
- 主な属性:
  - sessionId: セッション識別子
  - startedAt: 開始時刻
  - endedAt: 終了時刻（未終了時は空）
  - activeAppBundleId: 前面アプリ識別子
  - activeAppPid: 前面アプリPID
  - focusedWindowState: `present` / `absent`
  - result: `success` / `failed` / `cancelled`
  - failureCategory: `permission` / `focus` / `extraction` / `click` / `none`
- バリデーション:
  - startedAt は必須
  - result が `failed` の場合、failureCategory は `none` 以外必須

## 2. 操作候補
- 説明: クリック対象として提示する画面要素。
- 主な属性:
  - candidateId: 候補識別子
  - sessionId: 所属セッション
  - label: 入力選択用ラベル
  - role: 要素の役割（button/link/text等）
  - name: 表示名（空許容）
  - frameX, frameY, frameWidth, frameHeight: 画面座標
  - isVisible: 可視判定
  - isEnabled: 有効判定
- バリデーション:
  - label はセッション内で一意
  - frameWidth/Height は 0 より大きい
  - 画面外要素は候補除外対象として記録

## 3. クリック要求
- 説明: 利用者の選択を実行に変換する指示。
- 主な属性:
  - requestId: 要求識別子
  - sessionId: 所属セッション
  - label: 対象候補ラベル
  - clickType: `left` / `right`
  - requestedAt: 要求時刻
  - executedAt: 実行時刻
  - targetX, targetY: 実行座標
  - executionResult: `success` / `failed`
- バリデーション:
  - clickType は `left` または `right` のみ
  - label が対応候補に存在しない場合は失敗として扱う

## 4. 診断ログ項目
- 説明: サイクル検証で必須の観測情報。
- 主な属性:
  - osVersion
  - architecture
  - accessibilityTrusted
  - elementsExtracted
  - sampleFramesTopN
  - clickTarget
- バリデーション:
  - 失敗時も可能な限り項目欠損を避ける
  - `sampleFramesTopN` は最大5件を標準出力

## 関係
- 操作セッション 1 : N 操作候補
- 操作セッション 1 : N クリック要求
- クリック要求 N : 1 操作候補（labelで解決）
