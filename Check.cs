using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;

namespace inpsNuGet;

public class Check
{
    public static class Email
    {
        static ArrayList validDomainNames = new ArrayList();
        static ArrayList validDomainExtensions = new ArrayList();
        static ArrayList validDomains = new ArrayList();
        static bool shouldUseFullDomain = false;

        public static void AddValidDomainName(string str)
        {
            validDomainNames.Add(str);
        }

        public static void AddValidDomainExtension(string str)
        {
            validDomainExtensions.Add(str);
        }
        
        public static void AddValidDomain(string str)
        {
            validDomains.Add(str);
        }

        public static void ShouldUseFullDomain()
        {
            shouldUseFullDomain = true;
        }

        public static void ShouldUseFullDomain(bool boolean)
        {
            shouldUseFullDomain = boolean;
        }

        public static bool IsValid(string str)
        {
            if (shouldUseFullDomain)
            {
                try
                {
                    string[] domain = str.Split('@');
                    return validDomains.Contains(domain[1]);
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                try
                {
                    string[] domain = str.Split('@');
                    string domainName = domain[1].Split('.')[0];
                    string domainExtension = domain[1].Split('.')[1];
                    return validDomainNames.Contains(domainName) && validDomainExtensions.Contains(domainExtension);
                }
                catch
                {
                    return false;
                }
            }
        }
    }

    public static bool IsAValidPhilippineMobileNumber(string Str)
    {
        return Regex.IsMatch(Regex.Replace(Str, @"[\s\-\(\)]", ""), @"^(?:09|\+639|639)\d{9}$");
    }

    public static bool IsAllNumbers(string Str)
    {
        return !string.IsNullOrEmpty(Str) && Str.All(char.IsDigit);
    }

    public static bool HasNumbers(string Str)
    {
        return Str?.Any(char.IsDigit) ?? false;
    }

    public static bool IsAllAsciiNumbers(string Str)
    {
        return !string.IsNullOrEmpty(Str) && Str.All(char.IsAsciiDigit);
    }

    public static bool HasAsciiNumbers(string Str)
    {
        return Str?.Any(char.IsAsciiDigit) ?? false;
    }

    public static bool IsAllSymbols(string Str)
    {
        return !string.IsNullOrEmpty(Str) && Str.All(char.IsSymbol);
    }
    
    public static bool HasSymbols(string Str)
    {
        return Str?.Any(char.IsSymbol) ?? false;
    }

    public static bool IsAllPunctuations(string Str)
    {
        return !string.IsNullOrEmpty(Str) && Str.All(char.IsPunctuation);
    }
    
    public static bool HasPunctuations(string Str)
    {
        return Str?.Any(char.IsPunctuation) ?? false;
    }

    public static bool IsAllSpecialCharacters(string Str)
    {
        return !string.IsNullOrEmpty(Str) && Str.All(c => char.IsPunctuation(c) || char.IsSymbol(c));
    }

    public static bool HasSpecialCharacters(string Str)
    {
        return Str?.Any(c => char.IsPunctuation(c) || char.IsSymbol(c)) ?? false;
    }

    public static bool IsAllSpaces(string Str)
    {
        return !string.IsNullOrEmpty(Str) && Str.All(char.IsWhiteSpace);
    }

    public static bool HasSpaces(string Str)
    {
        return Str?.Any(char.IsWhiteSpace) ?? false;
    }

    public static bool HasNoSpaces(string Str)
    {
        return !string.IsNullOrEmpty(Str) && !Str.Any(char.IsWhiteSpace);
    }

    public static double HowManySecondsLeft(DateTime now, DateTime until)
    {
        return (until - now).TotalSeconds;
    }

    public static double HowManyMinutesLeft(DateTime now, DateTime until)
    {
        return (until - now).TotalMinutes;
    }

    public static double HowManyHoursLeft(DateTime now, DateTime until)
    {
        return (until - now).TotalHours;
    }

    public static double HowManyDaysLeft(DateTime now, DateTime until)
    {
        return (until - now).TotalDays;
    }

    static readonly HttpClient client = new HttpClient
    {
        Timeout = TimeSpan.FromSeconds(5)
    };

    static async Task CheckConnectionAsync()
    {
        try
        {
            var response = await client.GetAsync("http://clients3.google.com/generate_204");

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("You have a working internet connection.");
            }
            else
            {
                Console.WriteLine("No internet access (Server returned an error status).");
            }
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("No internet access (Failed to reach the test server).");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("No internet access (Connection timed out).");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }

    public static void CheckConnection()
    {
        CheckConnectionAsync().GetAwaiter().GetResult();
    }
}
