using ScreenSearch.Core.Models;

namespace ScreenSearch.Core.Candidates;

public sealed class CandidateFilter
{
    public IReadOnlyList<OperationCandidate> FilterVisibleCandidates(IReadOnlyList<OperationCandidate> candidates)
    {
        return candidates
            .Where(candidate => candidate.IsVisible)
            .Where(candidate => candidate.HasPositiveSize)
            .Where(candidate => candidate.FrameWidth >= 8 && candidate.FrameHeight >= 8)
            .Where(candidate => candidate.FrameX >= 0 && candidate.FrameY >= 0)
            .ToList();
    }
}
