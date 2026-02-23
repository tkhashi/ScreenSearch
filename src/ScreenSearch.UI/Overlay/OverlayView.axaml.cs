using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Threading;
using ScreenSearch.Core.Models;

namespace ScreenSearch.UI.Overlay;

public partial class OverlayView : UserControl
{
    private IReadOnlyList<OperationCandidate> _allCandidates = Array.Empty<OperationCandidate>();

    public event Action<string>? InputChanged;

    public OverlayView()
    {
        InitializeComponent();
        LabelInputBox.TextChanged += (_, _) => InputChanged?.Invoke(InputText);
        BoundsProperty.Changed.AddClassHandler<OverlayView>((view, _) => view.RenderMarkers(_allCandidates));
    }

    public string InputText => LabelInputBox.Text ?? string.Empty;

    public void ShowCandidates(IReadOnlyList<OperationCandidate> candidates)
    {
        _allCandidates = candidates;
        IsVisible = true;
        LabelInputBox.Text = string.Empty;
        OverlayStateText.Text = $"候補表示中: {candidates.Count} 件";
        CandidatesText.Text = candidates.Count == 0
            ? "候補なし"
            : string.Join(" / ", candidates.Select(candidate => $"{candidate.Label}:{candidate.Name}"));

        RenderMarkers(candidates);
        LabelInputBox.Focus();
    }

    public void UpdateFiltered(IReadOnlyList<OperationCandidate> candidates)
    {
        CandidatesText.Text = candidates.Count == 0
            ? "一致候補なし"
            : string.Join(" / ", candidates.Select(candidate => $"{candidate.Label}:{candidate.Name}"));

        RenderMarkers(candidates);
    }

    public void ShowMessage(string message)
    {
        OverlayStateText.Text = message;
    }

    public void HideOverlay()
    {
        IsVisible = false;
        MarkerCanvas.Children.Clear();
    }

    private void RenderMarkers(IReadOnlyList<OperationCandidate> candidates)
    {
        MarkerCanvas.Children.Clear();
        if (!IsVisible || Bounds.Width <= 1 || Bounds.Height <= 1)
        {
            return;
        }

        foreach (var candidate in candidates)
        {
            var marker = new Border
            {
                Background = Brushes.DarkOrange,
                CornerRadius = new CornerRadius(3),
                Padding = new Thickness(4, 1, 4, 1),
                Child = new TextBlock
                {
                    Text = candidate.Label,
                    Foreground = Brushes.Black,
                    FontWeight = FontWeight.Bold
                }
            };

            var left = Math.Clamp(candidate.FrameX, 0, Math.Max(0, Bounds.Width - 48));
            var top = Math.Clamp(candidate.FrameY, 0, Math.Max(0, Bounds.Height - 24));

            Canvas.SetLeft(marker, left);
            Canvas.SetTop(marker, top);
            MarkerCanvas.Children.Add(marker);
        }
    }
}
