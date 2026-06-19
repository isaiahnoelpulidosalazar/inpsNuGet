using inpsNuGet;
using Xunit;

namespace inpsNuGetTest
{
    public class TextTest
    {
        #region Happy Path Tests

        [Theory]
        [InlineData("My name is \"Alice\".", "Alice")]
        [InlineData("\"Hello\"", "Hello")]
        [InlineData("Text with \"spaces and punctuation!\" inside.", "spaces and punctuation!")]
        [InlineData("Some \"\" empty quotes", "")] // Empty quotes return an empty string
        [InlineData("First \"one\" and second \"two\"", "one\" and second \"two")] // Extracts between the absolute first and absolute last quotes
        public void GetTextFromDoubleQuotations_ValidInputs_ReturnsExtractedText(string input, string expected)
        {
            // Act
            string result = Text.GetTextFromDoubleQuotations(input);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region Exception / Edge Case Tests

        [Theory]
        [InlineData("No quotation marks here at all")]
        [InlineData("")] // Empty string
        public void GetTextFromDoubleQuotations_NoQuotes_ThrowsArgumentOutOfRangeException(string input)
        {
            // Assert: Without any quotation marks, Substring will receive an invalid length and throw
            Assert.Throws<ArgumentOutOfRangeException>(() => Text.GetTextFromDoubleQuotations(input));
        }

        [Fact]
        public void GetTextFromDoubleQuotations_SingleQuote_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            string input = "Only one \" quotation mark is present.";

            // Assert: With only one quotation mark, IndexOf and LastIndexOf match, resulting in a negative length
            Assert.Throws<ArgumentOutOfRangeException>(() => Text.GetTextFromDoubleQuotations(input));
        }

        #endregion
    }
}
