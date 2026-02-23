# Tasks: macOSキーボードクリック支援MVP

**Input**: Design documents from `/specs/001-macos-click-assist/`
**Prerequisites**: plan.md (required), spec.md (required), research.md, data-model.md, contracts/, quickstart.md

**Tests**: 今回の仕様ではTDDやテスト先行は明示要求されていないため、手動検証タスクを中心に構成する。

**Organization**: タスクはユーザーストーリー単位で独立実装・独立検証できるように分割する。

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: MVP検証を開始できる最小構成を整える。

- [x] T001 ソリューションとプロジェクト骨組みを作成する in `ScreenSearch.sln`
- [x] T002 `src/ScreenSearch.Core/` プロジェクトを初期化する in `src/ScreenSearch.Core/ScreenSearch.Core.csproj`
- [x] T003 [P] `src/ScreenSearch.macOS/` プロジェクトを初期化する in `src/ScreenSearch.macOS/ScreenSearch.macOS.csproj`
- [x] T004 [P] `src/ScreenSearch.UI/` プロジェクトを初期化する in `src/ScreenSearch.UI/ScreenSearch.UI.csproj`
- [x] T005 共通起動設定を追加する in `Directory.Build.props`
- [x] T006 サイクル定義テンプレートを追加する in `docs/cycle-template.md`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: すべてのユーザーストーリーに共通する基盤を先に整える。

**⚠️ CRITICAL**: このフェーズ完了前に US1/US2/US3 の実装へ進まない。

- [x] T007 診断ログの共通フォーマットを実装する in `src/ScreenSearch.Core/Diagnostics/DiagnosticLogFormatter.cs`
- [x] T008 [P] セッションモデルを定義する in `src/ScreenSearch.Core/Models/OperationSession.cs`
- [x] T009 [P] 候補モデルを定義する in `src/ScreenSearch.Core/Models/OperationCandidate.cs`
- [x] T010 [P] クリック要求モデルを定義する in `src/ScreenSearch.Core/Models/ClickRequest.cs`
- [x] T011 失敗カテゴリ列挙を定義する in `src/ScreenSearch.Core/Models/FailureCategory.cs`
- [x] T012 実現不能時の再交渉判定ルールを実装する in `src/ScreenSearch.Core/Policies/FeasibilityPolicy.cs`
- [x] T013 依存注入の最小構成を追加する in `src/ScreenSearch.UI/Program.cs`
- [x] T014 `What/Why/How to test/Expected/Logs` 出力ヘルパーを追加する in `src/ScreenSearch.Core/Diagnostics/CycleSummaryWriter.cs`

**Checkpoint**: 共通モデル・診断・実行ポリシーが揃い、各ストーリーを独立実装できる。

---

## Phase 3: User Story 1 - 操作候補の抽出確認 (Priority: P1) 🎯 MVP

**Goal**: 前面アプリから候補抽出し、件数とサンプルフレームをログ確認できる状態にする。

**Independent Test**: 対象アプリを前面表示して実行し、権限状態・前面アプリ情報・候補件数・上位フレームが出力されれば完了。

### Implementation for User Story 1

- [x] T015 [P] [US1] Accessibility権限チェックを実装する in `src/ScreenSearch.macOS/Accessibility/AccessibilityHelper.cs`
- [x] T016 [P] [US1] 前面アプリとフォーカスウィンドウ取得を実装する in `src/ScreenSearch.macOS/Accessibility/FocusedWindowProvider.cs`
- [x] T017 [US1] AX要素列挙を実装する in `src/ScreenSearch.macOS/Accessibility/AXElementFetcher.cs`
- [x] T018 [US1] 候補フィルタ（最小サイズ・画面内判定）を実装する in `src/ScreenSearch.Core/Candidates/CandidateFilter.cs`
- [x] T019 [US1] 候補抽出ユースケースを実装する in `src/ScreenSearch.Core/UseCases/ExtractCandidatesUseCase.cs`
- [x] T020 [US1] サンプルフレーム上位5件のログ出力を実装する in `src/ScreenSearch.Core/Diagnostics/CandidateSampleWriter.cs`
- [x] T021 [US1] US1手動検証手順を追加する in `specs/001-macos-click-assist/quickstart.md`

**Checkpoint**: US1単独で候補抽出可否を検証できる。

---

## Phase 4: User Story 2 - キーボードで左/右クリック実行 (Priority: P2)

**Goal**: 候補選択後に左/右クリックを実行し、座標と種別をログ確認できる状態にする。

**Independent Test**: 抽出済み候補を指定して left/right それぞれ実行し、対象アプリの反応とログを確認できれば完了。

### Implementation for User Story 2

- [x] T022 [US2] 候補ラベル解決ロジックを実装する in `src/ScreenSearch.Core/Candidates/CandidateResolver.cs`
- [x] T023 [US2] クリック座標計算を実装する in `src/ScreenSearch.Core/Click/ClickPointCalculator.cs`
- [x] T024 [P] [US2] 左クリック注入を実装する in `src/ScreenSearch.macOS/Input/ClickInjector.cs`
- [x] T025 [P] [US2] 右クリック注入を実装する in `src/ScreenSearch.macOS/Input/RightClickInjector.cs`
- [x] T026 [US2] クリック実行ユースケースを実装する in `src/ScreenSearch.Core/UseCases/ExecuteClickUseCase.cs`
- [x] T027 [US2] クリック結果ログ（座標/種別/成否）を実装する in `src/ScreenSearch.Core/Diagnostics/ClickExecutionWriter.cs`
- [x] T028 [US2] US2手動検証手順を追加する in `specs/001-macos-click-assist/quickstart.md`

**Checkpoint**: US2単独で left/right 実行可否を検証できる。

---

## Phase 5: User Story 3 - ホットキー起点の一連フロー (Priority: P3)

**Goal**: ホットキー開始から、US1の実候補抽出結果に対するオーバーレイラベル表示（座標配置）→入力選択→US2クリック実行→終了までを一連で成立させる。

**Independent Test**: 対象アプリを前面にしてホットキーから1回通し実行し、実候補のラベル表示・入力選択・クリック実行・完了後の再起動可能状態を確認できれば完了。

### Implementation for User Story 3

- [x] T029 [US3] ホットキー受信を実装する in `src/ScreenSearch.UI/HotKey/GlobalHotKeyListener.cs`
- [x] T030 [US3] ラベル生成ロジックを実装する in `src/ScreenSearch.Core/Labeling/LabelGenerator.cs`
- [x] T031 [US3] 接頭辞絞り込みロジックを実装する in `src/ScreenSearch.Core/Labeling/PrefixFilter.cs`
- [x] T032 [P] [US3] オーバーレイ表示モデルを実装する in `src/ScreenSearch.UI/Overlay/OverlayViewModel.cs`
- [x] T033 [P] [US3] オーバーレイUIを実装する in `src/ScreenSearch.UI/Overlay/OverlayView.axaml`
- [x] T034 [US3] 一連フローのオーケストレーションを実装する in `src/ScreenSearch.UI/Workflows/HotKeyClickWorkflow.cs`
- [x] T035 [US3] セッション終了と再起動可能状態への復帰処理を実装する in `src/ScreenSearch.Core/UseCases/FinalizeSessionUseCase.cs`
- [x] T036 [US3] US3手動検証手順を追加する in `specs/001-macos-click-assist/quickstart.md`
- [x] T042 [US3] US1抽出候補へラベルを付与して座標付き表示データを生成する in `src/ScreenSearch.Core/UseCases/ExtractCandidatesUseCase.cs`
- [x] T043 [US3] 実候補座標へオーバーレイラベルを重畳描画する in `src/ScreenSearch.UI/Overlay/OverlayView.axaml.cs`
- [x] T044 [US3] ラベル選択をUS2クリック実行（left/right）へ接続する in `src/ScreenSearch.UI/MainWindow.axaml.cs`
- [x] T045 [US3] 実行中の前面アプリ切替検知時にセッション中断・再取得案内を実装する in `src/ScreenSearch.UI/MainWindow.axaml.cs`
- [x] T046 [US3] US3統合手順（実候補ラベル表示→選択→クリック→終了）を更新する in `specs/001-macos-click-assist/quickstart.md`

**Checkpoint**: US3単独で一連フローの体験確認ができる。

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: 横断事項の整備と最終確認。

- [x] T037 診断ログ出力内容を仕様と突合する in `specs/001-macos-click-assist/contracts/interaction-contract.md`
- [x] T038 [P] 既知制約と再交渉条件を追記する in `specs/001-macos-click-assist/research.md`
- [x] T039 quickstartの手順を最新フローへ更新する in `specs/001-macos-click-assist/quickstart.md`
- [x] T040 [P] 進捗と残課題をPR本文へ反映する in `.github/pull_request_template.md`
- [x] T041 最終セルフチェック結果を記録する in `specs/001-macos-click-assist/checklists/requirements.md`

---

## Phase 7: カーソル移動

- [x] T050 検出したUIの座標へマウスカーソルが移動する
- [x] T051 検出したUIの座標へマウスカーソルが移動し、左クリックする
- [x] T052 検出したUIの座標へマウスカーソルが移動し、右クリックする



---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 (Setup)**: 即時開始可能
- **Phase 2 (Foundational)**: Phase 1 完了後に開始（全USの前提）
- **Phase 3-5 (US1-US3)**: Phase 2 完了後に開始
  - 優先実行は P1 → P2 → P3
  - 人員があれば US2/US3 は並行可能（ただしUS1成果への依存を考慮）
- **Phase 6 (Polish)**: 目標とするUS完了後に実施

### User Story Dependencies

- **US1 (P1)**: 依存なし（Foundational後に単独開始可能）
- **US2 (P2)**: US1の候補抽出結果を利用するためUS1完了推奨
- **US3 (P3)**: US1/US2 の成立を前提に統合

### Within Each User Story

- モデル・データ変換 → ユースケース → I/O実行 → ログ整備 → 手動検証手順更新
- 各USは完了時に単独検証を実施してから次へ進む

---

## Parallel Opportunities

- **Setup**: T003 と T004 は並行可能
- **Foundational**: T008/T009/T010 は並行可能
- **US1**: T015 と T016 は並行可能（後続で統合）
- **US2**: T024 と T025 は並行可能
- **US3**: T032 と T033 は並行可能
- **Polish**: T038 と T040 は並行可能

---

## Parallel Example: User Story 2

```bash
Task: "T024 [US2] 左クリック注入を実装する in src/ScreenSearch.macOS/Input/ClickInjector.cs"
Task: "T025 [US2] 右クリック注入を実装する in src/ScreenSearch.macOS/Input/RightClickInjector.cs"
```

---

## Implementation Strategy

### MVP First (US1)

1. Phase 1 と Phase 2 を完了
2. Phase 3 (US1) を完了
3. US1の独立検証を実施
4. 検証結果を反映して次フェーズ判断

### Incremental Delivery

1. US1 で候補抽出可否を固定
2. US2 でクリック操作価値を追加
3. US3 で体験フローを統合
4. 最後に横断事項を更新

### Parallel Team Strategy

1. 共通基盤は全員で先に完了
2. 以降は担当分割（抽出/クリック/UI）で並行実装
3. 各US完了時に必ず独立検証を実施

---

## Notes

- すべてのタスクはチェックリスト形式で管理し、完了時に `[x]` へ更新する
- 仕様変更や実現困難が発生した場合は、実装継続より先に顧客へ再交渉する
- 各タスク開始時に `What / Why / How to test / Expected / Logs` を提示する
- 変更は小さく保ち、節目で commit / push / PR更新を実施する
