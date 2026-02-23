namespace ScreenSearch.Core.Models;

public sealed record ClickRequest(
    string RequestId,
    string SessionId,
    string Label,
    string ClickType,
    DateTimeOffset RequestedAt,
    DateTimeOffset? ExecutedAt,
    double? TargetX,
    double? TargetY,
    string ExecutionResult
)
{
    public bool IsClickTypeValid => ClickType is "left" or "right";
}
