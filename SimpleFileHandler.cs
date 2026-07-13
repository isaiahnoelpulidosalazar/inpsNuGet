using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace inpsNuGet;

public class SimpleFileHandler
{
    public static void Write(string FilePath, string Content)
    {
        File.WriteAllText(FilePath, Content);
    }

    public static string Read(string FilePath)
    {
        return File.ReadAllText(FilePath);
    }

    public static void Append(string FilePath, string Content)
    {
        File.AppendAllText(FilePath, Content);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ProjectToLocation(Assembly ExecutingAssembly, string FileName)
    {
        try
        {
            string shortFileName = Path.GetFileName(FileName);
            string actualResourceName = ExecutingAssembly.GetManifestResourceNames().FirstOrDefault(name => name.EndsWith("." + shortFileName, StringComparison.OrdinalIgnoreCase));

            if (actualResourceName == null)
            {
                throw new FileNotFoundException($"Could not find embedded resource ending with '.{shortFileName}'");
            }

            string directory = Path.GetDirectoryName(FileName);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (Stream resourceStream = ExecutingAssembly.GetManifestResourceStream(actualResourceName))
            {
                using (FileStream projectFileStream = File.Create(FileName))
                {
                    resourceStream.CopyTo(projectFileStream);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Cannot copy project file. Error: {ex.Message}");
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ProjectToLocation(Assembly ExecutingAssembly, string FileName, string FilePath)
    {
        try
        {
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
            using (FileStream ProjectFileStream = File.Create(Path.Combine(FilePath, Path.GetFileName(FileName))))
            {
                ExecutingAssembly.GetManifestResourceStream(ExecutingAssembly.EntryPoint.DeclaringType.Namespace + "." + Path.GetFileName(FileName)).CopyTo(ProjectFileStream);
            }
        }
        catch
        {
            Console.WriteLine("Cannot copy project file. Please make sure the file's build action is set to 'Embedded Resource'.");
        }
    }
}
