using System.Runtime.InteropServices;

namespace ScreenSearch.macOS.Input;

public sealed class RightClickInjector
{
    private const uint KcgHidEventTap = 0;
    private const uint KcgEventRightMouseDown = 3;
    private const uint KcgEventRightMouseUp = 4;
    private const uint KcgMouseButtonRight = 1;

    public bool InjectRight(double x, double y)
    {
        if (!OperatingSystem.IsMacOS() || double.IsNaN(x) || double.IsNaN(y))
        {
            return false;
        }

        var location = new CGPoint(x, y);
        var down = CGEventCreateMouseEvent(IntPtr.Zero, KcgEventRightMouseDown, location, KcgMouseButtonRight);
        var up = CGEventCreateMouseEvent(IntPtr.Zero, KcgEventRightMouseUp, location, KcgMouseButtonRight);
        if (down == IntPtr.Zero || up == IntPtr.Zero)
        {
            if (down != IntPtr.Zero)
            {
                CFRelease(down);
            }

            if (up != IntPtr.Zero)
            {
                CFRelease(up);
            }

            return false;
        }

        CGEventPost(KcgHidEventTap, down);
        CGEventPost(KcgHidEventTap, up);
        CFRelease(down);
        CFRelease(up);

        return true;
    }

    [StructLayout(LayoutKind.Sequential)]
    private readonly struct CGPoint
    {
        public CGPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; }
        public double Y { get; }
    }

    [DllImport("/System/Library/Frameworks/ApplicationServices.framework/ApplicationServices")]
    private static extern IntPtr CGEventCreateMouseEvent(IntPtr source, uint mouseType, CGPoint mouseCursorPosition, uint mouseButton);

    [DllImport("/System/Library/Frameworks/ApplicationServices.framework/ApplicationServices")]
    private static extern void CGEventPost(uint tap, IntPtr evt);

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    private static extern void CFRelease(IntPtr cf);
}
