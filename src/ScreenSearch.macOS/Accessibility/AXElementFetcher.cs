using ScreenSearch.Core.Models;

namespace ScreenSearch.macOS.Accessibility;

public sealed class AXElementFetcher
{
    public IReadOnlyList<OperationCandidate> Fetch(string sessionId)
    {
        var seeds = new List<OperationCandidate>
        {
            new($"{sessionId}-01", sessionId, "AA", "button", "Open", 120, 80, 90, 32, true, true),
            new($"{sessionId}-02", sessionId, "AB", "text", "Search", 220, 80, 160, 28, true, true),
            new($"{sessionId}-03", sessionId, "AC", "link", "Downloads", 120, 130, 120, 24, true, true),
            new($"{sessionId}-04", sessionId, "AD", "button", "Cancel", -30, 20, 70, 28, true, true),
            new($"{sessionId}-05", sessionId, "AE", "button", "Tiny", 50, 50, 2, 2, true, true)
        };

        return seeds;
    }
}
