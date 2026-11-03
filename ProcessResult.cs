namespace inpsNuGet;

/// <summary>Result of an external process run.</summary>
public sealed record ProcessResult(int ExitCode, string StandardOutput, string StandardError)
{
    public bool Succeeded => ExitCode == 0;

    /// <summary>Stdout, followed by "Error Output:" and stderr when stderr is not blank.</summary>
    public string Combined =>
        string.IsNullOrWhiteSpace(StandardError)
            ? StandardOutput
            : (string.IsNullOrWhiteSpace(StandardOutput) ? "" : StandardOutput + Environment.NewLine)
              + "Error Output:" + Environment.NewLine + StandardError;
}
