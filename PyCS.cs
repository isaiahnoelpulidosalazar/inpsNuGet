using System.Diagnostics;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;

namespace inpsNuGet;

/// <summary>
/// Manages an embedded Python 3.13 runtime (extracted lazily on first use) with pip and script execution.
/// Requires Python.zip and PythonFiles.zip embedded in the package.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class PyCS
{
    const string PthContent = "python313.zip\r\n.\r\n\r\n# Uncomment to run site.main() automatically\r\nimport site\r\n";
    static readonly string[] TrustedHostArgs =
        ["--trusted-host", "pypi.org", "--trusted-host", "files.pythonhosted.org", "--trusted-host", "pypi.python.org"];

    readonly TextWriter? _log;
    readonly string _root, _pythonExe, _pipExe, _getPipScript, _siteCustomize;
    readonly object _lock = new();
    Process? _current;

    /// <summary>Opt-in. Adds --trusted-host for PyPI hosts, which DISABLES TLS verification for them.</summary>
    public bool TrustPypiHosts { get; set; }

    /// <param name="directory">Parent folder; Python is extracted to "&lt;directory&gt;/Python". Default: ./Python.</param>
    /// <param name="log">Optional progress sink (e.g. Console.Out). Null = silent.</param>
    public PyCS(string? directory = null, TextWriter? log = null)
    {
        _log = log;
        _root = string.IsNullOrWhiteSpace(directory) ? "Python" : Path.Combine(directory, "Python");
        _pythonExe = Path.Combine(_root, "python.exe");
        _pipExe = Path.Combine(_root, "Scripts", "pip.exe");
        _getPipScript = Path.Combine(_root, "get-pip.py");
        _siteCustomize = Path.Combine(_root, "sitecustomize.py");
    }

    /// <summary>Extracts the runtime if needed. Idempotent; called automatically by every other method.</summary>
    public void Initialize()
    {
        lock (_lock)
        {
            if (File.Exists(_pythonExe)) return;

            _log?.WriteLine("Creating Python 3.13 resources...");
            Assembly asm = typeof(PyCS).Assembly;
            try
            {
                SimpleFileHandler.ExtractEmbeddedZip(asm, "Python.zip", _root);
                SimpleFileHandler.ExtractEmbeddedZip(asm, "PythonFiles.zip", _root);
                File.WriteAllText(Path.Combine(_root, "python313._pth"), PthContent, new UTF8Encoding(false));
                SimpleFileHandler.ExtractZipSafe(Path.Combine(_root, "python313.zip"), Path.Combine(_root, "python313"));
            }
            catch
            {
                try { Directory.Delete(_root, recursive: true); } catch { /* best effort */ }
                throw;
            }
        }
    }

    /// <summary>Installs pip if missing. Returns the get-pip result, or null if pip was already installed.</summary>
    public ProcessResult? InstallPip()
    {
        Initialize();
        if (File.Exists(_pipExe)) { _log?.WriteLine("pip already installed."); return null; }
        if (!File.Exists(_getPipScript) || !File.Exists(_siteCustomize))
            throw new FileNotFoundException("get-pip.py / sitecustomize.py not found in the extracted runtime.", _getPipScript);

        _log?.WriteLine("Installing pip...");
        var args = new List<string> { _getPipScript };
        if (TrustPypiHosts) args.AddRange(TrustedHostArgs);
        return Log(Execute(_pythonExe, args));
    }

    public ProcessResult Pip(params string[] packages) => RunPip(["install"], packages, []);

    public ProcessResult PipUpgrade(params string[] packages) => RunPip(["install", "--upgrade"], packages, []);

    /// <summary>Installs from a local wheel directory only (no index access).</summary>
    public ProcessResult PipLocal(string findLinksDirectory, params string[] packages) =>
        RunPip(["install"], packages, ["--no-index", "--find-links", findLinksDirectory], useTrustedHosts: false);

    ProcessResult RunPip(string[] lead, string[] packages, string[] extra, bool useTrustedHosts = true)
    {
        Initialize();
        var args = new List<string>(lead);
        args.AddRange(packages); // one argv entry per package: no argument injection through spacing
        args.AddRange(extra);
        if (useTrustedHosts && TrustPypiHosts) args.AddRange(TrustedHostArgs);
        return Log(Execute(_pipExe, args));
    }

    public ProcessResult Run(string script)
    {
        Initialize();
        // Unique temp file per call, so concurrent runs can't overwrite each other.
        string path = Path.Combine(_root, $"main-{Guid.NewGuid():N}.py");
        File.WriteAllText(path, script, new UTF8Encoding(false));
        try { return Log(Execute(_pythonExe, [path])); }
        finally { File.Delete(path); }
    }

    public ProcessResult RunFile(string filePath)
    {
        Initialize();
        return Log(Execute(_pythonExe, [filePath]));
    }

    /// <summary>Runs a script and returns <see cref="ProcessResult.Combined"/> text.</summary>
    public string GetOutput(string script) => Run(script).Combined;

    public string GetFileOutput(string filePath) => RunFile(filePath).Combined;

    /// <summary>Kills the running Python process (and its children), if any.</summary>
    public void Stop()
    {
        lock (_lock)
        {
            if (_current is { HasExited: false } p)
            {
                try { p.Kill(entireProcessTree: true); }
                catch (InvalidOperationException) { }
            }
        }
    }

    ProcessResult Execute(string exe, IEnumerable<string> args)
    {
        var psi = new ProcessStartInfo(exe)
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };
        psi.Environment["PYTHONUTF8"] = "1";
        foreach (string a in args) psi.ArgumentList.Add(a);

        using var process = new Process { StartInfo = psi };
        lock (_lock) _current = process;
        try
        {
            process.Start();
            Task<string> stdout = process.StandardOutput.ReadToEndAsync();
            Task<string> stderr = process.StandardError.ReadToEndAsync();
            process.WaitForExit();
            return new ProcessResult(process.ExitCode, stdout.GetAwaiter().GetResult(), stderr.GetAwaiter().GetResult());
        }
        finally
        {
            lock (_lock) { if (ReferenceEquals(_current, process)) _current = null; }
        }
    }

    ProcessResult Log(ProcessResult r)
    {
        if (_log is not null && !string.IsNullOrWhiteSpace(r.Combined)) _log.WriteLine(r.Combined);
        return r;
    }
}
