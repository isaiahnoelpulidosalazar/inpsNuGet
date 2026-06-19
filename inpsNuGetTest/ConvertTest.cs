using Convert = inpsNuGet.Convert;
using Xunit;

namespace inpsNuGetTest
{
    public class ConvertTest
    {
        #region Reverse Tests

        [Theory]
        [InlineData("hello", "olleh")]
        [InlineData("a b!", "!b a")]
        [InlineData("", "")] // Edge case: empty string
        public void Reverse_ValidInputs_ReturnsReversedString(string input, string expected)
        {
            Assert.Equal(expected, Convert.Reverse(input));
        }

        #endregion

        #region Base64 Tests

        [Fact]
        public void Base64_RoundTrip_EncodesAndDecodesCorrectly()
        {
            string original = "hello world!";
            string base64Expected = "aGVsbG8gd29ybGQh";

            // Act
            string encoded = Convert.ToBase64(original);
            string decoded = Convert.FromBase64(base64Expected);

            // Assert
            Assert.Equal(base64Expected, encoded);
            Assert.Equal(original, decoded);
        }

        [Fact]
        public void Base64_EmptyString_EncodesAndDecodesCorrectly()
        {
            Assert.Equal("", Convert.ToBase64(""));
            Assert.Equal("", Convert.FromBase64(""));
        }

        #endregion

        #region Byte Array Tests

        [Fact]
        public void ByteArray_RoundTrip_ConvertsSuccessfully()
        {
            // Arrange
            string original = "hello";
            byte[] expectedBytes = { 104, 101, 108, 108, 111 };

            // Act
            byte[] bytes = Convert.ToByteArray(original);
            string decoded = Convert.FromByteArray(expectedBytes);

            // Assert
            Assert.Equal(expectedBytes, bytes);
            Assert.Equal(original, decoded);
        }

        #endregion

        #region Hex Tests

        [Theory]
        [InlineData("hello", "68656C6C6F")]
        [InlineData("Hello World!", "48656C6C6F20576F726C6421")]
        [InlineData("", "")]
        public void Hex_RoundTrip_EncodesAndDecodesCorrectly(string text, string hex)
        {
            // Act & Assert
            Assert.Equal(hex, Convert.ToHex(text));
            Assert.Equal(text, Convert.FromHex(hex));
        }

        [Fact]
        public void FromHex_InvalidLength_ThrowsException()
        {
            // FromHex expects characters in pairs of 2. An odd length should throw an exception.
            Assert.Throws<ArgumentOutOfRangeException>(() => Convert.FromHex("686"));
        }

        #endregion

        #region Binary Tests

        [Theory]
        [InlineData("hello", "0110100001100101011011000110110001101111")]
        [InlineData("A", "01000001")]
        [InlineData("", "")]
        public void Binary_RoundTrip_EncodesAndDecodesCorrectly(string text, string binary)
        {
            // Act & Assert
            Assert.Equal(binary, Convert.ToBinary(text));
            Assert.Equal(text, Convert.FromBinary(binary));
        }

        [Fact]
        public void FromBinary_InvalidLength_ThrowsException()
        {
            // FromBinary expects blocks of 8 bits. A non-multiple of 8 should throw an exception.
            Assert.Throws<ArgumentOutOfRangeException>(() => Convert.FromBinary("0100000"));
        }

        #endregion

        #region Numeric Parsing Tests

        [Fact]
        public void NumericParsing_ValidStrings_ParsesCorrectly()
        {
            // Act & Assert
            Assert.Equal(12345, Convert.ToInt("12345"));
            Assert.Equal(-12345, Convert.ToInt("-12345"));

            Assert.Equal(12.34, Convert.ToDouble("12.34"));
            Assert.Equal(-12.34, Convert.ToDouble("-12.34"));

            Assert.Equal(1234567890123L, Convert.ToLong("1234567890123"));

            Assert.Equal(3.14f, Convert.ToFloat("3.14"));
        }

        [Fact]
        public void NumericParsing_InvalidFormat_ThrowsFormatException()
        {
            // Assert that parsing invalid formats triggers typical System.FormatException
            Assert.Throws<FormatException>(() => Convert.ToInt("abc"));
            Assert.Throws<FormatException>(() => Convert.ToDouble("12.34.56"));
        }

        #endregion
    }
}
