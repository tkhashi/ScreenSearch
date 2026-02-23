using ScreenSearch.Core.Models;

namespace ScreenSearch.Core.Diagnostics;

public sealed class ClickExecutionWriter
{
    public string Write(ClickRequest request)
    {
        var x = request.TargetX?.ToString("0.##") ?? "n/a";
        var y = request.TargetY?.ToString("0.##") ?? "n/a";
        return $"Click Target: {x}, {y} / Type: {request.ClickType} / Result: {request.ExecutionResult}";
    }
}
