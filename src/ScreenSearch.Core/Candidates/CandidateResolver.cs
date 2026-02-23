using ScreenSearch.Core.Models;

namespace ScreenSearch.Core.Candidates;

public sealed class CandidateResolver
{
    public OperationCandidate? ResolveByLabel(IReadOnlyList<OperationCandidate> candidates, string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        var normalizedInput = input.Trim();
        var exact = candidates.Where(candidate => candidate.Label.Equals(normalizedInput, StringComparison.OrdinalIgnoreCase)).ToList();
        if (exact.Count == 1)
        {
            return exact[0];
        }

        var prefixed = candidates.Where(candidate => candidate.Label.StartsWith(normalizedInput, StringComparison.OrdinalIgnoreCase)).ToList();
        return prefixed.Count == 1 ? prefixed[0] : null;
    }
}
