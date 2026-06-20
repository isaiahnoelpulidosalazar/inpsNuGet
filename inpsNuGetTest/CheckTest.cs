using inpsNuGet;
using Xunit;

namespace inpsNuGetTest
{
    // CollectionDefinition used to ensure tests are run sequentially
    [CollectionDefinition("Sequential State Tests", DisableParallelization = true)]
    public class CheckTest
    {
        #region Email Validation Tests

        [Fact]
        public void Email_WithDomainAndExtension_ValidatesCorrectly()
        {
            // Setup data
            Check.Email.ShouldUseFullDomain(false);
            Check.Email.AddValidDomainName("gmail");
            Check.Email.AddValidDomainName("yahoo");
            Check.Email.AddValidDomainExtension("com");
            Check.Email.AddValidDomainExtension("org");

            // Test valid
            Assert.True(Check.Email.IsValid("user@gmail.com"));
            Assert.True(Check.Email.IsValid("test@yahoo.org"));

            // Test invalid
            Assert.False(Check.Email.IsValid("user@gmail.net"));
            Assert.False(Check.Email.IsValid("user@outlook.com"));

            // Test invalid format
            Assert.False(Check.Email.IsValid("invalid-email-format"));
        }

        [Fact]
        public void Email_WithFullDomain_ValidatesCorrectly()
        {
            // Setup data
            Check.Email.ShouldUseFullDomain(true);
            Check.Email.AddValidDomain("outlook.com");
            Check.Email.AddValidDomain("mycompany.co.uk");

            // Test valid
            Assert.True(Check.Email.IsValid("user@outlook.com"));
            Assert.True(Check.Email.IsValid("staff@mycompany.co.uk"));

            // Test invalid
            Assert.False(Check.Email.IsValid("user@gmail.com"));

            // Test invalid format
            Assert.False(Check.Email.IsValid("invalid-email-format"));
        }

        #endregion

        #region Philippine Mobile Number Tests

        [Theory]
        [InlineData("09171234567")]     // Standard format
        [InlineData("+639171234567")]   // International format
        [InlineData("639171234567")]    // International format without plus
        [InlineData("0917-123-4567")]   // Dashes
        [InlineData("(0917) 123 4567")] // Parentheses and spaces
        public void IsAValidPhilippineMobileNumber_ValidFormats_ReturnsTrue(string number)
        {
            // Test
            Assert.True(Check.IsAValidPhilippineMobileNumber(number));
        }

        [Theory]
        [InlineData("08171234567")]  // Invalid prefix
        [InlineData("0917123456")]   // Too short
        [InlineData("091712345678")] // Too long
        [InlineData("abc")]          // Non-numeric
        public void IsAValidPhilippineMobileNumber_InvalidFormats_ReturnsFalse(string number)
        {
            // Test
            Assert.False(Check.IsAValidPhilippineMobileNumber(number));
        }

        #endregion

        #region String Helper Tests

        [Theory]
        [InlineData("123456", true)]  // All numbers
        [InlineData("123a56", false)] // Contains a letter
        [InlineData("", true)]        // Edge case: empty string is considered all numbers
        public void IsAllNumbers_AssessCorrectly(string input, bool expected)
        {
            // Test
            Assert.Equal(expected, Check.IsAllNumbers(input));
        }

        [Theory]
        [InlineData("abc1def", true)] // Contains a number
        [InlineData("abcdef", false)] // No numbers
        [InlineData("", false)]       // Edge case: empty string has no numbers
        public void HasNumbers_AssessCorrectly(string input, bool expected)
        {
            // Test
            Assert.Equal(expected, Check.HasNumbers(input));
        }

        [Theory]
        [InlineData("hello!world", true)] // Contains a symbol
        [InlineData("helloworld", false)] // No symbols
        [InlineData("", false)]           // Edge case: empty string has no symbols
        public void HasSymbols_AssessCorrectly(string input, bool expected)
        {
            // Test
            Assert.Equal(expected, Check.HasSymbols(input));
        }

        [Theory]
        [InlineData("hello world", true)] // Contains a space
        [InlineData("helloworld", false)] // No spaces
        [InlineData("", false)]           // Edge case: empty string has no spaces
        public void HasSpaces_AssessCorrectly(string input, bool expected)
        {
            // Test
            Assert.Equal(expected, Check.HasSpaces(input));
        }

        #endregion

        #region DateTime Helper Tests

        [Fact]
        public void TimeCalculations_ReturnExpectedValues()
        {
            DateTime now = new DateTime(2026, 1, 1, 12, 0, 0);
            DateTime until = new DateTime(2026, 1, 2, 14, 30, 0); // 1 day, 2 hours, 30 minutes later

            // 1 day + 2.5 hours = 24 + 2.5 = 26.5 hours = 1590 minutes = 95400 seconds
            Assert.Equal(95400, Check.HowManySecondsLeft(now, until));

            // 1 day + 2.5 hours = 24 + 2.5 = 26.5 hours = 1590 minutes
            Assert.Equal(1590, Check.HowManyMinutesLeft(now, until));

            // 1 day + 2.5 hours = 24 + 2.5 = 26.5 hours
            Assert.Equal(26.5, Check.HowManyHoursLeft(now, until));

            // 1 day + 2.5 hours = 1 + (2.5 / 24) days = 1 + 0.1041666667 days
            Assert.Equal(1.10417, Check.HowManyDaysLeft(now, until), precision: 5);
        }

        #endregion
    }
}
