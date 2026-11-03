namespace inpsNuGet;

public static class Text
{
    /// <summary>Text between the first and last double quote. Throws <see cref="FormatException"/> if there aren't two.</summary>
    public static string GetTextFromDoubleQuotations(string line) =>
        TryGetTextFromDoubleQuotations(line, out string result)
            ? result
            : throw new FormatException("Input must contain two double-quote characters.");

    public static bool TryGetTextFromDoubleQuotations(string line, out string result)
    {
        ArgumentNullException.ThrowIfNull(line);
        int first = line.IndexOf('"');
        int last = line.LastIndexOf('"');
        if (first < 0 || last <= first)
        {
            result = string.Empty;
            return false;
        }
        result = line.Substring(first + 1, last - first - 1);
        return true;
    }
}
