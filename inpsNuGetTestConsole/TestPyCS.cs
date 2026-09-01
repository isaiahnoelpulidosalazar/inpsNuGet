using inpsNuGet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace inpsNuGetTestConsole
{
    public class TestPyCS
    {
        readonly bool ShowConsole = true;
        readonly object ProcessLock = new();
        Process? CurrentProcess;

        const string PythonVersionShort = "Python 3.13";
        const string PythonVersionFull = "Python 3.13.5";
        const string PythonZip = "Python.zip";
        const string PythonFilesZip = "PythonFiles.zip";
        const string PythonDir = "Python";
        static readonly string PythonExe = Path.Combine(PythonDir, "python.exe");
        static readonly string PipExe = Path.Combine(PythonDir, "Scripts", "pip.exe");
        static readonly string GetPipScript = Path.Combine(PythonDir, "get-pip.py");
        static readonly string SiteCustomize = Path.Combine(PythonDir, "sitecustomize.py");
        static readonly string MainPy = Path.Combine(PythonDir, "main.py");

        public TestPyCS() : this(true) { }

        public TestPyCS(bool console)
        {
            ShowConsole = console;
            CreatePython();
        }

        private void CreatePython()
        {
            if (!Directory.Exists(PythonDir))
            {
                if (ShowConsole)
                {
                    Console.WriteLine($"Creating {PythonVersionShort} resources...");
                }
                try
                {
                    SimpleFileHandler.ProjectToLocationThenExtractZipThenDelete(Assembly.GetExecutingAssembly(), PythonZip, PythonDir);
                    SimpleFileHandler.ProjectToLocationThenExtractZipThenDelete(Assembly.GetExecutingAssembly(), PythonFilesZip, PythonDir);
                    
                    string NestedExtractPath = Path.Combine(PythonDir, "python313");
                    string PthPath = Path.Combine(PythonDir, "python313._pth");
                    string PthContent = "python313.zip\r\n.\r\n\r\n# Uncomment to run site.main() automatically\r\nimport site\r\n";
                    File.WriteAllText(PthPath, PthContent, new UTF8Encoding(false));

                    string NestedZip = Path.Combine(PythonDir, "python313.zip");
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
                bool PipInstalled = Directory.Exists(Path.Combine(PythonDir, "Lib")) &&
                                    Directory.Exists(Path.Combine(PythonDir, "Scripts")) &&
                                    File.Exists(PipExe) &&
                                    File.Exists(Path.Combine(PythonDir, "Scripts", "pip3.13.exe")) &&
                                    File.Exists(Path.Combine(PythonDir, "Scripts", "pip3.exe"));

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

        private static void ExtractZipSafe(string ZipPath, string ExtractPath)
        {
            using (var Archive = ZipFile.OpenRead(ZipPath))
            {
                foreach (var Entry in Archive.Entries)
                {
                    string TargetPath = Path.GetFullPath(Path.Combine(ExtractPath, Entry.FullName));

                    string? DirectoryPath = Path.GetDirectoryName(TargetPath);
                    if (DirectoryPath != null)
                    {
                        Directory.CreateDirectory(DirectoryPath);
                    }

                    if (!string.IsNullOrEmpty(Entry.Name))
                    {
                        Entry.ExtractToFile(TargetPath, overwrite: true);
                    }
                }
            }
        }
    }
}
