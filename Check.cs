using System.Net;
using System.Text.RegularExpressions;

namespace inpsNuGet;

/// <summary>Stateless validation and inspection helpers.</summary>
public static partial class Check
{
    [GeneratedRegex(@"^(?:09|\+639|639)[0-9]{9}\z", RegexOptions.CultureInvariant)]
    private static partial Regex PhMobileRegex();

    [GeneratedRegex(@"[\s\-()]")]
    private static partial Regex FormattingChars();

    public static bool IsAValidPhilippineMobileNumber(string? str) =>
        str is not null && PhMobileRegex().IsMatch(FormattingChars().Replace(str, ""));

    static bool IsAll(string? s, Func<char, bool> p)
    {
        if (string.IsNullOrEmpty(s)) return false;
        foreach (char c in s) if (!p(c)) return false;
        return true;
    }

    static bool HasAny(string? s, Func<char, bool> p)
    {
        if (s is null) return false;
        foreach (char c in s) if (p(c)) return true;
        return false;
    }

    static bool IsSpecial(char c) => char.IsPunctuation(c) || char.IsSymbol(c);

    public static bool IsAllNumbers(string? s) => IsAll(s, char.IsDigit);
    public static bool HasNumbers(string? s) => HasAny(s, char.IsDigit);
    public static bool IsAllAsciiNumbers(string? s) => IsAll(s, char.IsAsciiDigit);
    public static bool HasAsciiNumbers(string? s) => HasAny(s, char.IsAsciiDigit);
    public static bool IsAllSymbols(string? s) => IsAll(s, char.IsSymbol);
    public static bool HasSymbols(string? s) => HasAny(s, char.IsSymbol);
    public static bool IsAllPunctuations(string? s) => IsAll(s, char.IsPunctuation);
    public static bool HasPunctuations(string? s) => HasAny(s, char.IsPunctuation);
    public static bool IsAllSpecialCharacters(string? s) => IsAll(s, IsSpecial);
    public static bool HasSpecialCharacters(string? s) => HasAny(s, IsSpecial);
    public static bool IsAllSpaces(string? s) => IsAll(s, char.IsWhiteSpace);
    public static bool HasSpaces(string? s) => HasAny(s, char.IsWhiteSpace);
    public static bool HasNoSpaces(string? s) => !string.IsNullOrEmpty(s) && !HasAny(s, char.IsWhiteSpace);

    public static double HowManySecondsLeft(DateTime now, DateTime until) => (until - now).TotalSeconds;
    public static double HowManyMinutesLeft(DateTime now, DateTime until) => (until - now).TotalMinutes;
    public static double HowManyHoursLeft(DateTime now, DateTime until) => (until - now).TotalHours;
    public static double HowManyDaysLeft(DateTime now, DateTime until) => (until - now).TotalDays;

    static readonly HttpClient Http = new(new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(2) })
    {
        Timeout = TimeSpan.FromSeconds(5)
    };

    /// <summary>
    /// true only if the connectivity endpoint answers 204 (a captive portal's 200 is not "online").
    /// Returns false on network failure or timeout; cancellation by the caller still throws.
    /// </summary>
    public static async Task<bool> HasInternetAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using HttpResponseMessage r = await Http
                .GetAsync("https://connectivitycheck.gstatic.com/generate_204",
                          HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                .ConfigureAwait(false);
            return r.StatusCode == HttpStatusCode.NoContent;
        }
        catch (HttpRequestException) { return false; }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested) { return false; }
    }
}
