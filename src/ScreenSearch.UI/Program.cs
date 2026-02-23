using Avalonia;
using ScreenSearch.Core.Candidates;
using ScreenSearch.Core.Diagnostics;
using ScreenSearch.Core.Models;
using ScreenSearch.Core.Policies;
using ScreenSearch.Core.UseCases;
using ScreenSearch.macOS.Accessibility;

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
}
