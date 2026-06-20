using inpsNuGet;
using Xunit;

namespace inpsNuGetTest
{
    public class TextTest
    {
        #region Happy Path Tests

        [Theory]
        [InlineData("My name is \"Alice\".", "Alice")] // Simple case with one pair of quotes
        [InlineData("\"Hello\"", "Hello")] // Case where the entire string is within quotes
        [InlineData("Text with \"spaces and punctuation!\" inside.", "spaces and punctuation!")] // Case with spaces and punctuation inside quotes
        [InlineData("Some \"\" empty quotes", "")] // Empty quotes return an empty string
        [InlineData("First \"one\" and second \"two\"", "one\" and second \"two")] // Extracts between the absolute first and absolute last quotes
        public void GetTextFromDoubleQuotations_ValidInputs_ReturnsExtractedText(string input, string expected)
        {
            // Setup
            string result = Text.GetTextFromDoubleQuotations(input);

            // Test
            Assert.Equal(expected, result);
        }

        #endregion

        #region Exception / Edge Case Tests

        [Theory]
        [InlineData("No quotation marks here at all")] // No quotes
        [InlineData("")]                               // Empty string
        public void GetTextFromDoubleQuotations_NoQuotes_ThrowsArgumentOutOfRangeException(string input)
        {
            // Without any quotation marks, Substring will receive an invalid length and throw
            Assert.Throws<ArgumentOutOfRangeException>(() => Text.GetTextFromDoubleQuotations(input));
        }

        [Fact]
        public void GetTextFromDoubleQuotations_SingleQuote_ThrowsArgumentOutOfRangeException()
        {
            // Test
            string input = "Only one \" quotation mark is present.";

            // With only one quotation mark, IndexOf and LastIndexOf match, resulting in a negative length
            Assert.Throws<ArgumentOutOfRangeException>(() => Text.GetTextFromDoubleQuotations(input));
        }

        #endregion
    }
}
