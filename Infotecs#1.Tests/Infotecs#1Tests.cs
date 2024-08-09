using System;
using System.IO;
using System.IO.Compression;
using Xunit;
using Moq;

public class ZipArchiveTests
{
    private readonly Mock<ILogger> _loggerMock;

    public ZipArchiveTests()
    {
        _loggerMock = new Mock<ILogger>();
    }

    [Fact]
    public void AddDirectoryToZip_DirectoryDoesNotExist_ShouldLogError()
    {
        // Arrange
        var archive = new ZipArchive(new MemoryStream(), ZipArchiveMode.Create, true);
        var nonExistentDirectory = "/path/to/nonexistent/directory";

        // Act
        AddDirectoryToZip(archive, nonExistentDirectory);

        // Assert
        _loggerMock.Verify(x => x.Error($"Директория {nonExistentDirectory} не найдена."), Times.Once);
    }

    [Fact]
    public void AddDirectoryToZip_ValidDirectory_ShouldAddFilesToArchive()
    {
        // Arrange
        var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDirectory);

        var filePath1 = Path.Combine(tempDirectory, "file1.txt");
        var filePath2 = Path.Combine(tempDirectory, "file2.txt");
        File.WriteAllText(filePath1, "Test content 1");
        File.WriteAllText(filePath2, "Test content 2");

        var archive = new ZipArchive(new MemoryStream(), ZipArchiveMode.Create, true);

        // Act
        AddDirectoryToZip(archive, tempDirectory);

        // Assert
        Assert.Equal(2, archive.Entries.Count);
        Assert.Contains(archive.Entries, e => e.Name == "file1.txt");
        Assert.Contains(archive.Entries, e => e.Name == "file2.txt");

        // Cleanup
        Directory.Delete(tempDirectory, true);
    }

    private void AddDirectoryToZip(ZipArchive archive, string sourceDirectory)
    {
        if (!Directory.Exists(sourceDirectory))
        {
            _loggerMock.Object.Error($"Директория {sourceDirectory} не найдена.");
            return;
        }

        foreach (var file in Directory.EnumerateFiles(sourceDirectory))
        {
            try
            {
                string fileName = Path.GetFileName(file);
                var entry = archive.CreateEntry(fileName, CompressionLevel.Optimal);

                using (var entryStream = entry.Open())
                using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    fileStream.CopyTo(entryStream);
                    _loggerMock.Object.Debug($"Файл {file} успешно добавлен в архив.");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                _loggerMock.Object.Error($"Ошибка доступа к файлу {file}: {ex.Message}");
            }
            catch (IOException ex)
            {
                _loggerMock.Object.Error($"Ошибка копирования файла {file}: {ex.Message}");
            }
            catch (Exception ex)
            {
                _loggerMock.Object.Error($"Произошла непредвиденная ошибка с файлом {file}: {ex.Message}");
            }
        }
    }

    public interface ILogger
    {
        void Error(string message);
        void Debug(string message);
    }
}
