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
    readonly bool ShowConsole = true;
    readonly object ProcessLock = new();
    Process? CurrentProcess;

    const string PythonVersionShort = "Python 3.13";
    const string PythonVersionFull = "Python 3.13.5";
    const string PythonZip = "Python.zip";
    const string PythonFilesZip = "PythonFiles.zip";
    const string PythonDir = "Python";
    string CustomPythonDir = string.Empty;
    string TargetPythonDir = string.Empty;
    string PythonExe = Path.Combine(PythonDir, "python.exe");
    string PipExe = Path.Combine(PythonDir, "Scripts", "pip.exe");
    string GetPipScript = Path.Combine(PythonDir, "get-pip.py");
    string SiteCustomize = Path.Combine(PythonDir, "sitecustomize.py");
    string MainPy = Path.Combine(PythonDir, "main.py");

    public PyCS() : this(true) { }

    public PyCS(bool console)
    {
        ShowConsole = console;
        CreatePython();
    }

    public PyCS(string customDir)
    {
        ShowConsole = true;
        if (customDir != null && !string.IsNullOrWhiteSpace(customDir))
        {
            CustomPythonDir = customDir;
            TargetPythonDir = string.IsNullOrWhiteSpace(CustomPythonDir) ? PythonDir : CustomPythonDir + "\\" + PythonDir;
            PythonExe = Path.Combine(TargetPythonDir, "python.exe");
            PipExe = Path.Combine(TargetPythonDir, "Scripts", "pip.exe");
            GetPipScript = Path.Combine(TargetPythonDir, "get-pip.py");
            SiteCustomize = Path.Combine(TargetPythonDir, "sitecustomize.py");
            MainPy = Path.Combine(TargetPythonDir, "main.py");
        }
        CreatePython();
    }

    public PyCS(bool console, string customDir)
    {
        ShowConsole = console;
        if (customDir != null && !string.IsNullOrWhiteSpace(customDir))
        {
            CustomPythonDir = customDir;
            TargetPythonDir = string.IsNullOrWhiteSpace(CustomPythonDir) ? PythonDir : CustomPythonDir + "\\" + PythonDir;
            PythonExe = Path.Combine(TargetPythonDir, "python.exe");
            PipExe = Path.Combine(TargetPythonDir, "Scripts", "pip.exe");
            GetPipScript = Path.Combine(TargetPythonDir, "get-pip.py");
            SiteCustomize = Path.Combine(TargetPythonDir, "sitecustomize.py");
            MainPy = Path.Combine(TargetPythonDir, "main.py");
        }
        CreatePython();
    }

    private void CreatePython()
    {
        if (!Directory.Exists(TargetPythonDir))
        {
            if (ShowConsole)
            {
                Console.WriteLine($"Creating {PythonVersionShort} resources...");
            }
            try
            {
                SimpleFileHandler.ProjectToLocationThenExtractZipThenDelete(Assembly.GetExecutingAssembly(), PythonZip, TargetPythonDir);
                SimpleFileHandler.ProjectToLocationThenExtractZipThenDelete(Assembly.GetExecutingAssembly(), PythonFilesZip, TargetPythonDir);
                
                string NestedExtractPath = Path.Combine(TargetPythonDir, "python313");
                string PthPath = Path.Combine(TargetPythonDir, "python313._pth");
                string PthContent = "python313.zip\r\n.\r\n\r\n# Uncomment to run site.main() automatically\r\nimport site\r\n";
                File.WriteAllText(PthPath, PthContent, new UTF8Encoding(false));

                string NestedZip = Path.Combine(TargetPythonDir, "python313.zip");
                SimpleFileHandler.ExtractZipSafe(NestedZip, NestedExtractPath);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to create {PythonVersionShort} resources: {e.Message}");
            }
        }
    }

    public void InstallPip()
    {
        bool GetPipExists = File.Exists(GetPipScript);
        bool SiteCustomizeExists = File.Exists(SiteCustomize);

        if (GetPipExists && SiteCustomizeExists)
        {
            bool PipInstalled = Directory.Exists(Path.Combine(TargetPythonDir, "Lib")) &&
                                Directory.Exists(Path.Combine(TargetPythonDir, "Scripts")) &&
                                File.Exists(PipExe) &&
                                File.Exists(Path.Combine(TargetPythonDir, "Scripts", "pip3.13.exe")) &&
                                File.Exists(Path.Combine(TargetPythonDir, "Scripts", "pip3.exe"));

            if (!PipInstalled)
            {
                if (ShowConsole)
                {
                    Console.WriteLine("Installing pip...");
                }

                try
                {
                    string GetPipArguments = $"\"{GetPipScript}\" --trusted-host pypi.org --trusted-host files.pythonhosted.org --trusted-host pypi.python.org";
                    string Output = RunProcess(PythonExe, GetPipArguments);

                    if (!string.IsNullOrWhiteSpace(Output))
                    {
                        Console.WriteLine("pip installation log:");
                        Console.WriteLine(Output);
                    }
                    else
                    {
                        Console.WriteLine("Failed to install pip. Empty output from get-pip process.");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to run pip installation: {e.Message}");
                }
            }
            else
            {
                if (ShowConsole)
                {
                    Console.WriteLine("pip already installed.");
                }
            }
        }
    }

    public void Pip(string[] args)
    {
        string Arguments = "install " + string.Join(" ", args) + " --trusted-host pypi.org --trusted-host files.pythonhosted.org --trusted-host pypi.python.org";
        string Output = RunProcess(PipExe, Arguments);
        if (ShowConsole)
        {
            Console.WriteLine(Output);
        }
    }

    public void PipUpgrade(string[] args)
    {
        string Arguments = "install --upgrade " + string.Join(" ", args) + " --trusted-host pypi.org --trusted-host files.pythonhosted.org --trusted-host pypi.python.org";
        string Output = RunProcess(PipExe, Arguments);
        if (ShowConsole)
        {
            Console.WriteLine(Output);
        }
    }

    public void PipLocal(string[] args)
    {
        string Arguments = "install " + string.Join(" ", args) + " --no-index --find-links /";
        string Output = RunProcess(PipExe, Arguments);
        if (ShowConsole)
        {
            Console.WriteLine(Output);
        }
    }

    public void Stop()
    {
        lock (ProcessLock)
        {
            if (CurrentProcess != null && !CurrentProcess.HasExited)
            {
                try
                {
                    CurrentProcess.CloseMainWindow();
                    if (!CurrentProcess.WaitForExit(2000))
                    {
                        CurrentProcess.Kill();
                        CurrentProcess.WaitForExit();
                    }
                }
                catch { }
            }
        }
    }

    public void Run(string Script)
    {
        File.WriteAllText(MainPy, Script);
        string Output = RunProcess(PythonExe, MainPy);
        Console.WriteLine(Output);
    }

    public void RunFile(string FilePath)
    {
        string Output = RunProcess(PythonExe, FilePath);
        Console.WriteLine(Output);
    }

    public string GetOutput(string Script)
    {
        File.WriteAllText(MainPy, Script);
        return RunProcess(PythonExe, MainPy);
    }

    public string GetFileOutput(string FilePath)
    {
        return RunProcess(PythonExe, FilePath);
    }

    private string RunProcess(string PythonExeFileName, string PythonExeArguments)
    {
        using (var Process = new Process())
        {
            Process.StartInfo = new ProcessStartInfo
            {
                FileName = PythonExeFileName,
                Arguments = PythonExeArguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = ShowConsole
            };

            lock (ProcessLock)
            {
                CurrentProcess = Process;
            }

            try
            {
                Process.Start();

                var OutputTask = Process.StandardOutput.ReadToEndAsync();
                var ErrorTask = Process.StandardError.ReadToEndAsync();

                Process.WaitForExit();

                Task.WaitAll(OutputTask, ErrorTask);

                string Output = OutputTask.Result;
                string Error = ErrorTask.Result;

                if (!string.IsNullOrWhiteSpace(Error))
                {
                    return $"{Output}{Environment.NewLine}Error Output:{Environment.NewLine}{Error}";
                }
                return Output;
            }
            catch (Exception e)
            {
                return $"Execution failed: {e.Message}";
            }
            finally
            {
                lock (ProcessLock)
                {
                    if (CurrentProcess == Process)
                    {
                        CurrentProcess = null;
                    }
                }
            }
        }
    }
}