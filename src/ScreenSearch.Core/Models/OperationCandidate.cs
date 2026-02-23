namespace ScreenSearch.Core.Models;

public sealed record OperationCandidate(
    string CandidateId,
    string SessionId,
    string Label,
    string Role,
    string Name,
    double FrameX,
    double FrameY,
    double FrameWidth,
    double FrameHeight,
    bool IsVisible,
    bool IsEnabled
)
{
    public bool HasPositiveSize => FrameWidth > 0 && FrameHeight > 0;
}
