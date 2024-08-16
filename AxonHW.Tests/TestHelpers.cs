using System.Runtime.CompilerServices;

// Author: Colin Gilbert
namespace AxonHW.Tests
{
    public class TestHelpers
    {
        public static string ReadFile(string file, [CallerFilePath] string filePath = "")
        {
            var directoryPath = Path.GetDirectoryName(filePath);
            var fullPath = Path.Join(directoryPath, file);
            return File.ReadAllText(fullPath);
        }
    }
}
