using System.Runtime.InteropServices;

namespace ScreenSearch.macOS.Input;

internal static class MouseEventBridge
{
    public static bool MoveCursor(double x, double y)
    {
        if (!OperatingSystem.IsMacOS())
        {
            return false;
        }

        return PostMouseEvent(CGEventType.MouseMoved, x, y, CGMouseButton.Left);
    }

    public static bool LeftClick(double x, double y)
    {
        if (!MoveCursor(x, y))
        {
            return false;
        }

        var downOk = PostMouseEvent(CGEventType.LeftMouseDown, x, y, CGMouseButton.Left);
        var upOk = PostMouseEvent(CGEventType.LeftMouseUp, x, y, CGMouseButton.Left);
        return downOk && upOk;
    }

    public static bool RightClick(double x, double y)
    {
        if (!MoveCursor(x, y))
        {
            return false;
        }

        var downOk = PostMouseEvent(CGEventType.RightMouseDown, x, y, CGMouseButton.Right);
        var upOk = PostMouseEvent(CGEventType.RightMouseUp, x, y, CGMouseButton.Right);
        return downOk && upOk;
    }

    private static bool PostMouseEvent(CGEventType eventType, double x, double y, CGMouseButton button)
    {
        IntPtr cgEvent = IntPtr.Zero;

        try
        {
            cgEvent = CGEventCreateMouseEvent(IntPtr.Zero, eventType, new CGPoint(x, y), button);
            if (cgEvent == IntPtr.Zero)
            {
                return false;
            }

            CGEventPost(CGEventTapLocation.Hid, cgEvent);
            return true;
        }
        finally
        {
            if (cgEvent != IntPtr.Zero)
            {
                CFRelease(cgEvent);
            }
        }
    }

    [DllImport("/System/Library/Frameworks/ApplicationServices.framework/ApplicationServices")]
    private static extern IntPtr CGEventCreateMouseEvent(
        IntPtr source,
        CGEventType mouseType,
        CGPoint mouseCursorPosition,
        CGMouseButton mouseButton);

    [DllImport("/System/Library/Frameworks/ApplicationServices.framework/ApplicationServices")]
    private static extern void CGEventPost(CGEventTapLocation tap, IntPtr @event);

    [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    private static extern void CFRelease(IntPtr cfTypeRef);

    private enum CGEventTapLocation : uint
    {
        Hid = 0
    }

    private enum CGMouseButton : uint
    {
        Left = 0,
        Right = 1
    }

    private enum CGEventType : uint
    {
        LeftMouseDown = 1,
        LeftMouseUp = 2,
        RightMouseDown = 3,
        RightMouseUp = 4,
        MouseMoved = 5
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
}