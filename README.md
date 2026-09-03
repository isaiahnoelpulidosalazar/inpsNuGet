# inpsNuGet

A multi-purpose C# utility library providing helpers for threading, file I/O, validation, cryptography, data conversion, sorting algorithms, embedded Python execution, and Windows Forms UI components.

---

## Installation

```bash
dotnet add package inpsNuGet
```

---

## Namespaces & Modules Overview

```csharp
using inpsNuGet;
```

---

## 1. Actions (`inpsNuGet.Actions`)
Handles asynchronous task execution, external executable execution with output capture, and dedicated background thread management.

### Methods & Properties
- `Run()`: Runs an `Action` on `Task.Run()`.
- `RunExe(string filePath, params string[] args)`: Spawns an external process asynchronously, captures `stdout` / `stderr`, and prints exit status.
- `RunOnDedicatedThread(bool doInBackground = true)`: Executes an action on a dedicated `System.Threading.Thread`.
- `IsRunning`: Returns `true` if the underlying `Task` is currently active.

```csharp
// Run action asynchronously
var action = new Actions(() => Console.WriteLine("Running in background")).Run();

// Run external executable
var exeRunner = new Actions().RunExe("git.exe", "status");

// Run on a dedicated background thread
var threadAction = new Actions(() => DoHeavyWork()).RunOnDedicatedThread(DoInBackground: true);
```

---

## 2. Check (`inpsNuGet.Check`)
Validation helpers for strings, Philippine mobile numbers, email domains, time differences, and connectivity.

### Methods
- **Email Validation**:
  - `Check.Email.AddValidDomain(string domain)`
  - `Check.Email.AddValidDomainName(string name)`
  - `Check.Email.AddValidDomainExtension(string ext)`
  - `Check.Email.ShouldUseFullDomain(bool useFullDomain = true)`
  - `Check.Email.IsValid(string email)`: Validates email against configured domain whitelist.
- **String & Character Checks**:
  - `IsAValidPhilippineMobileNumber(string str)`: Validates format (`09...`, `+639...`, `639...`).
  - `IsAllNumbers(str)`, `HasNumbers(str)`
  - `IsAllAsciiNumbers(str)`, `HasAsciiNumbers(str)`
  - `IsAllSymbols(str)`, `HasSymbols(str)`
  - `IsAllPunctuations(str)`, `HasPunctuations(str)`
  - `IsAllSpecialCharacters(str)`, `HasSpecialCharacters(str)`
  - `IsAllSpaces(str)`, `HasSpaces(str)`, `HasNoSpaces(str)`
- **Date Differences**:
  - `HowManySecondsLeft(now, until)`, `HowManyMinutesLeft(now, until)`, `HowManyHoursLeft(now, until)`, `HowManyDaysLeft(now, until)`
- **Network**:
  - `CheckConnection()`: Tests internet reachability via Google 204 endpoint.

```csharp
// Mobile check
bool validMobile = Check.IsAValidPhilippineMobileNumber("+639171234567"); // true

// Email whitelist validation
Check.Email.AddValidDomain("example.com");
Check.Email.ShouldUseFullDomain(true);
bool validEmail = Check.Email.IsValid("user@example.com"); // true

// Time left
double daysLeft = Check.HowManyDaysLeft(DateTime.Now, new DateTime(2026, 12, 31));
```

---

## 3. Cipher (`inpsNuGet.Cipher`)
Implementation of classic cipher algorithms.

- `CaesarCipher(string text, int shift)`
- `KeywordCipher(string text, string keyword)`
- `GiovanniCipher(string text, string keyword, string keyLetter)`
- `TranspositionCipher(string text)`

```csharp
string caesar = Cipher.CaesarCipher("HELLO WORLD", 3);
string keyword = Cipher.KeywordCipher("HELLO WORLD", "SECRET");
string giovanni = Cipher.GiovanniCipher("HELLO WORLD", "SECRET", "K");
string trans = Cipher.TranspositionCipher("HELLO WORLD");
```

---

## 4. Convert (`inpsNuGet.Convert`)
Data transformations, encodings, and string utilities.

- `Reverse(string str)`
- `ToBase64(string str)` / `FromBase64(string str)`
- `ToByteArray(string str)` / `FromByteArray(byte[] array)`
- `ToHex(string str)` / `FromHex(string str)`
- `ToBinary(string str)` / `FromBinary(string str)`
- `ToInt(str)`, `ToDouble(str)`, `ToLong(str)`, `ToFloat(str)`

```csharp
string b64 = Convert.ToBase64("Sample");
string original = Convert.FromBase64(b64);
string hex = Convert.ToHex("ABC"); // "414243"
```

---

## 5. PyCS (`inpsNuGet.PyCS`)
Automated extractor, environment manager, and runner for embedded Python 3.13.

### Methods
- `new PyCS(bool console = true, string customDir = "")`: Initializes environment from embedded resources (`Python.zip`, `PythonFiles.zip`).
- `InstallPip()`: Bootstraps `pip` using `get-pip.py`.
- `Pip(string[] args)` / `PipUpgrade(string[] args)` / `PipLocal(string[] args)`: Installs dependencies.
- `Run(string script)` / `RunFile(string filePath)`: Executes script and prints stdout.
- `GetOutput(string script)` / `GetFileOutput(string filePath)`: Executes script and returns output as `string`.
- `Stop()`: Gracefully halts or kills the active Python sub-process.

```csharp
var py = new PyCS();
py.InstallPip();
py.Pip(new[] { "requests", "numpy" });

string output = py.GetOutput("import sys; print(sys.version)");
```

---

## 6. SimpleFileHandler (`inpsNuGet.SimpleFileHandler`)
File operations, embedded resource extraction, and zip archive handling.

- `Write(filePath, content)` / `Read(filePath)` / `Append(filePath, content)`
- `ExtractZipSafe(zipPath, extractPath)`: Extracts archives while handling relative paths.
- `ProjectToLocation(assembly, fileName, [filePath])`: Extracts embedded manifest resources to disk.
- `ProjectToLocationThenExtractZip(assembly, fileName, [filePath])`
- `ProjectToLocationThenExtractZipThenDelete(assembly, fileName, [filePath])`

```csharp
SimpleFileHandler.Write("log.txt", "Initial log\n");
SimpleFileHandler.Append("log.txt", "Appended line");

// Extract embedded assembly resource to disk
SimpleFileHandler.ProjectToLocation(Assembly.GetExecutingAssembly(), "config.json", @"C:\AppConfig");
```

---

## 7. Sort (`inpsNuGet.Sort`)
Standard and non-standard sorting algorithm implementations.

| Method | Target Type | Complexity (Average) |
|---|---|---|
| `BubbleSort(arr)` | `int[]` | O(n²) |
| `CocktailShakerSort(arr)` | `int[]` | O(n²) |
| `OddEvenSort(arr)` | `int[]` | O(n²) |
| `SelectionSort(arr)` | `int[]` | O(n²) |
| `InsertionSort(arr)` | `int[]` | O(n²) |
| `ShellSort(arr)` | `int[]` | O(n log n) |
| `QuickSort(arr)` | `int[]` | O(n log n) |
| `MergeSort(arr)` | `int[]` | O(n log n) |
| `HeapSort(arr)` | `int[]` | O(n log n) |
| `IntroSort(arr)` | `int[]` | O(n log n) |
| `TimSort(arr)` | `int[]` | O(n log n) |
| `CountingSort(arr)` | `int[]` (non-negative) | O(n + k) |
| `BucketSortUniform(arr)` | `double[]` (0.0 - 1.0) | O(n + k) |
| `PigeonholeSort(arr)` | `int[]` | O(n + Range) |
| `TreeSort(arr)` | `int[]` | O(n log n) |
| `PatienceSorting(arr)` | `int[]` | O(n log n) |
| `BeadSort(arr)` | `int[]` (positive only) | O(S) |
| `BogoSort(arr)` | `int[]` | O((n+1)!) |

```csharp
int[] data = { 5, 3, 8, 4, 2 };
int[] sorted = Sort.QuickSort(data);
```

---

## 8. Text (`inpsNuGet.Text`)
Text parsing helper.

- `GetTextFromDoubleQuotations(string line)`: Extracts substring enclosed within double quotes (`"..."`).

```csharp
string extracted = Text.GetTextFromDoubleQuotations("Message: \"Hello World\"");
// extracted = "Hello World"
```

---

## 9. UI Components (`Windows Forms`)
*Requires Windows desktop compilation target.*

### `ClickableElement`
A custom selectable, toggleable panel item with hover, active, and click animations.
- `SetEvent(Action event)`: Sets click callback.
- `Toggle()`: Manually toggles state and updates UI colors.
- `IsToggled()`: Checks toggle state.
- `GetTitle()`: Returns label text.

### `VerticalList`
A double-buffered vertical flow control that automatically resizes child elements and autoscrolls to the newest item.
- `AddItem(string title)`
- `AddItem(string title, Action event)`
- `AddItem(ClickableElement element)`
- `ScrollToBottom()`

```csharp
var list = new VerticalList();

list.AddItem("Option 1", () => MessageBox.Show("Option 1 Selected"));
list.AddItem("Option 2", () => MessageBox.Show("Option 2 Selected"));

this.Controls.Add(list);
```

---

## License
MIT