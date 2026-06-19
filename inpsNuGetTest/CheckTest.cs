using inpsNuGet;
using Xunit;

namespace inpsNuGetTest
{
    [CollectionDefinition("Sequential State Tests", DisableParallelization = true)]
    public class CheckTest
    {
        #region Email Validation Tests

        [Fact]
        public void Email_WithDomainAndExtension_ValidatesCorrectly()
        {
            // Arrange
            Check.Email.ShouldUseFullDomain(false);
            Check.Email.AddValidDomainName("gmail");
            Check.Email.AddValidDomainName("yahoo");
            Check.Email.AddValidDomainExtension("com");
            Check.Email.AddValidDomainExtension("org");

            // Act & Assert
            Assert.True(Check.Email.IsValid("user@gmail.com"));
            Assert.True(Check.Email.IsValid("test@yahoo.org"));

            // Invalid extension or domain
            Assert.False(Check.Email.IsValid("user@gmail.net"));
            Assert.False(Check.Email.IsValid("user@outlook.com"));

            // Bad format (cannot split properly)
            Assert.False(Check.Email.IsValid("invalid-email-format"));
        }

        [Fact]
        public void Email_WithFullDomain_ValidatesCorrectly()
        {
            // Arrange
            Check.Email.ShouldUseFullDomain(true);
            Check.Email.AddValidDomain("outlook.com");
            Check.Email.AddValidDomain("mycompany.co.uk");

            // Act & Assert
            Assert.True(Check.Email.IsValid("user@outlook.com"));
            Assert.True(Check.Email.IsValid("staff@mycompany.co.uk"));

            // Outside of allowed domains
            Assert.False(Check.Email.IsValid("user@gmail.com"));
            Assert.False(Check.Email.IsValid("invalid-email-format"));
        }

        #endregion

        #region Philippine Mobile Number Tests

        [Theory]
        [InlineData("09171234567")]
        [InlineData("+639171234567")]
        [InlineData("639171234567")]
        [InlineData("0917-123-4567")]
        [InlineData("(0917) 123 4567")]
        public void IsAValidPhilippineMobileNumber_ValidFormats_ReturnsTrue(string number)
        {
            Assert.True(Check.IsAValidPhilippineMobileNumber(number));
        }

        [Theory]
        [InlineData("08171234567")]  // Wrong prefix
        [InlineData("0917123456")]   // Too short
        [InlineData("091712345678")] // Too long
        [InlineData("abc")]          // Non-numeric
        public void IsAValidPhilippineMobileNumber_InvalidFormats_ReturnsFalse(string number)
        {
            Assert.False(Check.IsAValidPhilippineMobileNumber(number));
        }

        #endregion

        #region String Helper Tests

        [Theory]
        [InlineData("123456", true)]
        [InlineData("123a56", false)]
        [InlineData("", true)] // Empty string returns true based on source implementation
        public void IsAllNumbers_AssessCorrectly(string input, bool expected)
        {
            Assert.Equal(expected, Check.IsAllNumbers(input));
        }

        [Theory]
        [InlineData("abc1def", true)]
        [InlineData("abcdef", false)]
        [InlineData("", false)]
        public void HasNumbers_AssessCorrectly(string input, bool expected)
        {
            Assert.Equal(expected, Check.HasNumbers(input));
        }

        [Theory]
        [InlineData("hello!world", true)]
        [InlineData("helloworld", false)]
        [InlineData("", false)]
        public void HasSymbols_AssessCorrectly(string input, bool expected)
        {
            Assert.Equal(expected, Check.HasSymbols(input));
        }

        [Theory]
        [InlineData("hello world", true)]
        [InlineData("helloworld", false)]
        [InlineData("", false)]
        public void HasSpaces_AssessCorrectly(string input, bool expected)
        {
            Assert.Equal(expected, Check.HasSpaces(input));
        }

        #endregion

        #region DateTime Helper Tests

        [Fact]
        public void TimeCalculations_ReturnExpectedValues()
        {
            // Arrange
            DateTime now = new DateTime(2026, 1, 1, 12, 0, 0);
            DateTime until = new DateTime(2026, 1, 2, 14, 30, 0); // 1 day, 2 hours, 30 minutes later

            // Act & Assert
            // 26.5 hours = 1590 minutes = 95,400 seconds = 1.10416 days
            Assert.Equal(95400, Check.HowManySecondsLeft(now, until));
            Assert.Equal(1590, Check.HowManyMinutesLeft(now, until));
            Assert.Equal(26.5, Check.HowManyHoursLeft(now, until));
            Assert.Equal(1.10417, Check.HowManyDaysLeft(now, until), precision: 5);
        }

        #endregion
    }
}
