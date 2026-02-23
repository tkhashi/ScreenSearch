using ScreenSearch.Core.Candidates;
using ScreenSearch.Core.Diagnostics;
using ScreenSearch.Core.Labeling;
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
    private readonly LabelGenerator _labelGenerator;

    public ExtractCandidatesUseCase(
        CandidateFilter candidateFilter,
        CandidateSampleWriter candidateSampleWriter,
        LabelGenerator labelGenerator)
    {
        _candidateFilter = candidateFilter;
        _candidateSampleWriter = candidateSampleWriter;
        _labelGenerator = labelGenerator;
    }

    public ExtractCandidatesResult Execute(
        OperationSession session,
        IReadOnlyList<OperationCandidate> fetchedCandidates)
    {
        var filtered = _candidateFilter.FilterVisibleCandidates(fetchedCandidates);
        var reassigned = ReassignLabels(filtered);
        var sampleLog = _candidateSampleWriter.WriteTop5(reassigned);
        return new ExtractCandidatesResult(session, reassigned, sampleLog);
    }

    private IReadOnlyList<OperationCandidate> ReassignLabels(IReadOnlyList<OperationCandidate> candidates)
    {
        var labels = _labelGenerator.Generate(candidates.Count);
        var relabeled = new List<OperationCandidate>(candidates.Count);

        for (var index = 0; index < candidates.Count; index++)
        {
            relabeled.Add(candidates[index] with { Label = labels[index] });
        }

        return relabeled;
    }
}
