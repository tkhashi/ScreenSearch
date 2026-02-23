namespace ScreenSearch.macOS.Input;

public sealed class RightClickInjector
{
    public bool InjectRight(double x, double y)
    {
        var ok = MouseEventBridge.RightClick(x, y);
        Console.WriteLine($"[RightClickInjector] right at ({x}, {y}) / Result: {(ok ? "success" : "failed")}");
        return ok;
    }
}
