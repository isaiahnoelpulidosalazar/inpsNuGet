using inpsNuGet;
using System.IO.Compression;
using System.Reflection;

internal class Program
{
    private static void Main(string[] args)
    {
        if (!File.Exists("motion_detection.zip"))
        {
            SimpleFileHandler.ProjectToLocation(Assembly.GetExecutingAssembly(), "motion_detection.zip");
            ZipFile.ExtractToDirectory("motion_detection.zip", Directory.GetCurrentDirectory());
            File.Delete("motion_detection.zip");
        }
        PyCS pyCS = new PyCS();
        if (!File.Exists("pipdone"))
        {
            pyCS.InstallPip();
            pyCS.PipLocal(new string[]
            {
                "numpy-2.2.6-cp313-cp313-win_amd64.whl",
                "opencv_python-4.12.0.88-cp37-abi3-win_amd64.whl",
                "imutils-0.5.4-py3-none-any.whl"
            });
            SimpleFileHandler.Write("pipdone", string.Empty);
        }
        pyCS.RunFile("motion_detection.py");
    }
}