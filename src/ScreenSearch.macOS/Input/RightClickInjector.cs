namespace ScreenSearch.macOS.Input;

public sealed class RightClickInjector
{
    public bool InjectRight(double x, double y)
    {
        Console.WriteLine($"[RightClickInjector] right at ({x}, {y})");
        return OperatingSystem.IsMacOS();
    }
}
