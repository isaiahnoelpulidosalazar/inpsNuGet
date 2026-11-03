using System.Text;

namespace inpsNuGet;

/// <summary>Classical ciphers over A-Z. Output is upper-case; non-letters pass through unchanged.</summary>
public static class Cipher
{
    const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    /// <summary>Removes spaces, upper-cases, then writes even-indexed characters followed by odd-indexed ones.</summary>
    public static string TranspositionCipher(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        string s = text.Replace(" ", "").ToUpperInvariant();

        return string.Create(s.Length, s, static (dest, src) =>
        {
            int pos = 0;
            for (int i = 0; i < src.Length; i += 2) dest[pos++] = src[i];
            for (int i = 1; i < src.Length; i += 2) dest[pos++] = src[i];
        });
    }

    /// <summary>Caesar shift. Any integer shift (negative or > 26) is accepted.</summary>
    public static string CaesarCipher(string text, int shift)
    {
        int k = ((shift % 26) + 26) % 26;
        return Substitute(text, Rotate(Alphabet, k));
    }

    public static string KeywordCipher(string text, string keyword) =>
        Substitute(text, BuildKeywordAlphabet(keyword));

    /// <param name="keyLetter">A letter A-Z; the keyword alphabet is rotated right by its alphabet index.</param>
    public static string GiovanniCipher(string text, string keyword, string keyLetter)
    {
        if (string.IsNullOrEmpty(keyLetter))
            throw new ArgumentException("keyLetter must be a letter A-Z.", nameof(keyLetter));

        int keyIndex = Alphabet.IndexOf(char.ToUpperInvariant(keyLetter[0]));
        if (keyIndex < 0)
            throw new ArgumentException("keyLetter must be a letter A-Z.", nameof(keyLetter));

        return Substitute(text, Rotate(BuildKeywordAlphabet(keyword), (26 - keyIndex) % 26));
    }

    static string Substitute(string text, string map)
    {
        ArgumentNullException.ThrowIfNull(text);
        return string.Create(text.Length, (text, map), static (dest, state) =>
        {
            (string src, string table) = state;
            for (int i = 0; i < src.Length; i++)
            {
                char c = char.ToUpperInvariant(src[i]);
                dest[i] = c is >= 'A' and <= 'Z' ? table[c - 'A'] : c;
            }
        });
    }

    static string Rotate(string s, int left) => string.Concat(s.AsSpan(left), s.AsSpan(0, left));

    static string BuildKeywordAlphabet(string keyword)
    {
        ArgumentNullException.ThrowIfNull(keyword);
        var sb = new StringBuilder(26);
        Span<bool> seen = stackalloc bool[26];

        foreach (char raw in keyword)
        {
            char c = char.ToUpperInvariant(raw);
            if (c is >= 'A' and <= 'Z' && !seen[c - 'A'])
            {
                seen[c - 'A'] = true;
                sb.Append(c);
            }
        }
        for (char c = 'A'; c <= 'Z'; c++)
            if (!seen[c - 'A']) sb.Append(c);

        return sb.ToString();
    }
}
