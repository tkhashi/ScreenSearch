using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using ScreenSearch.Core.Candidates;
using ScreenSearch.Core.Click;
using ScreenSearch.Core.Diagnostics;
using ScreenSearch.Core.Labeling;
using ScreenSearch.Core.Models;
using ScreenSearch.Core.UseCases;
using ScreenSearch.macOS.Accessibility;
using ScreenSearch.macOS.Input;
using ScreenSearch.UI.HotKey;

namespace ScreenSearch.UI;

public partial class MainWindow : Window
{
    private readonly GlobalHotKeyListener _hotKeyListener = new();
    private readonly LabelGenerator _labelGenerator = new();
    private readonly PrefixFilter _prefixFilter = new();
    private readonly CandidateResolver _candidateResolver = new();
    private readonly ClickPointCalculator _clickPointCalculator = new();
    private readonly ClickExecutionWriter _clickExecutionWriter = new();
    private readonly FinalizeSessionUseCase _finalizeSessionUseCase = new();
    private readonly AccessibilityHelper _accessibilityHelper = new();
    private readonly FocusedWindowProvider _focusedWindowProvider = new();
    private readonly AXElementFetcher _axElementFetcher = new();
    private readonly CandidateFilter _candidateFilter = new();
    private readonly CandidateSampleWriter _candidateSampleWriter = new();
    private readonly ClickInjector _clickInjector = new();
    private readonly RightClickInjector _rightClickInjector = new();
    private OperationSession? _currentSession;
    private IReadOnlyList<OperationCandidate> _currentCandidates = Array.Empty<OperationCandidate>();
    private IReadOnlyList<OperationCandidate> _currentFilteredCandidates = Array.Empty<OperationCandidate>();

    public MainWindow()
    {
        InitializeComponent();

        _hotKeyListener.Triggered += OnHotKeyTriggered;
        OverlayControl.InputChanged += OnOverlayInputChanged;
        AddHandler(KeyDownEvent, OnAnyKeyDown, RoutingStrategies.Tunnel | RoutingStrategies.Bubble, true);
        Opened += (_, _) => Focus();
    }

    private void OnAnyKeyDown(object? sender, KeyEventArgs e)
    {
        if (OverlayControl.IsVisible)
        {
            HandleOverlayInput(e);
            return;
        }

        if (!_hotKeyListener.TryHandle(e.Key, e.KeyModifiers))
        {
            return;
        }

        e.Handled = true;
    }

    private void OnHotKeyTriggered()
    {
        if (!_accessibilityHelper.IsAccessibilityTrusted())
        {
            HotKeyStatusText.Text = "権限未許可: Accessibilityを有効化してください";
            Console.WriteLine("US3 blocked: Accessibility権限が未許可です。");
            Console.Out.Flush();
            return;
        }

        var snapshot = _focusedWindowProvider.GetSnapshot();
        _currentSession = new OperationSession(
            Guid.NewGuid().ToString("N"),
            DateTimeOffset.UtcNow,
            null,
            snapshot.BundleId,
            snapshot.Pid,
            snapshot.FocusedWindowState,
            "success",
            FailureCategory.None);

        var fetched = _axElementFetcher.Fetch(_currentSession.SessionId);
        var extracted = new ExtractCandidatesUseCase(_candidateFilter, _candidateSampleWriter, _labelGenerator)
            .Execute(_currentSession, fetched);

        _currentCandidates = extracted.Candidates;
        _currentFilteredCandidates = _currentCandidates;

        var message = $"[{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss}] HotKey detected: Cmd+Shift+M / Candidates: {_currentCandidates.Count} / Active App: {snapshot.BundleId}";
        var status = _currentCandidates.Count == 0
            ? $"候補0件: {snapshot.BundleId}"
            : $"HotKey検知: Cmd+Shift+M ({DateTime.Now:HH:mm:ss}) / Overlay表示";

        Dispatcher.UIThread.Post(() =>
        {
            Title = $"ScreenSearch - HotKey OK {DateTime.Now:HH:mm:ss}";
            HotKeyStatusText.Text = status;
            OverlayControl.ShowCandidates(_currentCandidates);
        });

        Console.WriteLine(message);
        Console.Out.Flush();
    }

    private void OnOverlayInputChanged(string input)
    {
        _currentFilteredCandidates = _prefixFilter.Filter(_currentCandidates, input);
        OverlayControl.UpdateFiltered(_currentFilteredCandidates);
    }

    private void HandleOverlayInput(KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            FinalizeSession("cancelled", FailureCategory.Extraction, "US3 cancel: セッションを終了");
            e.Handled = true;
            return;
        }

        if (e.Key != Key.Enter)
        {
            return;
        }

        if (_currentSession is null)
        {
            return;
        }

        var currentSnapshot = _focusedWindowProvider.GetSnapshot();
        if (currentSnapshot.BundleId != _currentSession.ActiveAppBundleId || currentSnapshot.Pid != _currentSession.ActiveAppPid)
        {
            FinalizeSession("cancelled", FailureCategory.Focus, "US3中断: 前面アプリが切り替わりました。再実行してください。");
            e.Handled = true;
            return;
        }

        var resolved = _candidateResolver.ResolveByLabel(_currentFilteredCandidates, OverlayControl.InputText);
        if (resolved is null)
        {
            OverlayControl.ShowMessage("候補を一意に確定できません。ラベルを入力してください。");
            e.Handled = true;
            return;
        }

        var clickType = (e.KeyModifiers & KeyModifiers.Control) == KeyModifiers.Control ? "right" : "left";
        var executeClickUseCase = new ExecuteClickUseCase(_candidateResolver, _clickPointCalculator, _clickExecutionWriter);
        var (request, log) = executeClickUseCase.Execute(
            _currentSession.SessionId,
            _currentCandidates,
            resolved.Label,
            clickType,
            (x, y) => clickType == "right" ? _rightClickInjector.InjectRight(x, y) : _clickInjector.InjectLeft(x, y));

        var result = request.ExecutionResult == "success" ? "success" : "failed";
        var category = request.ExecutionResult == "success" ? FailureCategory.None : FailureCategory.Click;
        FinalizeSession(result, category, $"US3 {result}: {log}");
        e.Handled = true;
    }

    private void FinalizeSession(string result, FailureCategory category, string message)
    {
        if (_currentSession is not null)
        {
            _currentSession = _finalizeSessionUseCase.Finalize(_currentSession, result, category);
        }

        OverlayControl.HideOverlay();
        HotKeyStatusText.Text = $"{message} / 再実行可能";
        Console.WriteLine(message);
        Console.Out.Flush();
    }
}
