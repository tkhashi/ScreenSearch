namespace ScreenSearch.macOS.Accessibility;

public sealed class AccessibilityHelper
{
    public bool IsAccessibilityTrusted()
    {
        // MVP段階では可否検証を優先し、環境変数上書きで手動検証を可能にする。
        var overrideValue = Environment.GetEnvironmentVariable("SCREENSEARCH_ACCESSIBILITY_TRUSTED");
        if (bool.TryParse(overrideValue, out var trusted))
        {
            return trusted;
        }

        return OperatingSystem.IsMacOS();
    }
}
