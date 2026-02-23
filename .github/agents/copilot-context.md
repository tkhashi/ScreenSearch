# ScreenSearch Development Guidelines

Auto-generated from all feature plans. Last updated: 2026-02-23

## Active Technologies
- C# / .NET 10（環境制約時は .NET 8 を一時フォールバックし差分記録）
- Avalonia（UI/オーバーレイ）
- macOS Accessibility API（AXUIElement）
- CoreGraphics CGEvent（入力注入）
- ストレージ: N/A（永続化なし、ログは標準出力）

## Project Structure

```text
src/
tests/
specs/
```

## Commands

- `dotnet build`
- `dotnet run`
- `dotnet test`

## Code Style

- C#/.NET: 標準規約を基本とし、コメントは「なぜ」を優先して日本語で記載

## Recent Changes
- 001-macos-click-assist: 仕様・計画・設計成果物（research/data-model/quickstart/contracts）を追加
- 002-pr-state-guard: PR運用ガード（MERGED/CLOSED PRを編集しない）を追加

<!-- MANUAL ADDITIONS START -->
<!-- MANUAL ADDITIONS END -->