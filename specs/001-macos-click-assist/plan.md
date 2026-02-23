# Implementation Plan: macOSキーボードクリック支援MVP

**Branch**: `001-macos-click-assist` | **Date**: 2026-02-23 | **Spec**: [/Users/kazuhiro.takahashi/Documents/github/ScreenSearch/specs/001-macos-click-assist/spec.md](/Users/kazuhiro.takahashi/Documents/github/ScreenSearch/specs/001-macos-click-assist/spec.md)
**Input**: Feature specification from `/specs/001-macos-click-assist/spec.md`

## Summary

macOS のアクティブウィンドウに限定して、候補抽出→ラベル選択→左/右クリック実行までをキーボード主導で成立させる。MVP段階では全画面網羅やOCRは扱わず、AXUIElement による要素抽出と CGEvent によるクリック注入の成立可否を段階的に検証する。

## Technical Context

**Language/Version**: C# / .NET 10（ローカル環境で不可の場合は .NET 8 にフォールバックして差分を記録）  
**Primary Dependencies**: Avalonia（UI/オーバーレイ）, macOS Accessibility API（AXUIElement）, CoreGraphics CGEvent（入力注入）  
**Storage**: N/A（永続化なし。実行時ログは標準出力）  
**Testing**: 手動検証（3〜5ステップの検証手順） + `dotnet build` のビルド確認  
**Target Platform**: macOS（arm64/x64）, アクティブウィンドウ限定
**Project Type**: desktop-app  
**Performance Goals**: 候補抽出開始から候補表示まで 1.5 秒以内（通常UIアプリで20〜200候補想定）  
**Constraints**: 1サイクル 1機能、1〜3ファイル変更優先、過度抽象化禁止、失敗時は原因切り分け優先  
**Scale/Scope**: 単一ユーザーのローカル実行、1セッション1ウィンドウ、候補20〜200件規模

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- [x] 機能実現最優先: Epic 0 を先行し、UI統合前に可否検証を完了する。
- [x] アジャイル検証: 各サイクルで `What / Why / How to test / Expected / Logs` を必須出力する。
- [x] 実行責務: タスク分解と進捗更新はエージェント側で実施する。
- [x] 実現不能時対応: API制約や権限壁で成立しない場合は即時に代替案を提示し要件再合意する。
- [x] 日本語運用: 仕様・計画・検証手順・ログ説明を日本語で統一する。
- [x] デリバリー運用: 検証単位ごとに commit/push、スプリント完了時PRを作成する。

### Constitution Check（Phase 1設計後の再評価）

- [x] 要素抽出（AXUIElement）と入力注入（CGEvent）を分離し、責務境界を維持した。
- [x] 非目的（OCR/ML/他OS）を設計と成果物から除外した。
- [x] ログ必須項目を quickstart と contract に反映した。
- [x] 実装不能時のエスカレーション条件を research に明文化した。

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

**Structure Decision**: 単一デスクトップアプリ構成を採用し、UI層とmacOS依存層を分離する。MVPでは管理コード削減を優先し、新規共通基盤は導入しない。

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| なし | N/A | N/A |
