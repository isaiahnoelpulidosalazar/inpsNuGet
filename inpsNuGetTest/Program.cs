using inpsNuGet;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine(Cipher.CaesarCipher("Hello, World!", 3));
    }
}