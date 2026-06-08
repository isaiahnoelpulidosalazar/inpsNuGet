namespace inpsNuGet;

public class Text
{
    public static string GetTextFromDoubleQuotations(string line)
    {
        return line.Substring(line.IndexOf('"') + 1, line.LastIndexOf('"') - line.IndexOf('"') - 1);
    }
}
