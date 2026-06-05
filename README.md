# inpsNuGet

**inpsNuGet** A NuGet package for .NET that simplifies validation through a custom Check class, a custom Convert class for data conversion, and a dedicated class that simplifies file IO through SimpleFileHandler.

---

## Features

The package operates under the `nugetpackage` namespace and contains several distinct modules to help keep your codebase clean and readable:

### Validation (`Check`)
A robust validation class to simplify standard string, format, and date checks.
- **Email**: Highly customizable email domain validation (allows whitelisting specific domain names, extensions, or full domains).
- **Phone Numbers**: Validates Philippine mobile numbers (`09`, `+639`, `639`).
- **Strings**: Check for spaces, symbols, pure numerical strings, or presence of numbers.
- **Time/Dates**: Easily calculate the remaining time between two `DateTime` objects (Days, Hours, Minutes, Seconds).

### Cryptography (`Cipher`)
A class to implement classic and recreational cipher techniques for string encryption.
- Transposition Cipher
- Giovanni Cipher
- Keyword Cipher
- Caesar Cipher

### Data Conversion (`Convert`)
Effortless type casting, text manipulation, and data encoding.
- Base64, Hex, and Binary encoding/decoding.
- String reversal and Byte-array conversions (`UTF8`).
- Quick string parsing to `Int`, `Double`, `Long`, and `Float`.

### File & Resource Handling (`SimpleFileHandler`)
Static methods to rapidly read, write, and extract files.
- Rapid `Read()`, `Write()`, and `Append()` for text files.
- **Embedded Resources**: Use `ProjectToLocation()` to easily extract and copy files marked as 'Embedded Resource' from your executing assembly to a physical directory.

### Sorting Algorithms (`SortAlgorithms`)
A massive suite of sorting algorithms implemented in C# as quick plug-and-play functions for integer and double arrays.
- **Standard**: Quick Sort, Merge Sort, Heap Sort, Selection Sort, Insertion Sort, Bubble Sort.
- **Advanced/Niche**: Tim Sort, Intro Sort, Cocktail Shaker Sort, Shell Sort, Pigeonhole Sort, Bead Sort, Patience Sorting, and even Bogo Sort!

---

## Installation

Install the package via the .NET CLI:

```bash
dotnet add package inpsNuGet
```

Or via the NuGet Package Manager Console:

```powershell
Install-Package inpsNuGet
```

---

## Quick Usage Examples

To use the package, simply include the namespace at the top of your file:
```csharp
using nugetpackage;
```

### 1. Validating Phone Numbers & Dates (`Check`)
```csharp
// Validate Philippine Phone Numbers
bool isValidPhone = Check.IsAValidPhilippineMobileNumber("+639123456789");
Console.WriteLine(isValidPhone); // True

// Calculate time left
DateTime now = DateTime.Now;
DateTime future = now.AddDays(5).AddHours(2);
Console.WriteLine($"Days left: {Check.HowManyDaysLeft(now, future)}"); // ~5.083
```

### 2. Custom Email Validation (`Check.Email`)
```csharp
// Setup strict domain rules
Check.Email.AddValidDomainName("gmail");
Check.Email.AddValidDomainExtension("com");

bool isEmailValid = Check.Email.IsValid("user@gmail.com");
Console.WriteLine(isEmailValid); // True
```

### 3. File Handling & Resource Extraction (`SimpleFileHandler`)
```csharp
// Write, Append, and Read text
SimpleFileHandler.Write("log.txt", "Process started.\n");
SimpleFileHandler.Append("log.txt", "Process finished.\n");
Console.WriteLine(SimpleFileHandler.Read("log.txt"));

// Extract an Embedded Resource to a physical location
Assembly myAssembly = Assembly.GetExecutingAssembly();
SimpleFileHandler.ProjectToLocation(myAssembly, "MyConfig.json", @"C:\AppConfigs");
```

### 4. Text Encryption (`Cipher`)
```csharp
string encrypted = Cipher.CaesarCipher("HELLO WORLD", shift: 3);
Console.WriteLine(encrypted); // KHOOR ZRUOG
```

### 5. String & Data Conversions (`Convert`)
```csharp
// String Reversal
string reversed = Convert.Reverse("C# is awesome");

// Base64 Encoding
string encoded = Convert.ToBase64("Secret Message");
string decoded = Convert.FromBase64(encoded);

// Hex Conversions
string hexValue = Convert.ToHex("Hello");
Console.WriteLine(hexValue); // 48656C6C6F
```

### 6. Sorting Arrays (`SortAlgorithms`)
```csharp
int[] array = { 5, 2, 9, 1, 5, 6 };

// Using QuickSort
int[] sortedArray = SortAlgorithms.QuickSort(array);

// Using more exotic sorts
int[] introSorted = SortAlgorithms.IntroSort(array);

Console.WriteLine(string.Join(", ", sortedArray)); // 1, 2, 5, 5, 6, 9
```

---

## Contributing
Pull requests are welcome! For major changes, please open an issue first to discuss what you would like to change. 

## License
[MIT](https://choosealicense.com/licenses/mit/) 