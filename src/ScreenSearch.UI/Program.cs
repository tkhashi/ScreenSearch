using Avalonia;
using ScreenSearch.Core.Candidates;
using ScreenSearch.Core.Click;
using ScreenSearch.Core.Diagnostics;
using ScreenSearch.Core.Models;
using ScreenSearch.Core.Policies;
using ScreenSearch.Core.UseCases;
using ScreenSearch.macOS.Accessibility;
using ScreenSearch.macOS.Input;

namespace ScreenSearch.UI;

internal static class Program
{
    public static IReadOnlyDictionary<string, object> BuildServices()
    {
        return new Dictionary<string, object>
        {
            [nameof(DiagnosticLogFormatter)] = new Func<string, string, bool, string, string, int, IReadOnlyList<OperationCandidate>, string, string>(DiagnosticLogFormatter.Format),
            [nameof(FeasibilityPolicy)] = new Func<IReadOnlyList<FailureCategory>, bool>(FeasibilityPolicy.ShouldRenegotiate),
            [nameof(CycleSummaryWriter)] = new Func<string, string, IReadOnlyList<string>, IReadOnlyList<string>, IReadOnlyList<string>, string>(CycleSummaryWriter.Write)
        };
    }

    private static void Main(string[] args)
    {
        _ = BuildServices();

        if (args.Contains("--verify-us1", StringComparer.OrdinalIgnoreCase))
        {
            RunUs1Verification();
            return;
        }

        if (args.Contains("--verify-us2", StringComparer.OrdinalIgnoreCase))
        {
            RunUs2Verification();
            return;
        }

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    private static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect();
    }

    private static void RunUs1Verification()
    {
        var helper = new AccessibilityHelper();
        var focusedWindowProvider = new FocusedWindowProvider();
        var fetcher = new AXElementFetcher();
        var filter = new CandidateFilter();
        var sampleWriter = new CandidateSampleWriter();
        var extractUseCase = new ExtractCandidatesUseCase(filter, sampleWriter);

        var snapshot = focusedWindowProvider.GetSnapshot();
        var session = new OperationSession(
            Guid.NewGuid().ToString("N"),
            DateTimeOffset.UtcNow,
            null,
            snapshot.BundleId,
            snapshot.Pid,
            snapshot.FocusedWindowState,
            "success",
            FailureCategory.None);

        var fetched = fetcher.Fetch(session.SessionId);
        var result = extractUseCase.Execute(session, fetched);

        var diagnosticLog = DiagnosticLogFormatter.Format(
            Environment.OSVersion.VersionString,
            System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant(),
            helper.IsAccessibilityTrusted(),
            $"{snapshot.BundleId} / {snapshot.Pid}",
            snapshot.FocusedWindowState,
            result.Candidates.Count,
            result.Candidates,
            "n/a, n/a / Type: n/a");

        Console.WriteLine(diagnosticLog);
    }

    private static void RunUs2Verification()
    {
        var helper = new AccessibilityHelper();
        var focusedWindowProvider = new FocusedWindowProvider();
        var fetcher = new AXElementFetcher();
        var filter = new CandidateFilter();
        var sampleWriter = new CandidateSampleWriter();
        var extractUseCase = new ExtractCandidatesUseCase(filter, sampleWriter);
        var clickUseCase = new ExecuteClickUseCase(
            new CandidateResolver(),
            new ClickPointCalculator(),
            new ClickExecutionWriter());
        var leftInjector = new ClickInjector();
        var rightInjector = new RightClickInjector();

        var snapshot = focusedWindowProvider.GetSnapshot();
        var session = new OperationSession(
            Guid.NewGuid().ToString("N"),
            DateTimeOffset.UtcNow,
            null,
            snapshot.BundleId,
            snapshot.Pid,
            snapshot.FocusedWindowState,
            "success",
            FailureCategory.None);

        var fetched = fetcher.Fetch(session.SessionId);
        var extracted = extractUseCase.Execute(session, fetched);

        var label = Environment.GetEnvironmentVariable("SCREENSEARCH_TARGET_LABEL")
            ?? extracted.Candidates.FirstOrDefault()?.Label
            ?? "AA";
        var clickType = (Environment.GetEnvironmentVariable("SCREENSEARCH_CLICK_TYPE") ?? "left")
            .Trim()
            .ToLowerInvariant();

        Func<double, double, bool> clickAction = clickType == "right"
            ? rightInjector.InjectRight
            : leftInjector.InjectLeft;

        var execution = clickUseCase.Execute(session.SessionId, extracted.Candidates, label, clickType, clickAction);
        var clickTarget = execution.Request.TargetX.HasValue && execution.Request.TargetY.HasValue
            ? $"{execution.Request.TargetX.Value:0.##}, {execution.Request.TargetY.Value:0.##} / Type: {execution.Request.ClickType}"
            : $"n/a, n/a / Type: {execution.Request.ClickType}";

        var diagnosticLog = DiagnosticLogFormatter.Format(
            Environment.OSVersion.VersionString,
            System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant(),
            helper.IsAccessibilityTrusted(),
            $"{snapshot.BundleId} / {snapshot.Pid}",
            snapshot.FocusedWindowState,
            extracted.Candidates.Count,
            extracted.Candidates,
            clickTarget);

        Console.WriteLine(diagnosticLog);
        Console.WriteLine(execution.Log);
    }
}
