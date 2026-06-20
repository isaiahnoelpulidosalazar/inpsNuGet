using inpsNuGet;
using Xunit;

namespace inpsNuGetTest
{
    public class CipherTest
    {
        #region Transposition Cipher Tests

        [Theory]
        [InlineData("HELLO WORLD", "HLOOLELWRD")]      // Simple transposition
        [InlineData("ATTACK AT DAWN", "ATCADWTAKTAN")] // Transposition with spaces
        [InlineData("A1B2C3", "ABC123")]               // Non-alphabet characters are unchanged
        [InlineData("", "")]                           // Edge case: empty string
        public void TranspositionCipher_ValidInputs_ReturnExpectedCipher(string input, string expected)
        {
            // Setup
            string result = Cipher.TranspositionCipher(input);

            // Test
            Assert.Equal(expected, result);
        }

        #endregion

        #region Giovanni Cipher Tests

        [Theory]
        [InlineData("HELLO WORLD", "KEYWORD", "G", "EXRRB MBGRV")]      // Simple Giovanni cipher
        [InlineData("ATTACK AT DAWN", "SECRET", "F", "VLLVXA VL YVOF")] // Giovanni cipher with different keyword and key letter
        [InlineData("A 1 B 2 C 3!", "KEYWORD", "G", "S 1 T 2 U 3!")]    // Non-alphabet characters are unchanged
        [InlineData("", "KEYWORD", "G", "")]                            // Edge case: empty string
        public void GiovanniCipher_ValidInputs_ReturnExpectedCipher(string input, string keyword, string keyLetter, string expected)
        {
            // Setup
            string result = Cipher.GiovanniCipher(input, keyword, keyLetter);

            // Test
            Assert.Equal(expected, result);
        }

        #endregion

        #region Keyword Cipher Tests

        [Theory]
        [InlineData("HELLO WORLD", "KEYWORD", "AOGGJ UJNGW")]      // Simple keyword cipher
        [InlineData("ATTACK AT DAWN", "SECRET", "SQQSCH SQ RSWK")] // Keyword cipher with different keyword
        [InlineData("A 1 B 2 C 3!", "KEYWORD", "K 1 E 2 Y 3!")]    // Non-alphabet characters are unchanged
        [InlineData("", "KEYWORD", "")]                            // Edge case: empty string
        public void KeywordCipher_ValidInputs_ReturnExpectedCipher(string input, string keyword, string expected)
        {
            // Setup
            string result = Cipher.KeywordCipher(input, keyword);

            // Test
            Assert.Equal(expected, result);
        }

        #endregion

        #region Caesar Cipher Tests

        [Theory]
        [InlineData("HELLO WORLD", 3, "KHOOR ZRUOG")]       // Simple Caesar cipher with shift of 3
        [InlineData("ATTACK AT DAWN", 5, "FYYFHP FY IFBS")] // Caesar cipher with shift of 5
        [InlineData("A 1 B 2 C 3!", 3, "D 1 E 2 F 3!")]     // Non-alphabet characters are unchanged
        [InlineData("HELLO WORLD", 0, "HELLO WORLD")]       // Shift of 0 returns original (in uppercase)
        [InlineData("", 3, "")]                             // Edge case: empty string
        public void CaesarCipher_ValidInputs_ReturnExpectedCipher(string input, int shift, string expected)
        {
            // Setup
            string result = Cipher.CaesarCipher(input, shift);

            // Test
            Assert.Equal(expected, result);
        }

        #endregion
    }
}
