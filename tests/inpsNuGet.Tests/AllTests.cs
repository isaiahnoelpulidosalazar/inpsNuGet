using inpsNuGet;
using Xunit;

namespace inpsNuGet.Tests;

public class CipherTests
{
    [Fact] public void Caesar_ReadmeExample() => Assert.Equal("KHOOR ZRUOG", Cipher.CaesarCipher("HELLO WORLD", 3));
    [Fact] public void Caesar_NegativeAndLargeShifts() =>
        Assert.Equal(Cipher.CaesarCipher("ABC", 3), Cipher.CaesarCipher("ABC", -23));
    [Fact] public void Keyword_ReadmeExample() => Assert.Equal("AOGGJ UJNGW", Cipher.KeywordCipher("HELLO WORLD", "KEYWORD"));
    [Fact] public void Giovanni_ReadmeExample() => Assert.Equal("RYCCH SHLCE", Cipher.GiovanniCipher("HELLO WORLD", "KEYWORD", "C"));
    [Fact] public void Giovanni_BadKeyLetterThrows() =>
        Assert.Throws<ArgumentException>(() => Cipher.GiovanniCipher("A", "K", "1"));
    [Fact] public void Transposition_ReadmeExample() => Assert.Equal("HLOOLELWRD", Cipher.TranspositionCipher("HELLO WORLD"));
}

public class StringConvertTests
{
    [Fact] public void Reverse() => Assert.Equal("olleh", StringConvert.Reverse("hello"));
    [Fact] public void Hex_Ascii() => Assert.Equal("68656C6C6F", StringConvert.ToHex("hello"));
    [Theory]
    [InlineData("hello")]
    [InlineData("héllo €")]
    public void Hex_RoundTrip(string s) => Assert.Equal(s, StringConvert.FromHex(StringConvert.ToHex(s)));
    [Theory]
    [InlineData("hello")]
    [InlineData("héllo €")]
    public void Binary_RoundTrip(string s) => Assert.Equal(s, StringConvert.FromBinary(StringConvert.ToBinary(s)));
    [Fact] public void Binary_Ascii() => Assert.Equal("0110100001100101011011000110110001101111", StringConvert.ToBinary("hello"));
    [Fact] public void FromBinary_BadLengthThrows() => Assert.Throws<FormatException>(() => StringConvert.FromBinary("0101"));
    [Fact] public void ToDouble_IsCultureInvariant()
    {
        var old = System.Globalization.CultureInfo.CurrentCulture;
        try
        {
            System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("de-DE");
            Assert.Equal(3.14, StringConvert.ToDouble("3.14"));
        }
        finally { System.Globalization.CultureInfo.CurrentCulture = old; }
    }
}

public class TextTests
{
    [Fact] public void Extracts() => Assert.Equal("Welcome User", Text.GetTextFromDoubleQuotations("The message is \"Welcome User\" today."));
    [Theory]
    [InlineData("none")]
    [InlineData("one \" quote")]
    public void TooFewQuotes(string s)
    {
        Assert.False(Text.TryGetTextFromDoubleQuotations(s, out _));
        Assert.Throws<FormatException>(() => Text.GetTextFromDoubleQuotations(s));
    }
}

public class CheckTests
{
    [Theory]
    [InlineData("+639171234567", true)]
    [InlineData("09171234567", true)]
    [InlineData("0917-123-4567", true)]
    [InlineData("0817123456", false)]
    [InlineData(null, false)]
    public void PhMobile(string? s, bool expected) => Assert.Equal(expected, Check.IsAValidPhilippineMobileNumber(s));

    [Fact] public void CharacterChecks()
    {
        Assert.True(Check.IsAllNumbers("12345"));
        Assert.False(Check.IsAllNumbers(""));
        Assert.True(Check.HasNumbers("abc1"));
        Assert.False(Check.HasNumbers(null));
        Assert.True(Check.IsAllSymbols("$$$"));
        Assert.True(Check.IsAllPunctuations("..."));
        Assert.True(Check.HasSpecialCharacters("Test#"));
        Assert.True(Check.IsAllSpaces("   "));
        Assert.True(Check.HasNoSpaces("NoSpacesHere"));
        Assert.False(Check.HasNoSpaces("a b"));
    }

    [Fact] public void TimeLeft() =>
        Assert.Equal(9000.0, Check.HowManySecondsLeft(new DateTime(2026, 1, 1, 0, 0, 0), new DateTime(2026, 1, 1, 2, 30, 0)));
}

public class EmailDomainValidatorTests
{
    [Fact] public void SplitMode()
    {
        var v = new EmailDomainValidator().AddDomainName("gmail").AddDomainExtension("com");
        Assert.True(v.IsValid("test@gmail.com"));
        Assert.True(v.IsValid("test@GMAIL.COM"));
        Assert.False(v.IsValid("test@gmail.org"));
        Assert.False(v.IsValid("a@b@gmail.com"));
        Assert.False(v.IsValid("@gmail.com"));
        Assert.False(v.IsValid(null));
    }

    [Fact] public void FullDomainMode()
    {
        var v = new EmailDomainValidator { UseFullDomain = true }.AddDomain("company.co.uk");
        Assert.True(v.IsValid("user@company.co.uk"));
        Assert.False(v.IsValid("user@other.co.uk"));
    }

    [Fact] public void MultiDotExtension()
    {
        var v = new EmailDomainValidator().AddDomainName("company").AddDomainExtension("co.uk");
        Assert.True(v.IsValid("user@company.co.uk"));
    }
}

public class AreaCalculatorTests
{
    [Fact] public void Basics()
    {
        Assert.Equal(16.0, AreaCalculator.SquareArea(4));
        Assert.Equal(78.5398, AreaCalculator.CircleArea(5), 4);
        Assert.Equal(13.0900, AreaCalculator.SectorArea(5, 60), 4);
        Assert.Equal(2.2647, AreaCalculator.SegmentArea(5, 60), 3);
        Assert.Equal(54.0, AreaCalculator.CubeSurfaceArea(3));
        Assert.Equal(113.0973, AreaCalculator.SphereSurfaceArea(3), 4);
    }

    [Fact] public void Shoelace()
    {
        var v = new List<(double X, double Y)> { (0, 0), (4, 0), (4, 3), (0, 3) };
        Assert.Equal(12.0, AreaCalculator.PolygonShoelaceArea(v));
        Assert.Equal(0.0, AreaCalculator.PolygonShoelaceArea(v.Take(2).ToList()));
    }
}

public class SortTests
{
    public static IEnumerable<object[]> Algorithms() => new (string, Func<int[], int[]>)[]
    {
        (nameof(Sort.BubbleSort), Sort.BubbleSort),
        (nameof(Sort.CocktailShakerSort), Sort.CocktailShakerSort),
        (nameof(Sort.OddEvenSort), Sort.OddEvenSort),
        (nameof(Sort.SelectionSort), Sort.SelectionSort),
        (nameof(Sort.InsertionSort), Sort.InsertionSort),
        (nameof(Sort.ShellSort), Sort.ShellSort),
        (nameof(Sort.QuickSort), Sort.QuickSort),
        (nameof(Sort.MergeSort), Sort.MergeSort),
        (nameof(Sort.HeapSort), Sort.HeapSort),
        (nameof(Sort.IntroSort), Sort.IntroSort),
        (nameof(Sort.FrameworkSort), Sort.FrameworkSort),
        (nameof(Sort.CountingSort), Sort.CountingSort),
        (nameof(Sort.PatienceSort), Sort.PatienceSort),
        (nameof(EducationalSort.TreeSort), EducationalSort.TreeSort),
        (nameof(EducationalSort.BeadSort), a => EducationalSort.BeadSort(a.Select(Math.Abs).ToArray())),
    }.Select(t => new object[] { t.Item1, t.Item2 });

    static int[] Data(int n, int seed, int lo, int hi)
    {
        var r = new Random(seed);
        return Enumerable.Range(0, n).Select(_ => r.Next(lo, hi)).ToArray();
    }

    [Theory, MemberData(nameof(Algorithms))]
    public void SortsCorrectlyAndDoesNotMutateInput(string name, Func<int[], int[]> sort)
    {
        // Non-negative so every algorithm (incl. bead sort) accepts it; small n keeps O(n^2) sorts fast.
        foreach (int[] input in new[]
        {
            Array.Empty<int>(), new[] { 1 }, new[] { 5, 2, 9, 1, 5, 6 },
            Data(300, 1, 0, 50), Data(300, 2, 0, 1000),
            Enumerable.Range(0, 200).ToArray(), Enumerable.Range(0, 200).Reverse().ToArray(),
            Enumerable.Repeat(7, 100).ToArray()
        })
        {
            int[] copy = (int[])input.Clone();
            int[] expected = (int[])input.Clone();
            Array.Sort(expected);

            Assert.True(expected.SequenceEqual(sort(input)), $"{name} produced wrong order");
            Assert.True(copy.SequenceEqual(input), $"{name} mutated its input");
        }
    }

    [Theory]
    [InlineData(nameof(Sort.QuickSort))]
    [InlineData(nameof(Sort.MergeSort))]
    [InlineData(nameof(Sort.HeapSort))]
    [InlineData(nameof(Sort.IntroSort))]
    [InlineData(nameof(Sort.CountingSort))]
    public void HandlesNegativesAndLargeInputs(string name)
    {
        Func<int[], int[]> sort = name switch
        {
            nameof(Sort.QuickSort) => Sort.QuickSort,
            nameof(Sort.MergeSort) => Sort.MergeSort,
            nameof(Sort.HeapSort) => Sort.HeapSort,
            nameof(Sort.IntroSort) => Sort.IntroSort,
            _ => Sort.CountingSort
        };
        int[] input = Data(100_000, 3, -5000, 5000);
        int[] expected = (int[])input.Clone();
        Array.Sort(expected);
        Assert.True(expected.SequenceEqual(sort(input)));
    }

    [Fact] public void CountingSort_RangeTooLargeThrows() =>
        Assert.Throws<ArgumentException>(() => Sort.CountingSort([int.MinValue, int.MaxValue]));

    [Fact] public void BucketSort_SortsAndValidates()
    {
        Assert.Equal(new[] { 0.12, 0.56, 0.65, 0.66, 0.89 }, Sort.BucketSortUniform([0.89, 0.56, 0.65, 0.12, 0.66]));
        Assert.Throws<ArgumentOutOfRangeException>(() => Sort.BucketSortUniform([0.5, 1.5]));
        Assert.Throws<ArgumentOutOfRangeException>(() => Sort.BucketSortUniform([double.NaN]));
    }

    [Fact] public void TreeSort_SortedInputDoesNotOverflowStack() =>
        Assert.Equal(10_000, EducationalSort.TreeSort(Enumerable.Range(0, 10_000).ToArray()).Length);

    [Fact] public void BogoSort_SortsSmallArrayAndHonoursCancellation()
    {
        Assert.Equal(new[] { 1, 2, 3, 4 }, EducationalSort.BogoSort([3, 1, 4, 2]));
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        Assert.Throws<OperationCanceledException>(() => EducationalSort.BogoSort([2, 1], cts.Token));
    }

    [Fact] public void BeadSort_RejectsNegatives() =>
        Assert.Throws<ArgumentException>(() => EducationalSort.BeadSort([1, -1]));
}

public class SimpleFileHandlerTests : IDisposable
{
    readonly string _dir = Path.Combine(Path.GetTempPath(), "inps-" + Guid.NewGuid().ToString("N"));
    public SimpleFileHandlerTests() => Directory.CreateDirectory(_dir);
    public void Dispose() { try { Directory.Delete(_dir, true); } catch { } }

    [Fact] public void ReadWriteAppend()
    {
        string f = Path.Combine(_dir, "a.txt");
        SimpleFileHandler.Write(f, "a");
        SimpleFileHandler.Append(f, "b");
        Assert.Equal("ab", SimpleFileHandler.Read(f));
    }

    [Fact] public void ExtractZipSafe_ExtractsNormalZip()
    {
        string zip = Path.Combine(_dir, "ok.zip");
        using (var fs = File.Create(zip))
        using (var za = new System.IO.Compression.ZipArchive(fs, System.IO.Compression.ZipArchiveMode.Create))
        using (var w = new StreamWriter(za.CreateEntry("sub/hello.txt").Open())) w.Write("hi");

        string target = Path.Combine(_dir, "out");
        SimpleFileHandler.ExtractZipSafe(zip, target);
        Assert.Equal("hi", File.ReadAllText(Path.Combine(target, "sub", "hello.txt")));
    }

    [Fact] public void ExtractZipSafe_BlocksZipSlip()
    {
        string zip = Path.Combine(_dir, "evil.zip");
        using (var fs = File.Create(zip))
        using (var za = new System.IO.Compression.ZipArchive(fs, System.IO.Compression.ZipArchiveMode.Create))
        using (var w = new StreamWriter(za.CreateEntry("../escaped.txt").Open())) w.Write("x");

        string target = Path.Combine(_dir, "out2");
        Assert.Throws<IOException>(() => SimpleFileHandler.ExtractZipSafe(zip, target));
        Assert.False(File.Exists(Path.Combine(_dir, "escaped.txt")));
    }
}
