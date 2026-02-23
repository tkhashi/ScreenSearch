# Copilot Instructions for ScreenSearch

> キーボードクリック支援ツール（MVP）開発用。このファイルは Copilot が開発効率を保つための実装ガイドです。

## 開発スタイル

**Agile / 検証ループ（機能実現優先）**を採用：
- MVPの機能実現を保守性より優先
- 過度な抽象化を避け、管理コードが少ない設計を優先
- 各タスクは小さな検証可能機能（1〜3ファイル変更推奨）
- 実装前に `What / Why / How to test / Expected / Logs` を揃える
- ユーザーが実行 → フィードバック → 次サイクル計画
- 実現困難時は強行せず、要件変更・スコープ調整をユーザーに相談

**役割分担**:
- エージェント: 開発者 / PdM / PM としてタスク管理・優先順位管理・実行管理を担当
- ユーザー: 顧客として成果確認・動作検証・方針変更依頼・修正依頼を担当

詳細は `plan.md` を参照。

## Quick Setup

_Backlog に Epic 0 (可否検証) から Epic 1 (体験) の 8 タスクが登録されました。詳細は plan.md を確認_

**環境要件**:
- macOS（Tahoe 26.x 以上推奨）
- .NET 8.0 以上 + Avalonia SDK
- Xcode Command Line Tools

**初期セットアップ**:
```bash
# プロジェクト初期化（未実施）
# dotnet new sln -n ScreenSearch
# cd ScreenSearch
# dotnet add package Avalonia
# ... etc
```

## Build, Test & Lint Commands

```bash
# ビルド（セットアップ後）
# dotnet build

# 実行
# dotnet run

# テスト（ユニットテスト セットアップ後）
# dotnet test
```

**現状**: 開発環境構築前。S0-1 実装開始時にコマンド確定

## High-Level Architecture

**ScreenSearch MVP** = キーボードホットキー で UI要素をクリック実行

### 3層設計

```
┌─────────────────────────────────────┐
│ UI Layer (Avalonia)                  │
│ - ホットキーリスナー                  │
│ - オーバーレイ描画                    │
│ - ラベル表示 + 入力フォーム           │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│ Domain Logic (C#)                   │
│ - ラベル生成・フィルタリング           │
│ - クリック座標決定                    │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│ macOS API Layer                      │
│ - AXUIElement: 要素検出 / Frame取得  │
│ - CGEvent: クリック注入 (左/右)      │
│ - Accessibility権限管理              │
└─────────────────────────────────────┘
```

### データフロー

1. ホットキー起動
2. アクティブウィンドウ のフォーカス要素を取得
3. 子要素を列挙 → ラベル生成
4. オーバーレイ表示
5. ユーザー入力 → 候補絞り込み
6. 確定 → CGEvent でクリック注入 → 終了

### 要素検出の仕組み

- **AXUIElement**: Cocoa/AppKit 標準の Accessibility API
- macOS API 側で要素の役割 (button, text, link etc)・テキスト・フレームを取得
- フレーム = 画面座標 (x, y, width, height)
- 非表示・小さすぎる要素はフィルタ

### 重要な設計方針

- Avalonia は **表示のみ** → マルチプラットフォーム準備
- macOS API は **分離** → テスト・他OS対応を簡単に
- クリック実行は CGEvent を直接呼び出し

## Key Conventions

### 言語・記述ルール

- コメント・ドキュメント・検証手順・タスク記述はすべて日本語
- コードコメントは「なぜこの実装か」「ドメインとして重要な判断」に限定

### Backlog ・ タスク管理

- **Epic** ごとに分類：
  - **Epic 0**: 可否検証（基礎機能）→ S0-1 〜 S0-5
  - **Epic 1**: 体験（最小限の "それっぽさ"）→ S1-1 〜 S1-3
  - **Epic 2**: 安定化（後回し）
  
- **各タスク (id = s0-1, s0-2, etc)**
  - 1サイクル = 検証可能な1機能
  - 変更は最小限（1〜3ファイル推奨）
  - 進捗管理と更新はエージェント側で実施

### コミット・PR運用

- 作業開始前に必ず `git fetch --all --prune` を実行し、`git status -sb` で remote 追従状態を確認してから着手する
- ステップごと、スプリントごと、その他節目で小まめに commit / push
- 新しいブランチで最初の commit / push を行った場合は、必ずその時点で PR を作成する
- 作業完了時は、最新の変更内容・検証結果・残課題を反映して既存 PR を更新する
- PR が `MERGED` または `CLOSED` の場合はその PR を編集せず、新しいブランチを切って新規 PR を作成する
- PR本文は `.github/pull_request_template.md` のフォーマットに従う
- スプリント完了時は PR を作成し、目的・結果・残課題を日本語で記載

### 実装サイクル

**全タスク開始時に必ず提示**:

1. **What** - 何を実装するか
2. **Why** - なぜそれが必要か / 前タスクとの関係
3. **How to test** - ユーザーが実行する手順（3〜5ステップ）
4. **Expected** - 期待される結果（成功条件）
5. **Logs** - 出力すべきログ内容（デバッグ用）

**ユーザーが返す**:
- 実行環境（macOS Ver, Mac型, 対象アプリ）
- 実行手順（上記を番号順に）
- 結果（成功/失敗 + 症状）
- ログ出力（stdout）

### 必須ログ（全サイクル）

```
[ScreenSearch Diagnostic Log]
macOS Version: (e.g., Tahoe 26.x)
Architecture: (arm64 / x64)
Accessibility Trusted: true/false
Active App: bundle_id / pid
Focused Window: present/absent
Elements Extracted: N
Sample Frames (top 5): role / name / frame (x, y, w, h)
Click Target: x, y / Type: left / right
```

### コード構成（予定）

```
ScreenSearch/
├── src/
│   ├── ScreenSearch.Core/       # ドメインロジック
│   │   ├── LabelGenerator.cs    # ラベル生成
│   │   └── ...
│   ├── ScreenSearch.macOS/      # macOS API バインディング
│   │   ├── AccessibilityHelper.cs
│   │   ├── AXElementFetcher.cs
│   │   ├── ClickInjector.cs
│   │   └── ...
│   └── ScreenSearch.UI/         # Avalonia UI
│       ├── MainWindow.axaml
│       ├── OverlayView.cs
│       └── ...
├── tests/                       # ユニットテスト
└── plan.md                      # このプロジェクトの計画書
```

### 失敗時の切り分け

失敗したら **原因特定を優先**：

1. ログが出ているか → デバッグログを追加
2. Accessibility権限はあるか（S0-1 で確認）
3. 対象要素が取得できるか（S0-3 ）
4. 座標が正しいか（ログで確認）
5. 原因が取れるまで前のステップに戻す（前進より切り分け優先）

## Integration With Copilot

このファイルと `plan.md` をセットで使用します：

- **copilot-instructions.md** = 継続的なガイド（技術仕様・コード規約）
- **plan.md** = プロジェクト進行状況・Backlog・次ステップ

**各サイクルで更新する項目**:

- `plan.md` の進行状況
- Backlog 完了タスク
- 発見した新しい制約・パターン

---

**Last updated**: 2026-02-23  
**MVP Status**: Backlog構築完了 → S0-1 実装待ち

