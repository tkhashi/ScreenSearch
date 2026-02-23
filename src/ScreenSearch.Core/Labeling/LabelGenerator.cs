namespace ScreenSearch.Core.Labeling;

public sealed class LabelGenerator
{
    private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public IReadOnlyList<string> Generate(int count)
    {
        var labels = new List<string>();
        for (var index = 0; index < count; index++)
        {
            var first = Alphabet[(index / Alphabet.Length) % Alphabet.Length];
            var second = Alphabet[index % Alphabet.Length];
            labels.Add($"{first}{second}");
        }

        return labels;
    }
}
