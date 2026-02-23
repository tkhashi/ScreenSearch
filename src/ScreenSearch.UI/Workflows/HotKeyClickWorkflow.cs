using ScreenSearch.Core.Candidates;
using ScreenSearch.Core.Labeling;
using ScreenSearch.Core.Models;
using ScreenSearch.Core.UseCases;
using ScreenSearch.UI.HotKey;
using ScreenSearch.UI.Overlay;

namespace ScreenSearch.UI.Workflows;

public sealed class HotKeyClickWorkflow
{
    private readonly GlobalHotKeyListener _listener;
    private readonly PrefixFilter _prefixFilter;
    private readonly CandidateResolver _resolver;
    private readonly OverlayViewModel _overlayViewModel;
    private readonly FinalizeSessionUseCase _finalizeSessionUseCase;

    public HotKeyClickWorkflow(
        GlobalHotKeyListener listener,
        PrefixFilter prefixFilter,
        CandidateResolver resolver,
        OverlayViewModel overlayViewModel,
        FinalizeSessionUseCase finalizeSessionUseCase)
    {
        _listener = listener;
        _prefixFilter = prefixFilter;
        _resolver = resolver;
        _overlayViewModel = overlayViewModel;
        _finalizeSessionUseCase = finalizeSessionUseCase;
    }

    public void Start(OperationSession session, IReadOnlyList<OperationCandidate> candidates, string input)
    {
        _listener.Triggered += () =>
        {
            var filtered = _prefixFilter.Filter(candidates, input);
            _overlayViewModel.Update(filtered, input);

            var resolved = _resolver.ResolveByLabel(filtered, input);
            var result = resolved is null ? "cancelled" : "success";
            var failure = resolved is null ? FailureCategory.Extraction : FailureCategory.None;
            _ = _finalizeSessionUseCase.Finalize(session, result, failure);
        };
    }
}
