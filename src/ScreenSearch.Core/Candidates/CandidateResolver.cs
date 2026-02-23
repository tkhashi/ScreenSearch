using ScreenSearch.Core.Models;

namespace ScreenSearch.Core.Candidates;

public sealed class CandidateResolver
{
    public OperationCandidate? ResolveByLabel(IReadOnlyList<OperationCandidate> candidates, string input)
    {
        var exact = candidates.Where(candidate => candidate.Label.Equals(input, StringComparison.OrdinalIgnoreCase)).ToList();
        if (exact.Count == 1)
        {
            return exact[0];
        }

        var prefixed = candidates.Where(candidate => candidate.Label.StartsWith(input, StringComparison.OrdinalIgnoreCase)).ToList();
        return prefixed.Count == 1 ? prefixed[0] : null;
    }
}
