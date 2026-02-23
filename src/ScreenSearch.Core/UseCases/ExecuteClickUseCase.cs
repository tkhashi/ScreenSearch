using ScreenSearch.Core.Candidates;
using ScreenSearch.Core.Click;
using ScreenSearch.Core.Diagnostics;
using ScreenSearch.Core.Models;

namespace ScreenSearch.Core.UseCases;

public sealed class ExecuteClickUseCase
{
    private readonly CandidateResolver _candidateResolver;
    private readonly ClickPointCalculator _clickPointCalculator;
    private readonly ClickExecutionWriter _clickExecutionWriter;

    public ExecuteClickUseCase(
        CandidateResolver candidateResolver,
        ClickPointCalculator clickPointCalculator,
        ClickExecutionWriter clickExecutionWriter)
    {
        _candidateResolver = candidateResolver;
        _clickPointCalculator = clickPointCalculator;
        _clickExecutionWriter = clickExecutionWriter;
    }

    public (ClickRequest Request, string Log) Execute(
        string sessionId,
        IReadOnlyList<OperationCandidate> candidates,
        string label,
        string clickType,
        Func<double, double, bool> clickAction)
    {
        var normalizedClickType = clickType.Trim().ToLowerInvariant();
        if (normalizedClickType is not ("left" or "right"))
        {
            var invalidType = new ClickRequest(
                Guid.NewGuid().ToString("N"),
                sessionId,
                label,
                normalizedClickType,
                DateTimeOffset.UtcNow,
                null,
                null,
                null,
                "failed");
            return (invalidType, _clickExecutionWriter.Write(invalidType));
        }

        var resolved = _candidateResolver.ResolveByLabel(candidates, label);
        if (resolved is null)
        {
            var missing = new ClickRequest(
                Guid.NewGuid().ToString("N"),
                sessionId,
                label,
                normalizedClickType,
                DateTimeOffset.UtcNow,
                null,
                null,
                null,
                "failed");
            return (missing, _clickExecutionWriter.Write(missing));
        }

        var (x, y) = _clickPointCalculator.CalculateCenter(resolved);
        bool ok;
        try
        {
            ok = clickAction(x, y);
        }
        catch
        {
            ok = false;
        }

        var request = new ClickRequest(
            Guid.NewGuid().ToString("N"),
            sessionId,
            resolved.Label,
            normalizedClickType,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow,
            x,
            y,
            ok ? "success" : "failed");

        return (request, _clickExecutionWriter.Write(request));
    }
}
