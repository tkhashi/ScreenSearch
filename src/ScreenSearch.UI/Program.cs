using Avalonia;
using ScreenSearch.Core.Candidates;
using ScreenSearch.Core.Click;
using ScreenSearch.Core.Diagnostics;
using ScreenSearch.Core.Labeling;
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
        var (snapshot, result) = BuildExtractionResult();

        var diagnosticLog = DiagnosticLogFormatter.Format(
            Environment.OSVersion.VersionString,
            System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant(),
            helper.IsAccessibilityTrusted(),
            $"{snapshot.BundleId} / {snapshot.Pid}",
            snapshot.FocusedWindowState,
            result.Candidates.Count,
            result.Candidates,
            "n/a, n/a / Type: n/a");

        WriteVerificationOutput("us1", diagnosticLog);
    }

    private static void RunUs2Verification()
    {
        var helper = new AccessibilityHelper();
        var (snapshot, extracted) = BuildExtractionResult();

        if (extracted.Candidates.Count == 0)
        {
            WriteVerificationOutput("us2", "US2 verify failed: 候補が0件のためクリック検証を実行できません。");
            return;
        }

        var candidateResolver = new CandidateResolver();
        var clickPointCalculator = new ClickPointCalculator();
        var clickExecutionWriter = new ClickExecutionWriter();
        var executeClickUseCase = new ExecuteClickUseCase(candidateResolver, clickPointCalculator, clickExecutionWriter);

        var leftInjector = new ClickInjector();
        var rightInjector = new RightClickInjector();
        var targetLabel = extracted.Candidates[0].Label;

        var (leftRequest, leftLog) = executeClickUseCase.Execute(
            extracted.Session.SessionId,
            extracted.Candidates,
            targetLabel,
            "left",
            (x, y) => leftInjector.InjectLeft(x, y));

        var (rightRequest, rightLog) = executeClickUseCase.Execute(
            extracted.Session.SessionId,
            extracted.Candidates,
            targetLabel,
            "right",
            (x, y) => rightInjector.InjectRight(x, y));

        var diagnosticLog = DiagnosticLogFormatter.Format(
            Environment.OSVersion.VersionString,
            System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant(),
            helper.IsAccessibilityTrusted(),
            $"{snapshot.BundleId} / {snapshot.Pid}",
            snapshot.FocusedWindowState,
            extracted.Candidates.Count,
            extracted.Candidates,
            $"{leftRequest.TargetX?.ToString("0.##") ?? "n/a"}, {leftRequest.TargetY?.ToString("0.##") ?? "n/a"} / Type: left");

        var lines = string.Join(Environment.NewLine, new[]
        {
            diagnosticLog,
            $"US2 verify target label: {targetLabel}",
            leftLog,
            rightLog
        });
        WriteVerificationOutput("us2", lines);
    }

    private static void WriteVerificationOutput(string mode, string content)
    {
        Console.WriteLine(content);

        var logsDirectory = Path.Combine(Path.GetTempPath(), "ScreenSearch");
        Directory.CreateDirectory(logsDirectory);

        var latestPath = Path.Combine(logsDirectory, $"verify-{mode}-latest.log");
        File.WriteAllText(latestPath, content + Environment.NewLine);

        Console.WriteLine($"Verification log file: {latestPath}");
    }

    private static (FocusedWindowSnapshot Snapshot, ExtractCandidatesResult Result) BuildExtractionResult()
    {
        var focusedWindowProvider = new FocusedWindowProvider();
        var fetcher = new AXElementFetcher();
        var filter = new CandidateFilter();
        var sampleWriter = new CandidateSampleWriter();
        var labelGenerator = new LabelGenerator();
        var extractUseCase = new ExtractCandidatesUseCase(filter, sampleWriter, labelGenerator);

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
        return (snapshot, result);
    }
}
