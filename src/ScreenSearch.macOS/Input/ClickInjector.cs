namespace ScreenSearch.macOS.Input;

public sealed class ClickInjector
{
    public bool InjectLeft(double x, double y)
    {
        Console.WriteLine($"[ClickInjector] left at ({x}, {y})");
        return OperatingSystem.IsMacOS();
    }
}
