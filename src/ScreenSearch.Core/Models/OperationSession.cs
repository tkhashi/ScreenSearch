namespace ScreenSearch.Core.Models;

public sealed record OperationSession(
    string SessionId,
    DateTimeOffset StartedAt,
    DateTimeOffset? EndedAt,
    string ActiveAppBundleId,
    int ActiveAppPid,
    string FocusedWindowState,
    string Result,
    FailureCategory FailureCategory
)
{
    public bool IsFailureCategoryValid => Result != "failed" || FailureCategory != FailureCategory.None;
}
