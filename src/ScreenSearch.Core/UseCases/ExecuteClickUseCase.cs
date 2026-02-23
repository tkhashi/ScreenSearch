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
        var resolved = _candidateResolver.ResolveByLabel(candidates, label);
        if (resolved is null)
        {
            var missing = new ClickRequest(
                Guid.NewGuid().ToString("N"),
                sessionId,
                label,
                clickType,
                DateTimeOffset.UtcNow,
                null,
                null,
                null,
                "failed");
            return (missing, _clickExecutionWriter.Write(missing));
        }

        var (x, y) = _clickPointCalculator.CalculateCenter(resolved);
        var ok = clickAction(x, y);
        var request = new ClickRequest(
            Guid.NewGuid().ToString("N"),
            sessionId,
            label,
            clickType,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow,
            x,
            y,
            ok ? "success" : "failed");

        return (request, _clickExecutionWriter.Write(request));
    }
}
