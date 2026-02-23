using ScreenSearch.Core.Models;

namespace ScreenSearch.UI.Overlay;

public sealed class OverlayViewModel
{
    public IReadOnlyList<OperationCandidate> VisibleCandidates { get; private set; } = Array.Empty<OperationCandidate>();
    public string Input { get; private set; } = string.Empty;

    public void Update(IReadOnlyList<OperationCandidate> candidates, string input)
    {
        VisibleCandidates = candidates;
        Input = input;
    }
}
