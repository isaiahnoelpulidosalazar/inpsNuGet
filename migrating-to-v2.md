# Migrating to inpsNuGet v2

v2.0.0 is a clean-break release. There are no `[Obsolete]` shims, so the compiler will flag every spot that needs a change. This guide lists each one.

**General changes that affect many call sites**

- Utility classes are now `static`. `new Cipher()`, `new Sort()` and similar no longer compile (they were never useful).
- Public parameter names are camelCase (`Str` → `str`/`s`, `FilePath` → `filePath`, `DoInBackground` → `doInBackground`). Call sites using **named arguments** with the old names must be updated. Positional calls are unaffected.
- Library code no longer writes to `Console` and no longer swallows errors. Failures throw exceptions. Wrap calls in `try/catch` where v1 printed a message and carried on.
- Build output no longer contains empty `ClickableElement` / `VerticalList` stub classes on the plain `net8.0` target. Those types exist only on `net8.0-windows`.
- The `Changes` class is gone (release notes live in `CHANGELOG.md`). `LICENSE` is no longer an embedded resource.

---

## 1. `Convert` → `StringConvert`

`inpsNuGet.Convert` clashed with `System.Convert`. It is now `StringConvert`.

```csharp
// v1
Convert.ToHex("hello");
// v2
StringConvert.ToHex("hello");
```

Method names are unchanged: `Reverse`, `ToBase64`, `FromBase64`, `ToByteArray`, `FromByteArray`, `ToHex`, `FromHex`, `ToBinary`, `FromBinary`, `ToInt`, `ToLong`, `ToFloat`, `ToDouble`.

Behavior changes:

| Method | v1 | v2 |
| --- | --- | --- |
| `ToHex`/`FromHex`, `ToBinary`/`FromBinary` | worked on UTF-16 chars; broke on non-ASCII | work on UTF-8 bytes; identical for ASCII, correct round trip for everything |
| `FromHex`/`FromBinary` | bad input could throw arbitrary errors | `FormatException` for invalid digits or lengths |
| `ToInt/ToLong/ToFloat/ToDouble` | used the current culture | use `CultureInfo.InvariantCulture` (`"3.14"` always parses) |

If you need the old culture-dependent parsing, call `int.Parse(s)` etc. directly.

## 2. `Check`

### Email validation: `Check.Email` → `EmailDomainValidator`

The static, process-wide validator is now an instance you create and own.

```csharp
// v1
Check.Email.AddValidDomainName("gmail");
Check.Email.AddValidDomainExtension("com");
Check.Email.ShouldUseFullDomain(false);
bool ok = Check.Email.IsValid("test@gmail.com");

Check.Email.AddValidDomain("company.co.uk");
Check.Email.ShouldUseFullDomain(true);

// v2
var emails = new EmailDomainValidator()
    .AddDomainName("gmail")
    .AddDomainExtension("com");
bool ok = emails.IsValid("test@gmail.com");

var full = new EmailDomainValidator { UseFullDomain = true }
    .AddDomain("company.co.uk");
```

| v1 | v2 |
| --- | --- |
| `Check.Email.AddValidDomainName(s)` | `validator.AddDomainName(s)` |
| `Check.Email.AddValidDomainExtension(s)` | `validator.AddDomainExtension(s)` |
| `Check.Email.AddValidDomain(s)` | `validator.AddDomain(s)` |
| `Check.Email.ShouldUseFullDomain()` / `(bool)` | `validator.UseFullDomain = true/false` |
| `Check.Email.IsValid(s)` | `validator.IsValid(s)` |

Behavior changes:

- Matching is **case-insensitive**.
- Exactly one `@` and a non-empty local part are required (`a@b@gmail.com` is now invalid).
- In name/extension mode, the extension is **everything after the first dot**, so `company.co.uk` has name `company` and extension `co.uk`. In v1 it was read as name `company`, extension `co`.
- Configuration is no longer shared across the whole process. Store the validator in a field or register it as a singleton in DI.

### Connectivity: `Check.CheckConnection()` → `await Check.HasInternetAsync()`

```csharp
// v1: printed a message to the console
Check.CheckConnection();

// v2: returns a bool
bool online = await Check.HasInternetAsync();
```

It uses HTTPS and returns `true` only for HTTP 204, so captive portals no longer count as "online". Pass a `CancellationToken` if you need cancellation. A sync wrapper is intentionally not provided (it risks deadlocks); use `.GetAwaiter().GetResult()` from a thread-pool thread only if you must.

### Everything else in `Check`

Pattern checks (`IsAllNumbers`, `HasSymbols`, …), `IsAValidPhilippineMobileNumber` and the `HowMany…Left` methods keep their names and results. They now accept `null` safely. The phone check uses `[0-9]` rather than `\d`, so non-ASCII digits no longer match.

## 3. `Cipher`

Signatures are unchanged. Behavior differences:

- `CaesarCipher` accepts negative and >26 shifts (v1 threw).
- `GiovanniCipher` throws `ArgumentException` for a non-letter `keyLetter` (v1: `ArgumentOutOfRangeException`/garbage). Non-letters in `keyword` are ignored instead of corrupting the alphabet.
- Upper-casing is culture-invariant (fixes the Turkish-`i` problem).

## 4. `Text`

`GetTextFromDoubleQuotations` now throws `FormatException` (v1: `ArgumentOutOfRangeException`) when there aren't two quote characters. New: `Text.TryGetTextFromDoubleQuotations(line, out result)`.

## 5. `SimpleFileHandler`

| v1 | v2 |
| --- | --- |
| `ProjectToLocation(asm, name)` | `ProjectToLocation(asm, name)` (target directory defaults to the current directory) |
| `ProjectToLocation(asm, name, dir)` | `ProjectToLocation(asm, name, dir)` |
| `ProjectToLocationThenExtractZip(asm, name[, dir])` | `ExtractEmbeddedZip(asm, name[, dir])` |
| `ProjectToLocationThenExtractZipThenDelete(asm, name[, dir])` | `ExtractEmbeddedZip(asm, name[, dir])` (no temp zip is written, so there is nothing to delete) |
| `ExtractZipSafe(zip, dir)` | `ExtractZipSafe(zip, dir)` |

Behavior changes:

- `ExtractZipSafe` is now actually safe: an entry that resolves outside the target folder throws `IOException` (Zip Slip). v1 only claimed to protect against this.
- Missing embedded resources throw `FileNotFoundException` (v1 printed a message and continued, sometimes creating an empty file).
- All I/O errors propagate instead of being printed.

## 6. `Actions`

```csharp
// v1
new Actions(...).RunOnDedicatedThread(DoInBackground: false);
await new Actions().ExeRunner("git", "status");   // printed exit code/output to the console

// v2
new Actions(...).RunOnDedicatedThread(doInBackground: false);
ProcessResult r = await Actions.ExecuteAsync("git", ["status"]);
// r.ExitCode, r.StandardOutput, r.StandardError, r.Succeeded
```

- `ExeRunner` is removed; use the static `Actions.ExecuteAsync` (supports `CancellationToken`, kills the process tree on cancel).
- `RunExe` no longer prints. Await `RunningTask` to wait for it; it faults if the process can't start.
- `RunOnDedicatedThread`'s task no longer runs continuations on the dedicated thread.
- The class is `sealed`.

## 7. `PyCS`

```csharp
// v1
var py = new PyCS(true, @"C:\CustomPython");
py.Pip(new[] { "requests", "numpy" });
py.PipLocal(new[] { "pkg" });          // hard-coded --find-links /
py.Run("print(1)");                     // printed to console

// v2
var py = new PyCS(@"C:\CustomPython", log: Console.Out);
py.Pip("requests", "numpy");           // params string[]; arrays still work
py.PipLocal(@"C:\wheels", "pkg");      // directory is now an explicit parameter
ProcessResult r = py.Run("print(1)");
```

| v1 | v2 |
| --- | --- |
| `PyCS()`, `PyCS(bool)`, `PyCS(string)`, `PyCS(bool, string)` | `PyCS(string? directory = null, TextWriter? log = null)`. `console: true` becomes `log: Console.Out`; `console: false` becomes `log: null` |
| Extraction in the constructor | Lazy. First method call extracts, or call `Initialize()` yourself |
| `Pip(string[])`, `PipUpgrade(string[])` | `Pip(params string[])`, `PipUpgrade(params string[])`, now returning `ProcessResult` |
| `PipLocal(string[])` | `PipLocal(string findLinksDirectory, params string[] packages)` |
| `Run`/`RunFile` printed output (void) | return `ProcessResult` (write it yourself, or use `.Combined`) |
| `GetOutput`/`GetFileOutput` | unchanged signature; returns the same "stdout + `Error Output:` + stderr" text |
| `InstallPip()` (void) | returns `ProcessResult?` (null when pip was already installed) |
| `main.py` in the Python folder | unique temp file per call (safe for concurrent use) |
| `--trusted-host` always on | **off by default.** Set `py.TrustPypiHosts = true` to restore it (disables TLS verification for PyPI hosts) |
| Errors returned as `"Execution failed: ..."` strings | exceptions (`Win32Exception` when python can't start, `FileNotFoundException` for missing resources, …) |

Also: `Stop()` now kills the whole process tree, and `PyCS` is `sealed` and marked `[SupportedOSPlatform("windows")]` (analyzer warning CA1416 on other platforms).

The package still expects `Python.zip` and `PythonFiles.zip` embedded in the assembly.

## 8. `Sort`

**Contract change:** every method returns a **new** array and leaves its input untouched. In v1 `BubbleSort`, `CocktailShakerSort`, `OddEvenSort`, `SelectionSort`, `InsertionSort`, `ShellSort` and `BogoSort` sorted the input in place (and returned it). If you relied on that, use the return value:

```csharp
// v1 (in place)
Sort.BubbleSort(numbers);
// v2
numbers = Sort.BubbleSort(numbers);
```

| v1 | v2 |
| --- | --- |
| `Sort.TimSort` | `Sort.FrameworkSort` (it was always just `Array.Sort`) |
| `Sort.PigeonholeSort` | `Sort.CountingSort` (same algorithm; handles negatives) |
| `Sort.PatienceSorting` | `Sort.PatienceSort` |
| `Sort.BogoSort`, `Sort.BeadSort`, `Sort.TreeSort` | `EducationalSort.BogoSort`, `.BeadSort`, `.TreeSort` |
| `Sort.Node` (public class) | removed (internal now) |

Other behavior changes:

- `CountingSort` supports negatives; throws `ArgumentException` when `max - min + 1 > 2^24`. (v1: crash on negatives, overflow/OOM on wide ranges.)
- `BucketSortUniform` throws `ArgumentOutOfRangeException` for values outside [0, 1) or NaN. Empty input returns an empty array.
- `BogoSort` accepts a `CancellationToken`. `BeadSort` accepts non-negative values and caps the maximum at 2^20.
- `MergeSort` is stable. `QuickSort`/`IntroSort`/`HeapSort` are now allocation-light in-place implementations. `TreeSort` is iterative, so sorted input no longer causes a stack overflow.

## 9. `AreaCalculator`

All method names and parameters are unchanged. The class is `static`.

- `PolygonShoelaceArea` takes `IReadOnlyList<(double X, double Y)>` instead of `IList<...>`. `List<>` and arrays work as before; a custom type that only implements `IList<T>` needs `.ToList()`/`.ToArray()`.
- `Math.Pow(x, 2)` was replaced by `x * x`; results can differ in the last floating-point digit.

## 10. Windows Forms controls

- `ClickableElement` fires its event only when the **left** button is pressed and released **inside** the element (v1: any button, anywhere).
- `ClickableElement` has a parameterless constructor (for the designer). `GetTitle()`/`IsToggled()` are unchanged.
- `VerticalList` no longer adjusts item margins on every add. The extra bottom spacing is list padding now, so verify the look in your UI. `OnLayout`/`OnResize`/`AdjustItemWidths` overrides are gone (subclasses that relied on them should override `OnClientSizeChanged`).

---

## Quick checklist

1. `Convert.` → `StringConvert.`
2. `Check.Email.*` → `EmailDomainValidator` instance
3. `Check.CheckConnection()` → `await Check.HasInternetAsync()`
4. `ExeRunner(...)` → `Actions.ExecuteAsync(...)`
5. `PyCS` constructor args, `PipLocal`, `Run` return values; opt in to `TrustPypiHosts` only if needed
6. `ProjectToLocationThenExtractZip[ThenDelete]` → `ExtractEmbeddedZip`
7. Sorting: stop relying on in-place mutation; rename `TimSort`, `PigeonholeSort`, `PatienceSorting`; move Bogo/Bead/Tree to `EducationalSort`
8. Wrap calls in `try/catch` where you relied on console-printed errors
9. Named arguments using old PascalCase parameter names
