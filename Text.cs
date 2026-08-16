namespace inpsNuGet;

public class Text
{
    public static string GetTextFromDoubleQuotations(string Line)
    {
        return Line.Substring(Line.IndexOf('"') + 1, Line.LastIndexOf('"') - Line.IndexOf('"') - 1);
    }
}