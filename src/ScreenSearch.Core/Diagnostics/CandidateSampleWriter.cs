using ScreenSearch.Core.Models;

namespace ScreenSearch.Core.Diagnostics;

public sealed class CandidateSampleWriter
{
    public string WriteTop5(IReadOnlyList<OperationCandidate> candidates)
    {
        var top = candidates.Take(5).ToList();
        if (top.Count == 0)
        {
            return "Sample Frames (top 5): none";
        }

        return "Sample Frames (top 5): " + string.Join(" | ", top.Select(candidate =>
            $"{candidate.Role} / {candidate.Name} / frame ({candidate.FrameX}, {candidate.FrameY}, {candidate.FrameWidth}, {candidate.FrameHeight})"));
    }
}
