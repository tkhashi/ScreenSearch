using ScreenSearch.Core.Models;

namespace ScreenSearch.Core.Labeling;

public sealed class PrefixFilter
{
    public IReadOnlyList<OperationCandidate> Filter(IReadOnlyList<OperationCandidate> candidates, string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return candidates;
        }

        return candidates
            .Where(candidate => candidate.Label.StartsWith(input, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}
