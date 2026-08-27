using System.IO;
using System.IO.Compression;
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

    public static void ExtractZipSafe(string ZipPath, string ExtractPath)
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

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ProjectToLocation(Assembly ExecutingAssembly, string FileName)
    {
        try
        {
            string ShortFileName = Path.GetFileName(FileName);
            string ActualResourceName = ExecutingAssembly.GetManifestResourceNames().FirstOrDefault(name => name.EndsWith("." + ShortFileName, StringComparison.OrdinalIgnoreCase));

            if (ActualResourceName == null)
            {
                throw new FileNotFoundException($"Could not find embedded resource ending with '.{ShortFileName}'");
            }

            string DirectoryPath = Path.GetDirectoryName(FileName);
            if (!string.IsNullOrEmpty(DirectoryPath) && !Directory.Exists(DirectoryPath))
            {
                Directory.CreateDirectory(DirectoryPath);
            }

            using (Stream ResourceStream = ExecutingAssembly.GetManifestResourceStream(ActualResourceName))
            {
                using (FileStream ProjectFileStream = File.Create(FileName))
                {
                    ResourceStream.CopyTo(ProjectFileStream);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Cannot copy project file. Error: {e.Message}");
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ProjectToLocation(Assembly ExecutingAssembly, string FileName, string FilePath)
    {
        try
        {
            string ShortFileName = Path.GetFileName(FileName);
            string ActualResourceName = ExecutingAssembly.GetManifestResourceNames().FirstOrDefault(name => name.EndsWith("." + ShortFileName, StringComparison.OrdinalIgnoreCase));

            if (ActualResourceName == null)
            {
                throw new FileNotFoundException($"Could not find embedded resource ending with '.{ShortFileName}'");
            }

            if (!string.IsNullOrEmpty(FilePath) && !Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }

            using (Stream ResourceStream = ExecutingAssembly.GetManifestResourceStream(ActualResourceName))
            {
                using (FileStream ProjectFileStream = File.Create(Path.Combine(FilePath, Path.GetFileName(FileName))))
                {
                    ResourceStream.CopyTo(ProjectFileStream);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Cannot copy project file. Error: {e.Message}");
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ProjectToLocationThenExtractZip(Assembly ExecutingAssembly, string FileName)
    {
        try
        {
            string ShortFileName = Path.GetFileName(FileName);
            string ActualResourceName = ExecutingAssembly.GetManifestResourceNames().FirstOrDefault(name => name.EndsWith("." + ShortFileName, StringComparison.OrdinalIgnoreCase));

            if (ActualResourceName == null)
            {
                throw new FileNotFoundException($"Could not find embedded resource ending with '.{ShortFileName}'");
            }

            string DirectoryPath = Path.GetDirectoryName(FileName);
            if (!string.IsNullOrEmpty(DirectoryPath) && !Directory.Exists(DirectoryPath))
            {
                Directory.CreateDirectory(DirectoryPath);
            }

            using (Stream ResourceStream = ExecutingAssembly.GetManifestResourceStream(ActualResourceName))
            {
                using (FileStream ProjectFileStream = File.Create(FileName))
                {
                    ResourceStream.CopyTo(ProjectFileStream);
                }
            }

            ExtractZipSafe(FileName, DirectoryPath);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Cannot copy project file. Error: {e.Message}");
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ProjectToLocationThenExtractZip(Assembly ExecutingAssembly, string FileName, string FilePath)
    {
        try
        {
            string ShortFileName = Path.GetFileName(FileName);
            string ActualResourceName = ExecutingAssembly.GetManifestResourceNames().FirstOrDefault(name => name.EndsWith("." + ShortFileName, StringComparison.OrdinalIgnoreCase));

            if (ActualResourceName == null)
            {
                throw new FileNotFoundException($"Could not find embedded resource ending with '.{ShortFileName}'");
            }

            if (!string.IsNullOrEmpty(FilePath) && !Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }

            using (Stream ResourceStream = ExecutingAssembly.GetManifestResourceStream(ActualResourceName))
            {
                using (FileStream ProjectFileStream = File.Create(Path.Combine(FilePath, ShortFileName)))
                {
                    ResourceStream.CopyTo(ProjectFileStream);
                }
            }

            ExtractZipSafe(FileName, FilePath);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Cannot copy project file. Error: {e.Message}");
        }
    }
}