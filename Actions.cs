using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace inpsNuGet;

public class Actions
{
    readonly Action Action;
    public Task? RunningTask { get; private set; }
    public bool IsRunning => RunningTask != null && !RunningTask.IsCompleted;

    public Actions()
    {
        this.Action = () => { };
    }
    
    public Actions(Action Action)
    {
        this.Action = Action;
    }

    public Actions Run()
    {
        RunningTask = Task.Run(Action);
        return this;
    }

    public Actions RunExe(string FilePath, params string[] FileArguments)
    {
        RunningTask = ExeRunner(FilePath, FileArguments);
        return this;
    }

    public async Task ExeRunner(string FilePath, params string[] FileArguments)
    {
        ProcessStartInfo ProcessStartInfo = new ProcessStartInfo
        {
            FileName = FilePath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        foreach (string Argument in FileArguments)
        {
            ProcessStartInfo.ArgumentList.Add(Argument);
        }

        using Process Process = new Process { StartInfo = ProcessStartInfo };
        Process.Start();

        Task<string> OutputTask = Process.StandardOutput.ReadToEndAsync();
        Task<string> ErrorTask = Process.StandardError.ReadToEndAsync();

        await Process.WaitForExitAsync();

        string Output = await OutputTask;
        string Error = await ErrorTask;

        Console.WriteLine($"Exit Code: {Process.ExitCode}");
        Console.WriteLine($"Output: {Output}");
        if (!string.IsNullOrEmpty(Error))
        {
            Console.WriteLine($"Error: {Error}");
        }
    }

    public Actions RunOnDedicatedThread(bool DoInBackground = true)
    {
        TaskCompletionSource TaskCompletionSource = new TaskCompletionSource();

        var Thread = new Thread(() =>
        {
            try
            {
                Action();
                TaskCompletionSource.SetResult();
            }
            catch (Exception e)
            {
                TaskCompletionSource.SetException(e);
            }
        })
        {
            IsBackground = DoInBackground
        };

        RunningTask = TaskCompletionSource.Task;
        Thread.Start();

        return this;
    }
}