# inpsNuGet

`inpsNuGet` is a multipurpose .NET utility library: an embedded Python runtime manager, process/thread helpers, Windows Forms controls, sorting algorithms, text ciphers, conversions, validation helpers and geometry formulas.

**Namespace:** `inpsNuGet` · **Targets:** `net8.0`, `net8.0-windows` · **Version:** 2.x

## Installation

```bash
dotnet add package inpsNuGet
```

## Python runtime (`PyCS`, Windows only)

The embedded runtime is extracted lazily on first use. Failures throw exceptions; results come back as `ProcessResult`.

```csharp
using inpsNuGet;

var py = new PyCS(log: Console.Out);               // extracts to ./Python
// var py = new PyCS(@"C:\CustomPython", Console.Out);

py.InstallPip();
py.Pip("requests", "numpy");
py.PipUpgrade("requests");
py.PipLocal(@"C:\wheels", "mypackage");            // offline install

ProcessResult r = py.Run("print('Hello from Embedded Python!')");
Console.WriteLine(r.StandardOutput);
string text = py.GetOutput("import math; print(math.sqrt(144))");
py.RunFile("script.py");
py.Stop();                                          // kill the running process tree
```

`py.TrustPypiHosts = true` adds `--trusted-host` for PyPI. It disables TLS verification for those hosts, so only use it behind a TLS-intercepting proxy.

## Threading and processes (`Actions`)

```csharp
new Actions(() => Console.WriteLine("thread pool")).Run();
new Actions(() => Console.WriteLine("own thread")).RunOnDedicatedThread(doInBackground: true);

// Run an executable and get the result (nothing is written to the console):
ProcessResult result = await Actions.ExecuteAsync("git", ["status", "--short"]);
Console.WriteLine($"{result.ExitCode}: {result.StandardOutput}");

// Or fire and track:
var a = new Actions().RunExe("git", "status");
await a.RunningTask!;
```

## Windows Forms controls (`net8.0-windows` only)

```csharp
var list = new VerticalList { Dock = DockStyle.Fill };
list.AddItem("Item 1");
list.AddItem("Item 2", () => MessageBox.Show("clicked"));

var toggle = new ClickableElement("Toggleable");
toggle.SetEvent(() => toggle.Toggle());
list.AddItem(toggle);
Controls.Add(list);
```

## Ciphers

Output is upper-case; non-letters pass through.

```csharp
Cipher.CaesarCipher("HELLO WORLD", 3);                       // "KHOOR ZRUOG" (negative shifts work too)
Cipher.KeywordCipher("HELLO WORLD", "KEYWORD");              // "AOGGJ UJNGW"
Cipher.GiovanniCipher("HELLO WORLD", "KEYWORD", "C");        // "RYCCH SHLCE"
Cipher.TranspositionCipher("HELLO WORLD");                   // "HLOOLELWRD"
```

## Sorting

**Every method returns a new array and never modifies its input.**

| `Sort` method | Notes |
| :--- | :--- |
| `BubbleSort`, `CocktailShakerSort`, `OddEvenSort`, `SelectionSort`, `InsertionSort`, `ShellSort` | simple / quadratic-ish |
| `QuickSort` | in-place 3-way, median-of-three |
| `MergeSort` | stable |
| `HeapSort` | in-place heap |
| `IntroSort` | quicksort + heapsort fallback + insertion sort |
| `FrameworkSort` | `Array.Sort` on a copy |
| `CountingSort` | negatives OK; value range capped at 2^24 |
| `BucketSortUniform(double[])` | values in [0, 1) |
| `PatienceSort` | piles + k-way merge |

`EducationalSort`: `BogoSort(arr, cancellationToken)`, `BeadSort` (non-negative), `TreeSort` (unbalanced BST).

```csharp
int[] sorted = Sort.QuickSort([5, 2, 9, 1, 5, 6]);
```

## Validation (`Check`, `EmailDomainValidator`)

```csharp
var emails = new EmailDomainValidator()
    .AddDomainName("gmail").AddDomainExtension("com");
emails.IsValid("test@gmail.com");                  // true (case-insensitive)

var full = new EmailDomainValidator { UseFullDomain = true }.AddDomain("company.co.uk");
full.IsValid("user@company.co.uk");                // true

Check.IsAValidPhilippineMobileNumber("+639171234567");
Check.IsAllNumbers("12345"); Check.HasSymbols("price: $5"); Check.HasNoSpaces("abc");
Check.HowManyHoursLeft(DateTime.Now, DateTime.Now.AddHours(2.5));

bool online = await Check.HasInternetAsync();      // true only on HTTP 204
```

## Conversions (`StringConvert`, `Text`)

```csharp
StringConvert.Reverse("hello");                     // "olleh"
StringConvert.ToBase64("Sample");  StringConvert.FromBase64(b64);
StringConvert.ToHex("hello");                       // "68656C6C6F" (UTF-8 bytes)
StringConvert.ToBinary("hello");                    // 8 bits per UTF-8 byte
StringConvert.ToDouble("3.1415");                   // invariant culture

Text.GetTextFromDoubleQuotations("say \"hi\" now"); // "hi" (throws FormatException if no pair of quotes)
Text.TryGetTextFromDoubleQuotations(line, out var inner);
```

## Geometry (`AreaCalculator`)

```csharp
AreaCalculator.CircleArea(5.0);                     // 78.54
AreaCalculator.PolygonShoelaceArea([(0, 0), (4, 0), (4, 3), (0, 3)]); // 12.0
AreaCalculator.SphereSurfaceArea(3.0);              // 113.10
```

## Files and embedded resources (`SimpleFileHandler`)

```csharp
SimpleFileHandler.Write("o.txt", "x"); SimpleFileHandler.Append("o.txt", "y"); SimpleFileHandler.Read("o.txt");

// Throws IOException on Zip Slip entries
SimpleFileHandler.ExtractZipSafe("archive.zip", @"C:\Out");

var asm = Assembly.GetExecutingAssembly();
SimpleFileHandler.ProjectToLocation(asm, "config.json", @"C:\AppConfig");
SimpleFileHandler.ExtractEmbeddedZip(asm, "payload.zip", @"C:\AppPayload"); // no temp file
```
