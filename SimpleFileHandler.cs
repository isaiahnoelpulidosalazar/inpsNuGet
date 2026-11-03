using System.IO.Compression;
using System.Reflection;

namespace inpsNuGet;

public static class SimpleFileHandler
{
    public static void Write(string filePath, string content) => File.WriteAllText(filePath, content);
    public static string Read(string filePath) => File.ReadAllText(filePath);
    public static void Append(string filePath, string content) => File.AppendAllText(filePath, content);

    /// <summary>Extracts a zip, throwing <see cref="IOException"/> if any entry would land outside the target (Zip Slip).</summary>
    public static void ExtractZipSafe(string zipPath, string extractPath) =>
        ZipFile.ExtractToDirectory(zipPath, NormalizeDir(extractPath), overwriteFiles: true);

    /// <summary>
    /// Copies an embedded resource (matched by file-name suffix, case-insensitive) into <paramref name="targetDirectory"/>
    /// (current directory if empty). Throws <see cref="FileNotFoundException"/> if the resource doesn't exist.
    /// </summary>
    public static void ProjectToLocation(Assembly assembly, string resourceFileName, string targetDirectory = "")
    {
        using Stream source = OpenEmbedded(assembly, resourceFileName);
        string dir = NormalizeDir(targetDirectory);
        Directory.CreateDirectory(dir);

        using FileStream target = File.Create(Path.Combine(dir, Path.GetFileName(resourceFileName)));
        source.CopyTo(target);
    }

    /// <summary>Extracts an embedded zip straight from the assembly stream: no temporary file, nothing to delete.</summary>
    public static void ExtractEmbeddedZip(Assembly assembly, string resourceFileName, string targetDirectory = "")
    {
        using Stream source = OpenEmbedded(assembly, resourceFileName);
        using var archive = new ZipArchive(source, ZipArchiveMode.Read);
        string dir = NormalizeDir(targetDirectory);
        Directory.CreateDirectory(dir);
        archive.ExtractToDirectory(dir, overwriteFiles: true);
    }

    static string NormalizeDir(string dir) => string.IsNullOrEmpty(dir) ? "." : dir;

    static Stream OpenEmbedded(Assembly assembly, string fileName)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        string suffix = "." + Path.GetFileName(fileName);
        string? name = assembly.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith(suffix, StringComparison.OrdinalIgnoreCase));

        return (name is null ? null : assembly.GetManifestResourceStream(name))
            ?? throw new FileNotFoundException($"Embedded resource '*{suffix}' was not found.", fileName);
    }
}
