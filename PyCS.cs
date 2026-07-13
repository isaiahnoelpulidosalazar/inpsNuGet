using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;

namespace inpsNuGet;

public class PyCS
{
    private readonly bool _showConsole = true;
    private readonly object _processLock = new();
    private Process? _currentProcess;

    private static readonly string PythonZip = "python-3.13.5-embed-amd64.zip";
    private static readonly string PythonDir = "python3_13";
    private static readonly string PythonExe = Path.Combine(PythonDir, "python.exe");
    private static readonly string PipExe = Path.Combine(PythonDir, "Scripts", "pip.exe");
    private static readonly string GetPipScript = Path.Combine(PythonDir, "get-pip.py");
    private static readonly string SiteCustomize = Path.Combine(PythonDir, "sitecustomize.py");
    private static readonly string MainPy = Path.Combine(PythonDir, "main.py");

    public PyCS() : this(true)
    {
    }

    public PyCS(bool console)
    {
        _showConsole = console;
        CreatePython();
    }

    private void CreatePython()
    {
        if (!File.Exists(PythonZip))
        {
            if (_showConsole)
            {
                Console.WriteLine("Creating Python 3.13 resources...");
            }
            try
            {
                using (var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("inpsNuGet.python-3.13.5-embed-amd64.zip"))
                {
                    if (resourceStream == null)
                    {
                        throw new FileNotFoundException("Embedded Python ZIP resource not found.");
                    }

                    using (var fileStream = File.Create(PythonZip))
                    {
                        resourceStream.CopyTo(fileStream);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create Python 3.13 resources: {ex.Message}");
            }
        }
        else
        {
            if (_showConsole)
            {
                Console.WriteLine("Python 3.13 resources already created.");
            }
        }

        bool zipReadable = false;
        try
        {
            if (File.Exists(PythonZip))
            {
                using (File.OpenRead(PythonZip)) { }
                zipReadable = true;
            }
        }
        catch
        {
        }

        if (zipReadable)
        {
            string nestedExtractPath = Path.Combine(PythonDir, "python313");
            if (!Directory.Exists(nestedExtractPath))
            {
                if (_showConsole)
                {
                    Console.WriteLine("Extracting Python 3.13 resources...");
                }
                try
                {
                    Directory.CreateDirectory(PythonDir);
                    ZipFile.ExtractToDirectory(PythonZip, PythonDir);

                    string pthPath = Path.Combine(PythonDir, "python313._pth");
                    string pthContent = "python313.zip\r\n.\r\n\r\n# Uncomment to run site.main() automatically\r\nimport site\r\n";
                    File.WriteAllText(pthPath, pthContent, Encoding.UTF8);

                    string nestedZip = Path.Combine(PythonDir, "python313.zip");
                    ZipFile.ExtractToDirectory(nestedZip, nestedExtractPath);

                    using (var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("inpsNuGet.sitecustomize.py"))
                    {
                        if (resourceStream != null)
                        {
                            using (var fileStream = File.Create(SiteCustomize))
                            {
                                resourceStream.CopyTo(fileStream);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (_showConsole)
                    {
                        Console.WriteLine($"Failed to extract Python 3.13 resources: {ex.Message}");
                    }
                }
            }
            else
            {
                if (_showConsole)
                {
                    Console.WriteLine("Python 3.13 resources already extracted.");
                }
            }
        }
    }

    public void InstallPip()
    {
        try
        {
            if (!File.Exists(GetPipScript))
            {
                if (_showConsole)
                {
                    Console.WriteLine("Extracting get-pip.py from resources...");
                }

                using (var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("inpsNuGet.get-pip.py"))
                {
                    if (resourceStream == null)
                    {
                        throw new FileNotFoundException("Embedded get-pip.py resource not found.");
                    }

                    Directory.CreateDirectory(PythonDir);
                    using (var fileStream = File.Create(GetPipScript))
                    {
                        resourceStream.CopyTo(fileStream);
                    }
                }
            }
            else
            {
                if (_showConsole)
                {
                    Console.WriteLine("get-pip.py already extracted.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to extract get-pip: {ex.Message}. Make sure get-pip.py is set to 'Embedded Resource' in your project.");
        }

        bool getPipExists = File.Exists(GetPipScript);
        bool siteCustomizeExists = File.Exists(SiteCustomize);

        if (getPipExists && siteCustomizeExists)
        {
            bool pipInstalled = Directory.Exists(Path.Combine(PythonDir, "Lib")) &&
                                Directory.Exists(Path.Combine(PythonDir, "Scripts")) &&
                                File.Exists(PipExe) &&
                                File.Exists(Path.Combine(PythonDir, "Scripts", "pip3.13.exe")) &&
                                File.Exists(Path.Combine(PythonDir, "Scripts", "pip3.exe"));

            if (!pipInstalled)
            {
                if (_showConsole)
                {
                    Console.WriteLine("Installing pip (requires internet access for the Python process)...");
                }

                try
                {
                    string output = RunProcess(PythonExe, GetPipScript);
                    if (!string.IsNullOrWhiteSpace(output))
                    {
                        Console.WriteLine("pip installation completed.");
                    }
                    else
                    {
                        Console.WriteLine("Failed to install pip. (The execution of get-pip.py still needs internet access to fetch pip wheels).");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to run pip installation: {ex.Message}");
                }
            }
            else
            {
                if (_showConsole)
                {
                    Console.WriteLine("pip already installed.");
                }
            }
        }
    }

    public void Pip(string[] args)
    {
        string arguments = "install " + string.Join(" ", args);
        string output = RunProcess(PipExe, arguments);
        if (_showConsole)
        {
            Console.WriteLine(output);
        }
    }

    public void PipUpgrade(string[] args)
    {
        string arguments = "install --upgrade " + string.Join(" ", args);
        string output = RunProcess(PipExe, arguments);
        if (_showConsole)
        {
            Console.WriteLine(output);
        }
    }

    public void PipLocal(string[] args)
    {
        string arguments = "install " + string.Join(" ", args) + " --no-index --find-links /";
        string output = RunProcess(PipExe, arguments);
        if (_showConsole)
        {
            Console.WriteLine(output);
        }
    }

    public void Stop()
    {
        lock (_processLock)
        {
            if (_currentProcess != null && !_currentProcess.HasExited)
            {
                try
                {
                    _currentProcess.CloseMainWindow();
                    if (!_currentProcess.WaitForExit(2000))
                    {
                        _currentProcess.Kill();
                        _currentProcess.WaitForExit();
                    }
                }
                catch
                {
                }
            }
        }
    }

    public void Run(string script)
    {
        File.WriteAllText(MainPy, script);
        string output = RunProcess(PythonExe, MainPy);
        Console.WriteLine(output);
    }

    public void RunFile(string filePath)
    {
        string output = RunProcess(PythonExe, filePath);
        Console.WriteLine(output);
    }

    public string GetOutput(string script)
    {
        File.WriteAllText(MainPy, script);
        return RunProcess(PythonExe, MainPy);
    }

    public string GetFileOutput(string filePath)
    {
        return RunProcess(PythonExe, filePath);
    }

    private string RunProcess(string fileName, string arguments)
    {
        using (var proc = new Process())
        {
            proc.StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = _showConsole
            };

            lock (_processLock)
            {
                _currentProcess = proc;
            }

            try
            {
                proc.Start();
                string output = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit();
                return output;
            }
            catch (Exception ex)
            {
                return $"Execution failed: {ex.Message}";
            }
            finally
            {
                lock (_processLock)
                {
                    if (_currentProcess == proc)
                    {
                        _currentProcess = null;
                    }
                }
            }
        }
    }
}