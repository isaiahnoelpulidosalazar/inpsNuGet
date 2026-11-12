using System.Globalization;
using System.Text;

namespace inpsNuGet;

/// <summary>String conversion helpers (formerly <c>inpsNuGet.Convert</c>).</summary>
public static class StringConvert
{
    public static string Reverse(string str)
    {
        ArgumentNullException.ThrowIfNull(str);
        return string.Create(str.Length, str, static (dest, src) =>
        {
            src.AsSpan().CopyTo(dest);
            dest.Reverse();
        });
    }

    public static string ToBase64(string str) => Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
    public static string FromBase64(string str) => Encoding.UTF8.GetString(Convert.FromBase64String(str));
    public static byte[] ToByteArray(string str) => Encoding.UTF8.GetBytes(str);
    public static string FromByteArray(byte[] array) => Encoding.UTF8.GetString(array);

    /// <summary>Upper-case hex of the UTF-8 bytes.</summary>
    public static string ToHex(string str) => Convert.ToHexString(Encoding.UTF8.GetBytes(str));
    public static string FromHex(string hex) => Encoding.UTF8.GetString(Convert.FromHexString(hex));

    /// <summary>Binary (8 digits per UTF-8 byte).</summary>
    public static string ToBinary(string str)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(str);
        return string.Create(bytes.Length * 8, bytes, static (dest, src) =>
        {
            int pos = 0;
            foreach (byte b in src)
                for (int bit = 7; bit >= 0; bit--)
                    dest[pos++] = ((b >> bit) & 1) == 0 ? '0' : '1';
        });
    }

    public static string FromBinary(string bits)
    {
        ArgumentNullException.ThrowIfNull(bits);
        if (bits.Length % 8 != 0) throw new FormatException("Binary string length must be a multiple of 8.");

        var bytes = new byte[bits.Length / 8];
        for (int i = 0; i < bytes.Length; i++)
        {
            int v = 0;
            for (int j = 0; j < 8; j++)
            {
                char c = bits[i * 8 + j];
                if (c is not ('0' or '1')) throw new FormatException($"Invalid binary digit '{c}'.");
                v = (v << 1) | (c - '0');
            }
            bytes[i] = (byte)v;
        }
        return Encoding.UTF8.GetString(bytes);
    }

    public static int ToInt(string s) => int.Parse(s, CultureInfo.InvariantCulture);
    public static long ToLong(string s) => long.Parse(s, CultureInfo.InvariantCulture);
    public static float ToFloat(string s) => float.Parse(s, CultureInfo.InvariantCulture);
    public static double ToDouble(string s) => double.Parse(s, CultureInfo.InvariantCulture);
}
