using inpsNuGet;
using Xunit;

namespace inpsNuGetTest
{
    public class CipherTest
    {
        #region Transposition Cipher Tests

        [Theory]
        [InlineData("HELLO WORLD", "HLOOLELWRD")]
        [InlineData("ATTACK AT DAWN", "ATCADWTAKTAN")]
        [InlineData("A1B2C3", "ABC123")] // Keeps non-alphabet characters but shifts their positions
        [InlineData("", "")] // Edge case: empty string
        public void TranspositionCipher_ValidInputs_ReturnExpectedCipher(string input, string expected)
        {
            // Act
            string result = Cipher.TranspositionCipher(input);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region Giovanni Cipher Tests

        [Theory]
        [InlineData("HELLO WORLD", "KEYWORD", "G", "EXRRB MBGRV")]
        [InlineData("ATTACK AT DAWN", "SECRET", "F", "VLLVXA VL YVOF")]
        [InlineData("A 1 B 2 C 3!", "KEYWORD", "G", "S 1 T 2 U 3!")] // Non-alphabet characters are unchanged
        [InlineData("", "KEYWORD", "G", "")] // Edge case: empty string
        public void GiovanniCipher_ValidInputs_ReturnExpectedCipher(string input, string keyword, string keyLetter, string expected)
        {
            // Act
            string result = Cipher.GiovanniCipher(input, keyword, keyLetter);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region Keyword Cipher Tests

        [Theory]
        [InlineData("HELLO WORLD", "KEYWORD", "AOGGJ UJNGW")]
        [InlineData("ATTACK AT DAWN", "SECRET", "SQQSCH SQ RSWK")]
        [InlineData("A 1 B 2 C 3!", "KEYWORD", "K 1 E 2 Y 3!")] // Non-alphabet characters are unchanged
        [InlineData("", "KEYWORD", "")] // Edge case: empty string
        public void KeywordCipher_ValidInputs_ReturnExpectedCipher(string input, string keyword, string expected)
        {
            // Act
            string result = Cipher.KeywordCipher(input, keyword);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region Caesar Cipher Tests

        [Theory]
        [InlineData("HELLO WORLD", 3, "KHOOR ZRUOG")]
        [InlineData("ATTACK AT DAWN", 5, "FYYFHP FY IFBS")]
        [InlineData("A 1 B 2 C 3!", 3, "D 1 E 2 F 3!")] // Non-alphabet characters are unchanged
        [InlineData("HELLO WORLD", 0, "HELLO WORLD")] // Shift of 0 returns original (in uppercase)
        [InlineData("", 3, "")] // Edge case: empty string
        public void CaesarCipher_ValidInputs_ReturnExpectedCipher(string input, int shift, string expected)
        {
            // Act
            string result = Cipher.CaesarCipher(input, shift);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion
    }
}
