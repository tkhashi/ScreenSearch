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
- US2検証で `dotnet run -- --verify-us2` の標準出力が見えない場合は、次を優先して使用する:
	- `dotnet build ScreenSearch.sln`
	- `SCREENSEARCH_ACCESSIBILITY_TRUSTED=true dotnet ./src/ScreenSearch.UI/bin/Debug/net10.0-macos/osx-arm64/ScreenSearch.UI.dll --verify-us2`
- 期待ログ: `[ScreenSearch Diagnostic Log]`、`US2 verify target label`、left/right の `Click Target ... Result: success|failed`
- ログファイル保存先: `Verification log file: ...` に表示される `/tmp` 配下（環境により `/var/folders/.../T/ScreenSearch/verify-us2-latest.log`）
<!-- MANUAL ADDITIONS END -->