using ScreenSearch.Core.Models;
using System.Text;

namespace ScreenSearch.Core.Diagnostics;

public static class DiagnosticLogFormatter
{
    public static string Format(
        string macOsVersion,
        string architecture,
        bool accessibilityTrusted,
        string activeApp,
        string focusedWindow,
        int elementsExtracted,
        IReadOnlyList<OperationCandidate> sampleCandidates,
        string clickTarget)
    {
        var builder = new StringBuilder();
        builder.AppendLine("[ScreenSearch Diagnostic Log]");
        builder.AppendLine($"macOS Version: {macOsVersion}");
        builder.AppendLine($"Architecture: {architecture}");
        builder.AppendLine($"Accessibility Trusted: {accessibilityTrusted.ToString().ToLowerInvariant()}");
        builder.AppendLine($"Active App: {activeApp}");
        builder.AppendLine($"Focused Window: {focusedWindow}");
        builder.AppendLine($"Elements Extracted: {elementsExtracted}");
        builder.AppendLine($"Sample Frames (top 5): {BuildSample(sampleCandidates)}");
        builder.AppendLine($"Click Target: {clickTarget}");
        return builder.ToString();
    }

    private static string BuildSample(IReadOnlyList<OperationCandidate> candidates)
    {
        if (candidates.Count == 0)
        {
            return "none";
        }

        return string.Join(" | ", candidates.Take(5).Select(candidate =>
            $"{candidate.Role} / {candidate.Name} / frame ({candidate.FrameX}, {candidate.FrameY}, {candidate.FrameWidth}, {candidate.FrameHeight})"));
    }
}
