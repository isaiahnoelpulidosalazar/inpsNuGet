using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace inpsNuGetTestForm
{
    public class TestSimpleFileHandler
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
                string? ActualResourceName = ExecutingAssembly.GetManifestResourceNames().FirstOrDefault(name => name.EndsWith("." + ShortFileName, StringComparison.OrdinalIgnoreCase));

                if (ActualResourceName == null)
                {
                    throw new FileNotFoundException($"Could not find embedded resource ending with '.{ShortFileName}'");
                }

                string? DirectoryPath = Path.GetDirectoryName(FileName);
                if (!string.IsNullOrEmpty(DirectoryPath) && !Directory.Exists(DirectoryPath))
                {
                    Directory.CreateDirectory(DirectoryPath);
                }

                using (Stream? ResourceStream = ExecutingAssembly.GetManifestResourceStream(ActualResourceName))
                {
                    using (FileStream ProjectFileStream = File.Create(FileName))
                    {
                        ResourceStream?.CopyTo(ProjectFileStream);
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
                string? ActualResourceName = ExecutingAssembly.GetManifestResourceNames().FirstOrDefault(name => name.EndsWith("." + ShortFileName, StringComparison.OrdinalIgnoreCase));

                if (ActualResourceName == null)
                {
                    throw new FileNotFoundException($"Could not find embedded resource ending with '.{ShortFileName}'");
                }

                if (!string.IsNullOrEmpty(FilePath) && !Directory.Exists(FilePath))
                {
                    Directory.CreateDirectory(FilePath);
                }

                using (Stream? ResourceStream = ExecutingAssembly.GetManifestResourceStream(ActualResourceName))
                {
                    using (FileStream ProjectFileStream = File.Create(Path.Combine(FilePath, Path.GetFileName(FileName))))
                    {
                        ResourceStream?.CopyTo(ProjectFileStream);
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
                ProjectToLocation(ExecutingAssembly, FileName);
                ExtractZipSafe(FileName, "");
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
                ProjectToLocation(ExecutingAssembly, FileName, FilePath);
                ExtractZipSafe(FilePath + "\\" + FileName, FilePath);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Cannot copy project file. Error: {e.Message}");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ProjectToLocationThenExtractZipThenDelete(Assembly ExecutingAssembly, string FileName)
        {
            try
            {
                ProjectToLocationThenExtractZip(ExecutingAssembly, FileName);
                File.Delete(FileName);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Cannot copy project file. Error: {e.Message}");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ProjectToLocationThenExtractZipThenDelete(Assembly ExecutingAssembly, string FileName, string FilePath)
        {
            try
            {
                ProjectToLocationThenExtractZip(ExecutingAssembly, FileName, FilePath);
                File.Delete(FilePath + "\\" + FileName);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Cannot copy project file. Error: {e.Message}");
            }
        }
    }
}
