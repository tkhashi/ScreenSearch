using System.Diagnostics;

namespace ScreenSearch.macOS.Accessibility;

public sealed record FocusedWindowSnapshot(
    string BundleId,
    int Pid,
    string FocusedWindowState,
    string AppName
);

public sealed class FocusedWindowProvider
{
    public FocusedWindowSnapshot GetSnapshot()
    {
        var detected = DetectFrontmostSnapshot();

        var appName = Environment.GetEnvironmentVariable("SCREENSEARCH_ACTIVE_APP")
            ?? detected.AppName
            ?? "Finder";

        var bundleId = Environment.GetEnvironmentVariable("SCREENSEARCH_ACTIVE_BUNDLE")
            ?? detected.BundleId
            ?? "com.apple.finder";

        var pidValue = Environment.GetEnvironmentVariable("SCREENSEARCH_ACTIVE_PID");
        var pid = int.TryParse(pidValue, out var parsedPid)
            ? parsedPid
            : detected.Pid;

        var focusedWindowState = Environment.GetEnvironmentVariable("SCREENSEARCH_FOCUSED_WINDOW")
            ?? detected.FocusedWindowState
            ?? "absent";

        return new FocusedWindowSnapshot(bundleId, pid, focusedWindowState, appName);
    }

    private static (string? AppName, string? BundleId, int Pid, string? FocusedWindowState) DetectFrontmostSnapshot()
    {
        if (!OperatingSystem.IsMacOS())
        {
            return (null, null, Environment.ProcessId, null);
        }

        var appName = RunAppleScript("tell application \"System Events\" to name of first application process whose frontmost is true");
        var bundleId = RunAppleScript("tell application \"System Events\" to bundle identifier of first application process whose frontmost is true");
        var pidRaw = RunAppleScript("tell application \"System Events\" to unix id of first application process whose frontmost is true");
        var windowCountRaw = RunAppleScript("tell application \"System Events\" to count of windows of first application process whose frontmost is true");

        _ = int.TryParse(pidRaw, out var pid);
        _ = int.TryParse(windowCountRaw, out var windowCount);
        var focusedWindowState = windowCount > 0 ? "present" : "absent";

        return (appName, bundleId, pid == 0 ? Environment.ProcessId : pid, focusedWindowState);
    }

    private static string? RunAppleScript(string script)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "osascript",
                ArgumentList = { "-e", script },
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            if (process is null)
            {
                return null;
            }

            var output = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit(3000);

            return process.ExitCode == 0 && !string.IsNullOrWhiteSpace(output)
                ? output
                : null;
        }
        catch
        {
            return null;
        }
    }
}
