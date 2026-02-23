using ScreenSearch.Core.Candidates;
using ScreenSearch.Core.Diagnostics;
using ScreenSearch.Core.Models;

namespace ScreenSearch.Core.UseCases;

public sealed record ExtractCandidatesResult(
    OperationSession Session,
    IReadOnlyList<OperationCandidate> Candidates,
    string SampleFramesLog
);

public sealed class ExtractCandidatesUseCase
{
    private readonly CandidateFilter _candidateFilter;
    private readonly CandidateSampleWriter _candidateSampleWriter;

    public ExtractCandidatesUseCase(CandidateFilter candidateFilter, CandidateSampleWriter candidateSampleWriter)
    {
        _candidateFilter = candidateFilter;
        _candidateSampleWriter = candidateSampleWriter;
    }

    public ExtractCandidatesResult Execute(
        OperationSession session,
        IReadOnlyList<OperationCandidate> fetchedCandidates)
    {
        var filtered = _candidateFilter.FilterVisibleCandidates(fetchedCandidates);
        var sampleLog = _candidateSampleWriter.WriteTop5(filtered);
        return new ExtractCandidatesResult(session, filtered, sampleLog);
    }
}
