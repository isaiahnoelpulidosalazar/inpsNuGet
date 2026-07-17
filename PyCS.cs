using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace inpsNuGet;

public class PyCS
{
    private readonly bool _showConsole = true;
    private readonly object _processLock = new();
    private Process? _currentProcess;

    private static readonly string PythonZip = "python-3.13.5-embed-amd64.zip";
    private static readonly string PyFilesZip = "py_files.zip";
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

        if (!File.Exists(PyFilesZip))
        {
            try
            {
                using (var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("inpsNuGet.py_files.zip"))
                {
                    if (resourceStream == null)
                    {
                        throw new FileNotFoundException("Embedded py_files.zip resource not found.");
                    }

                    using (var fileStream = File.Create(PyFilesZip))
                    {
                        resourceStream.CopyTo(fileStream);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write py_files.zip: {ex.Message}");
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
                    try
                    {
                        if (Directory.Exists(PythonDir))
                        {
                            Directory.Delete(PythonDir, true);
                        }
                    }
                    catch
                    {
                    }

                    Directory.CreateDirectory(PythonDir);
                    
                    ExtractZipSafe(PythonZip, PythonDir);

                    string pthPath = Path.Combine(PythonDir, "python313._pth");
                    string pthContent = "python313.zip\r\n.\r\n\r\n# Uncomment to run site.main() automatically\r\nimport site\r\n";
                    File.WriteAllText(pthPath, pthContent, new UTF8Encoding(false));

                    string nestedZip = Path.Combine(PythonDir, "python313.zip");
                    ExtractZipSafe(nestedZip, nestedExtractPath);

                    if (File.Exists(PyFilesZip))
                    {
                        ExtractZipSafe(PyFilesZip, PythonDir);
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
            if (!File.Exists(GetPipScript) || !File.Exists(SiteCustomize))
            {
                if (_showConsole)
                {
                    Console.WriteLine("Extracting helper files from py_files.zip...");
                }

                if (!File.Exists(PyFilesZip))
                {
                    using (var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("inpsNuGet.py_files.zip"))
                    {
                        if (resourceStream == null)
                        {
                            throw new FileNotFoundException("Embedded py_files.zip resource not found.");
                        }

                        using (var fileStream = File.Create(PyFilesZip))
                        {
                            resourceStream.CopyTo(fileStream);
                        }
                    }
                }

                Directory.CreateDirectory(PythonDir);
                ExtractZipSafe(PyFilesZip, PythonDir);
            }
            else
            {
                if (_showConsole)
                {
                    Console.WriteLine("Helper files already extracted.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to extract helper files: {ex.Message}. Make sure py_files.zip is set to 'Embedded Resource' in your project.");
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
                    Console.WriteLine("Installing pip...");
                }

                try
                {
                    string getPipArguments = $"\"{GetPipScript}\" --trusted-host pypi.org --trusted-host files.pythonhosted.org --trusted-host pypi.python.org";
                    string output = RunProcess(PythonExe, getPipArguments);
                    
                    if (!string.IsNullOrWhiteSpace(output))
                    {
                        Console.WriteLine("pip installation log:");
                        Console.WriteLine(output);
                    }
                    else
                    {
                        Console.WriteLine("Failed to install pip. Empty output from get-pip process.");
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
        string arguments = "install " + string.Join(" ", args) + " --trusted-host pypi.org --trusted-host files.pythonhosted.org --trusted-host pypi.python.org";
        string output = RunProcess(PipExe, arguments);
        if (_showConsole)
        {
            Console.WriteLine(output);
        }
    }

    public void PipUpgrade(string[] args)
    {
        string arguments = "install --upgrade " + string.Join(" ", args) + " --trusted-host pypi.org --trusted-host files.pythonhosted.org --trusted-host pypi.python.org";
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
                RedirectStandardError = true,
                CreateNoWindow = _showConsole
            };

            lock (_processLock)
            {
                _currentProcess = proc;
            }

            try
            {
                proc.Start();
                
                var outTask = proc.StandardOutput.ReadToEndAsync();
                var errTask = proc.StandardError.ReadToEndAsync();
                
                proc.WaitForExit();
                
                Task.WaitAll(outTask, errTask);

                string output = outTask.Result;
                string error = errTask.Result;

                if (!string.IsNullOrWhiteSpace(error))
                {
                    return $"{output}{Environment.NewLine}Error Output:{Environment.NewLine}{error}";
                }
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

    private static void ExtractZipSafe(string zipPath, string extractPath)
    {
        using (var archive = ZipFile.OpenRead(zipPath))
        {
            foreach (var entry in archive.Entries)
            {
                string targetPath = Path.GetFullPath(Path.Combine(extractPath, entry.FullName));
                
                string? directory = Path.GetDirectoryName(targetPath);
                if (directory != null)
                {
                    Directory.CreateDirectory(directory);
                }

                if (!string.IsNullOrEmpty(entry.Name)) 
                {
                    entry.ExtractToFile(targetPath, overwrite: true);
                }
            }
        }
    }
}