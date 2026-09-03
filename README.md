# inpsNuGet

`inpsNuGet` is a multipurpose .NET utility library providing embedded Python runtime management, threading/process executors, Windows Forms UI controls, sorting algorithms, text ciphers, data conversion helpers, and file system handlers.

**Namespace:** `inpsNuGet`

---

## Installation

### .NET CLI
```bash
dotnet add package inpsNuGet
```

### Package Manager
```powershell
Install-Package inpsNuGet
```

---

## Table of Contents
- [Python Runtime Integration (`PyCS`)](#python-runtime-integration-pycs)
- [Threading & Execution (`Actions`)](#threading--execution-actions)
- [UI Components (Windows Forms)](#ui-components-windows-forms)
  - [VerticalList](#verticallist)
  - [ClickableElement](#clickableelement)
- [Algorithms & Cryptography](#algorithms--cryptography)
  - [Sort](#sort)
  - [Cipher](#cipher)
- [Validation & Network (`Check`)](#validation--network-check)
- [Data Conversion & Formatting (`Convert`, `Text`)](#data-conversion--formatting-convert-text)
- [File & Embedded Resource I/O (`SimpleFileHandler`)](#file--embedded-resource-io-simplefilehandler)

---

## Python Runtime Integration (`PyCS`)

`PyCS` deploys and manages an embedded Python 3.13 runtime directly within a .NET host application, providing pip package management and script execution.

```csharp
using inpsNuGet;

// Initialize embedded Python runtime (default extracted to "Python" folder)
PyCS py = new PyCS(console: true);

// Or specify a custom deployment directory
// PyCS py = new PyCS(console: true, customDir: @"C:\CustomPython");

// Install Pip & Packages
py.InstallPip();
py.Pip(new string[] { "requests", "numpy" });
py.PipUpgrade(new string[] { "requests" });

// Execute Python Script & Print to Console
py.Run("print('Hello from Embedded Python!')");

// Execute Python Script File
py.RunFile("script.py");

// Execute and Capture Stdout/Stderr as a string
string result = py.GetOutput("import math; print(math.sqrt(144))");

// Terminate running Python process
py.Stop();
```

---

## Threading & Execution (`Actions`)

Provides a task and thread orchestration wrapper for running delegate actions, background worker threads, and external executables.

```csharp
using inpsNuGet;

// 1. Run action on Task ThreadPool
Actions taskAction = new Actions(() =>
{
    Console.WriteLine("Executing on ThreadPool task...");
}).Run();

// Check status
bool running = taskAction.IsRunning;

// 2. Run action on a dedicated System.Threading.Thread
Actions threadAction = new Actions(() =>
{
    Console.WriteLine("Executing on dedicated thread...");
}).RunOnDedicatedThread(DoInBackground: true);

// 3. Execute external process asynchronously with argument handling
Actions exeAction = new Actions();
exeAction.RunExe("git.exe", "status", "--short");
```

---

## UI Components (Windows Forms)

> *Target OS: Windows Forms (`net8.0-windows` / `net9.0-windows`)*

### VerticalList & ClickableElement

`VerticalList` is a custom `FlowLayoutPanel` managing dynamically resized, selectable button items (`ClickableElement`) with custom hover, click, and toggle state rendering.

```csharp
using System.Windows.Forms;
using inpsNuGet;

public class MainForm : Form
{
    public MainForm()
    {
        VerticalList list = new VerticalList
        {
            Dock = DockStyle.Fill
        };

        // Add text item
        list.AddItem("Item 1");

        // Add item with click callback
        list.AddItem("Item 2 with Action", () =>
        {
            MessageBox.Show("Item 2 clicked!");
        });

        // Add custom configured ClickableElement
        ClickableElement customElement = new ClickableElement("Toggleable Item");
        customElement.SetEvent(() =>
        {
            customElement.Toggle();
            bool isToggled = customElement.IsToggled();
        });

        list.AddItem(customElement);
        Controls.Add(list);
    }
}
```

---

## Algorithms & Cryptography

### Sort

A collection of sorting algorithms operating on primitive arrays.

| Algorithm | Method Signature | Behavior |
| :--- | :--- | :--- |
| **Bubble Sort** | `Sort.BubbleSort(int[] arr)` | In-place |
| **Cocktail Shaker Sort** | `Sort.CocktailShakerSort(int[] arr)` | In-place |
| **Odd-Even Sort** | `Sort.OddEvenSort(int[] arr)` | In-place |
| **Selection Sort** | `Sort.SelectionSort(int[] arr)` | In-place |
| **Insertion Sort** | `Sort.InsertionSort(int[] arr)` | In-place |
| **Shell Sort** | `Sort.ShellSort(int[] arr)` | In-place |
| **Quick Sort** | `Sort.QuickSort(int[] arr)` | Returns new array |
| **Merge Sort** | `Sort.MergeSort(int[] arr)` | Returns new array |
| **Heap Sort** | `Sort.HeapSort(int[] arr)` | Priority queue backed |
| **Intro Sort** | `Sort.IntroSort(int[] arr)` | Hybrid Quicksort / Heapsort |
| **Tim Sort** | `Sort.TimSort(int[] arr)` | Array.Sort clone |
| **Counting Sort** | `Sort.CountingSort(int[] arr)` | Non-negative integers |
| **Bucket Sort** | `Sort.BucketSortUniform(double[] arr)` | Values in range $[0.0, 1.0)$ |
| **Pigeonhole Sort** | `Sort.PigeonholeSort(int[] arr)` | Integer range mapping |
| **Tree Sort** | `Sort.TreeSort(int[] arr)` | Binary search tree traversal |
| **Patience Sorting** | `Sort.PatienceSorting(int[] arr)` | Pile sort via binary search |
| **Bogo Sort** | `Sort.BogoSort(int[] arr)` | Randomized shuffle sort |
| **Bead Sort** | `Sort.BeadSort(int[] arr)` | Gravity sort (non-negative integers) |

```csharp
int[] numbers = new int[] { 5, 2, 9, 1, 5, 6 };
int[] sorted = Sort.QuickSort(numbers);

double[] doubles = new double[] { 0.89, 0.56, 0.65, 0.12, 0.66 };
double[] sortedDoubles = Sort.BucketSortUniform(doubles);
```

---

### Cipher

Classical text encryption ciphers for uppercase Latin alphabetic characters.

```csharp
using inpsNuGet;

// Caesar Cipher
string caesar = Cipher.CaesarCipher("HELLO WORLD", 3);
// Output: "KHOOR ZRUOG"

// Keyword Substitution Cipher
string keyword = Cipher.KeywordCipher("HELLO WORLD", "KEYWORD");
// Output: "AOGGJ UJNGW"

// Giovanni Cipher (Rotated Keyword Cipher)
string giovanni = Cipher.GiovanniCipher("HELLO WORLD", "KEYWORD", "C");
// Output: "RYCCH SHLCE"

// Transposition Cipher (Interleaves even/odd characters, strips spaces)
string transposed = Cipher.TranspositionCipher("HELLO WORLD");
// Output: "HLOOLELWRD"
```

---

## Validation & Network (`Check`)

```csharp
using inpsNuGet;

// --- Email Validation ---
// Split Mode (Name and Extension configured separately)
Check.Email.AddValidDomainName("gmail");
Check.Email.AddValidDomainExtension("com");
Check.Email.ShouldUseFullDomain(false);
bool isValidSplit = Check.Email.IsValid("test@gmail.com"); // true

// Full Domain Mode
Check.Email.AddValidDomain("company.co.uk");
Check.Email.ShouldUseFullDomain(true);
bool isValidFull = Check.Email.IsValid("user@company.co.uk"); // true


// --- Pattern & Character Checks ---
Check.IsAValidPhilippineMobileNumber("+639171234567"); // true
Check.IsAValidPhilippineMobileNumber("09171234567");    // true

Check.IsAllNumbers("12345");          // true
Check.HasNumbers("abc1");             // true
Check.IsAllAsciiNumbers("123");       // true
Check.HasAsciiNumbers("a1");          // true

Check.IsAllSymbols("$$$");            // true
Check.HasSymbols("price: $5");        // true
Check.IsAllPunctuations("...");       // true
Check.HasPunctuations("Hello!");      // true
Check.IsAllSpecialCharacters("@#$!"); // true
Check.HasSpecialCharacters("Test#");  // true

Check.IsAllSpaces("   ");             // true
Check.HasSpaces("Hello World");       // true
Check.HasNoSpaces("NoSpacesHere");    // true


// --- Time Remaining Calculations ---
DateTime now = DateTime.Now;
DateTime target = now.AddHours(2.5);

double sec  = Check.HowManySecondsLeft(now, target); // ~9000.0
double min  = Check.HowManyMinutesLeft(now, target); // ~150.0
double hrs  = Check.HowManyHoursLeft(now, target);   // ~2.5
double days = Check.HowManyDaysLeft(now, target);    // ~0.104


// --- Connectivity Check ---
// Sends a request to Google generate_204 endpoint and logs status to console
Check.CheckConnection();
```

---

## Data Conversion & Formatting (`Convert`, `Text`)

### Convert
```csharp
using inpsNuGet;

// String Manipulation
string reversed = Convert.Reverse("hello"); // "olleh"

// Base64
string b64 = Convert.ToBase64("Sample");
string fromB64 = Convert.FromBase64(b64);

// Byte Array
byte[] bytes = Convert.ToByteArray("Sample");
string fromBytes = Convert.FromByteArray(bytes);

// Hexadecimal
string hex = Convert.ToHex("hello"); // "68656C6C6F"
string fromHex = Convert.FromHex(hex); // "hello"

// Binary
string bin = Convert.ToBinary("hello"); // "0110100001100101011011000110110001101111"
string fromBin = Convert.FromBinary(bin); // "hello"

// Numeric Parsing
int iVal    = Convert.ToInt("42");
double dVal = Convert.ToDouble("3.1415");
long lVal   = Convert.ToLong("9999999999");
float fVal  = Convert.ToFloat("1.25");
```

### Text
```csharp
using inpsNuGet;

// Extracts substring contained between double quotation marks
string extracted = Text.GetTextFromDoubleQuotations("The message is \"Welcome User\" today.");
// Output: "Welcome User"
```

---

## File & Embedded Resource I/O (`SimpleFileHandler`)

Utilities for file access, safe zip extraction, and extracting embedded assembly resources to disk.

```csharp
using System.Reflection;
using inpsNuGet;

// Basic File I/O
SimpleFileHandler.Write("output.txt", "Initial content");
SimpleFileHandler.Append("output.txt", "\nAppended content");
string content = SimpleFileHandler.Read("output.txt");

// Safe Zip Extraction (with automatic folder creation and path traversal protection)
SimpleFileHandler.ExtractZipSafe("archive.zip", @"C:\ExtractedFiles");

// --- Embedded Resource Projection ---
Assembly assembly = Assembly.GetExecutingAssembly();

// 1. Extract embedded resource matching file name to target path
SimpleFileHandler.ProjectToLocation(assembly, "config.json", @"C:\AppConfig");

// 2. Extract embedded zip resource, extract contents, then delete original zip
SimpleFileHandler.ProjectToLocationThenExtractZipThenDelete(assembly, "payload.zip", @"C:\AppPayload");
```