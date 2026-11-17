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

    /// <summary>Runs the action on the thread pool and blocks until it finishes. Rethrows the original exception on failure.</summary>
    public Actions Run()
    {
        Volatile.Write(ref _runningTask, Task.Run(_action));
        WaitForCurrent();
        return this;
    }

    /// <summary>Runs an executable and blocks until it exits (faults on failure).</summary>
    public Actions RunExe(string filePath, params string[] fileArguments)
    {
        Volatile.Write(ref _runningTask, ExecuteAsync(filePath, fileArguments));
        WaitForCurrent();
        return this;
    }

    /// <summary>Runs the action on a dedicated thread and blocks until it finishes. Rethrows the original exception on failure.</summary>
    public Actions RunOnDedicatedThread(bool doInBackground = true)
    {
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var thread = new Thread(() =>
        {
            try { _action(); tcs.SetResult(); }
            catch (Exception e) { tcs.SetException(e); }
        })
        { IsBackground = doInBackground };

        Volatile.Write(ref _runningTask, tcs.Task);
        thread.Start();
        WaitForCurrent();
        return this;
    }

    // GetAwaiter().GetResult() rethrows the original exception instead of wrapping it in an AggregateException.
    void WaitForCurrent() => RunningTask?.GetAwaiter().GetResult();

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