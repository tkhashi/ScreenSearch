namespace ScreenSearch.Core.Diagnostics;

public static class CycleSummaryWriter
{
    public static string Write(
        string what,
        string why,
        IReadOnlyList<string> howToTest,
        IReadOnlyList<string> expected,
        IReadOnlyList<string> logs)
    {
        var lines = new List<string>
        {
            "What",
            $"- {what}",
            "Why",
            $"- {why}",
            "How to test"
        };

        lines.AddRange(howToTest.Select((step, index) => $"{index + 1}. {step}"));
        lines.Add("Expected");
        lines.AddRange(expected.Select(item => $"- {item}"));
        lines.Add("Logs");
        lines.AddRange(logs.Select(item => $"- {item}"));

        return string.Join(Environment.NewLine, lines);
    }
}
