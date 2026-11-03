using System.Diagnostics;

namespace inpsNuGet;

/// <summary>Runs a delegate on the thread pool or a dedicated thread, or launches executables.</summary>
public sealed class Actions
{
    readonly Action _action;
    Task? _runningTask;

    public Task? RunningTask => Volatile.Read(ref _runningTask);
    public bool IsRunning => RunningTask is { IsCompleted: false };

    public Actions() : this(static () => { }) { }

    public Actions(Action action) => _action = action ?? throw new ArgumentNullException(nameof(action));

    public Actions Run()
    {
        Volatile.Write(ref _runningTask, Task.Run(_action));
        return this;
    }

    /// <summary>Starts an executable; the result is available through <see cref="RunningTask"/> (faults on failure).</summary>
    public Actions RunExe(string filePath, params string[] fileArguments)
    {
        Volatile.Write(ref _runningTask, ExecuteAsync(filePath, fileArguments));
        return this;
    }

    public Actions RunOnDedicatedThread(bool doInBackground = true)
    {
        // RunContinuationsAsynchronously: awaiters must not run inline on the dedicated thread.
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var thread = new Thread(() =>
        {
            try { _action(); tcs.SetResult(); }
            catch (Exception e) { tcs.SetException(e); }
        })
        { IsBackground = doInBackground };

        Volatile.Write(ref _runningTask, tcs.Task);
        thread.Start();
        return this;
    }

    /// <summary>Runs an executable and returns its exit code and captured output. Never writes to the console.</summary>
    public static async Task<ProcessResult> ExecuteAsync(
        string filePath, IEnumerable<string>? arguments = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var psi = new ProcessStartInfo(filePath)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        if (arguments is not null)
            foreach (string a in arguments) psi.ArgumentList.Add(a);

        using var process = new Process { StartInfo = psi };
        if (!process.Start()) throw new InvalidOperationException($"Could not start '{filePath}'.");

        // Read both pipes concurrently to avoid the full-pipe deadlock.
        Task<string> stdout = process.StandardOutput.ReadToEndAsync(cancellationToken);
        Task<string> stderr = process.StandardError.ReadToEndAsync(cancellationToken);

        try
        {
            await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            try { process.Kill(entireProcessTree: true); } catch (InvalidOperationException) { }
            throw;
        }

        return new ProcessResult(
            process.ExitCode,
            await stdout.ConfigureAwait(false),
            await stderr.ConfigureAwait(false));
    }
}
