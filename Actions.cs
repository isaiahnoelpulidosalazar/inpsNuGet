using System;
using System.Threading;
using System.Threading.Tasks;

namespace inpsNuGet;

public class Actions
{
    readonly Action Action;
    public Task RunningTask { get; private set; }
    public bool IsRunning => RunningTask != null && !RunningTask.IsCompleted;

    public Actions(Action Action)
    {
        this.Action = Action;
    }

    public Actions Run()
    {
        RunningTask = Task.Run(Action);
        return this;
    }

    public Actions RunOnDedicatedThread(bool DoInBackground = true)
    {
        var Thread = new Thread(new ThreadStart(Action))
        {
            IsBackground = DoInBackground
        };
        Thread.Start();
        return this;
    }
}