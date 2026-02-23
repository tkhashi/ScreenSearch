using ScreenSearch.Core.Models;

namespace ScreenSearch.Core.Policies;

public static class FeasibilityPolicy
{
    public static bool ShouldRenegotiate(IReadOnlyList<FailureCategory> recentFailures)
    {
        if (recentFailures.Count < 2)
        {
            return false;
        }

        var permissionOrFocusFailures = recentFailures.Count(x => x is FailureCategory.Permission or FailureCategory.Focus);
        var extractionFailures = recentFailures.Count(x => x == FailureCategory.Extraction);
        var clickFailures = recentFailures.Count(x => x == FailureCategory.Click);

        return permissionOrFocusFailures >= 2 || extractionFailures >= 2 || clickFailures >= 2;
    }
}
