using ScreenSearch.Core.Models;

namespace ScreenSearch.Core.Click;

public sealed class ClickPointCalculator
{
    public (double X, double Y) CalculateCenter(OperationCandidate candidate)
    {
        if (candidate.FrameWidth <= 0 || candidate.FrameHeight <= 0)
        {
            throw new ArgumentException("候補要素のサイズが不正です。", nameof(candidate));
        }

        var x = candidate.FrameX + candidate.FrameWidth / 2;
        var y = candidate.FrameY + candidate.FrameHeight / 2;
        return (x, y);
    }
}
