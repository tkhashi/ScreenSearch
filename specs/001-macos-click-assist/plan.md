# Implementation Plan: macOSキーボードクリック支援MVP

**Branch**: `001-macos-click-assist` | **Date**: 2026-02-23 | **Spec**: [/Users/kazuhiro.takahashi/Documents/github/ScreenSearch/specs/001-macos-click-assist/spec.md](/Users/kazuhiro.takahashi/Documents/github/ScreenSearch/specs/001-macos-click-assist/spec.md)
**Input**: Feature specification from `/specs/001-macos-click-assist/spec.md`

## Summary

macOS のアクティブウィンドウに限定し、候補抽出→ラベル選択→左/右クリック実行までをキーボード主導で成立させる。MVPでは可否検証を優先し、Epic 0 で AXUIElement と CGEvent の成立を確認してから Epic 1 のオーバーレイ統合に進む。

## Technical Context

**Language/Version**: C# / .NET 10（環境制約時は .NET 8 を一時フォールバックし差分記録）  
**Primary Dependencies**: Avalonia, AXUIElement, CoreGraphics CGEvent  
**Storage**: N/A（永続化なし、ログは標準出力）  
**Testing**: `dotnet build` + 手動検証（3〜5ステップ）  
**Target Platform**: macOS（arm64/x64）、アクティブウィンドウ限定
**Project Type**: desktop-app  
**Performance Goals**: 候補抽出開始から候補提示まで 1.5 秒以内（通常UIアプリ）  
**Constraints**: 1サイクル=1検証機能、1〜3ファイル変更優先、過度抽象化禁止、失敗時は切り分け優先  
**Scale/Scope**: 単一ユーザーのローカル実行、1セッション1ウィンドウ、候補20〜200件

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- [x] 機能実現最優先: Epic 0（可否検証）を優先し、非目的機能を除外する。
- [x] アジャイル検証: 各サイクルで `What / Why / How to test / Expected / Logs` を必須化する。
- [x] 実行責務: タスク分解・優先順位・進捗管理はエージェント主導で実施する。
- [x] 実現不能時対応: API制約・権限制約で成立しない場合、要件再交渉へ切り替える。
- [x] 日本語運用: 仕様・計画・検証・ログ説明を日本語で統一する。
- [x] デリバリー運用: 節目ごとの commit/push とスプリント完了時 PR を実施する。

### Constitution Check（Phase 1再評価）

- [x] Avalonia（表示）と macOS API（検出/注入）の責務分離を維持した。
- [x] 非対象（OCR/ML、他OS、全画面網羅）を成果物スコープから除外した。
- [x] 必須ログ項目を quickstart と contract へ反映した。
- [x] 実装不能時の再交渉条件を research へ明文化した。

## Project Structure

### Documentation (this feature)

```text
specs/001-macos-click-assist/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── interaction-contract.md
└── tasks.md
```

### Source Code (repository root)
<!--
  ACTION REQUIRED: Replace the placeholder tree below with the concrete layout
  for this feature. Delete unused options and expand the chosen structure with
  real paths (e.g., apps/admin, packages/something). The delivered plan must
  not include Option labels.
-->

```text
src/
├── ScreenSearch.Core/
│   ├── LabelGenerator.cs
│   └── ...
├── ScreenSearch.macOS/
│   ├── AccessibilityHelper.cs
│   ├── AXElementFetcher.cs
│   ├── ClickInjector.cs
│   └── ...
└── ScreenSearch.UI/
  ├── MainWindow.axaml
  ├── OverlayView.cs
  └── ...

tests/
└── ScreenSearch.Core.Tests/
```

**Structure Decision**: 単一デスクトップアプリ構成で進め、責務境界を守りつつ管理コードを増やさない最小構成を採用する。

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| なし | N/A | N/A |
