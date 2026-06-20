using inpsNuGet;
using Xunit;

namespace inpsNuGetTest
{
    public class SimpleFileHandlerTest : IDisposable
    {
        private readonly string _tempFilePath;

        public SimpleFileHandlerTest()
        {
            // Set up a unique temporary file path for each test execution
            _tempFilePath = Path.Combine(Path.GetTempPath(), $"test_file_{Guid.NewGuid()}.txt");
        }

        // Clean up the temporary file after each test runs
        public void Dispose()
        {
            if (File.Exists(_tempFilePath))
            {
                try
                {
                    File.Delete(_tempFilePath);
                }
                catch
                {
                    // Ignore cleanup failures to prevent test runner crashes
                }
            }
        }

        #region Read, Write, and Append Tests

        [Fact]
        public void Write_CreatesFileWithSpecifiedContent()
        {
            // Setup
            string content = "Hello World, this is a write test.";
            SimpleFileHandler.Write(_tempFilePath, content);

            // Test
            Assert.True(File.Exists(_tempFilePath));
            Assert.Equal(content, File.ReadAllText(_tempFilePath));
        }

        [Fact]
        public void Read_RetrievesCorrectContentFromFile()
        {
            // Setup
            string content = "Sample data for testing read function.";
            File.WriteAllText(_tempFilePath, content);
            string result = SimpleFileHandler.Read(_tempFilePath);

            // Test
            Assert.Equal(content, result);
        }

        [Fact]
        public void Append_AddsContentToEndOfFile()
        {
            // Setup
            File.WriteAllText(_tempFilePath, "Line 1\n");
            string appendContent = "Line 2";
            SimpleFileHandler.Append(_tempFilePath, appendContent);

            // Test
            string finalContent = File.ReadAllText(_tempFilePath);
            Assert.Equal("Line 1\nLine 2", finalContent);
        }

        #endregion

        #region ProjectToLocation Exception Handling Tests

        [Fact]
        public void ProjectToLocation_NullAssembly_HandlesExceptionGracefully()
        {
            // The method should catch all exceptions internally and not crash the test suite
            var exception = Record.Exception(() => SimpleFileHandler.ProjectToLocation(null!, _tempFilePath));

            // Test
            Assert.Null(exception);
        }

        [Fact]
        public void ProjectToLocation_WithDirectory_NullAssembly_HandlesExceptionGracefully()
        {
            // Setup
            string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            // The directory overload should also catch all exceptions internally
            var exception = Record.Exception(() => SimpleFileHandler.ProjectToLocation(null!, "dummyFile.txt", tempDir));

            // Test
            Assert.Null(exception);

            // Cleanup the created directory if it exists
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }

        #endregion
    }
}
