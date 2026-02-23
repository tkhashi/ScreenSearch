using ScreenSearch.Core.Models;

namespace ScreenSearch.Core.Click;

public sealed class ClickPointCalculator
{
    public (double X, double Y) CalculateCenter(OperationCandidate candidate)
    {
        var x = candidate.FrameX + candidate.FrameWidth / 2;
        var y = candidate.FrameY + candidate.FrameHeight / 2;
        return (x, y);
    }
}
