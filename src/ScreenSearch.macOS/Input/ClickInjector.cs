namespace ScreenSearch.macOS.Input;

public sealed class ClickInjector
{
    public bool MoveCursor(double x, double y)
    {
        var moved = MouseEventBridge.MoveCursor(x, y);
        Console.WriteLine($"[ClickInjector] move at ({x}, {y}) / Result: {(moved ? "success" : "failed")}");
        return moved;
    }

    public bool InjectLeft(double x, double y)
    {
        var ok = MouseEventBridge.LeftClick(x, y);
        Console.WriteLine($"[ClickInjector] left at ({x}, {y}) / Result: {(ok ? "success" : "failed")}");
        return ok;
    }
}
