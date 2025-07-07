using System.IO.Compression;

using DcsTranslateTool.Share.Services;

using Xunit;

namespace DcsTranslateTool.Share.Tests.Services;

public class ZipServiceTests
{
    [Fact(DisplayName = "ファイルパスを指定して追加したとき指定パスのエントリが作成される")]
    public void AddFile_AddsFile()
    {
        // Arrange
        string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir);
        string zipPath = Path.Combine(tempDir, "test.zip");
        string filePath = Path.Combine(tempDir, "file.txt");
        File.WriteAllText(filePath, "sample");
        var service = new ZipService();

        // Act
        service.AddFile(zipPath, filePath, "entry.txt");

        // Assert
        using ZipArchive archive = ZipFile.OpenRead(zipPath);
        Assert.Contains("entry.txt", archive.Entries.Select(e => e.FullName));
        Directory.Delete(tempDir, true);
    }

    [Fact(DisplayName = "バイト列を指定して追加したとき指定パスのエントリが作成される")]
    public void AddBytes_AddsBytes()
    {
        // Arrange
        string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir);
        string zipPath = Path.Combine(tempDir, "test.zip");
        byte[] data = [1, 2, 3];
        var service = new ZipService();

        // Act
        service.AddBytes(zipPath, data, "bytes.bin");

        // Assert
        using ZipArchive archive = ZipFile.OpenRead(zipPath);
        ZipArchiveEntry entry = archive.GetEntry("bytes.bin")!;
        using Stream stream = entry.Open();
        byte[] actual = new byte[data.Length];
        stream.Read(actual);
        Assert.Equal(data, actual);
        Directory.Delete(tempDir, true);
    }

    [Fact(DisplayName = "存在しないファイルを追加しようとしたとき例外が送出される")]
    public void AddFile_FileNotFound_Throws()
    {
        // Arrange
        string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir);
        string zipPath = Path.Combine(tempDir, "test.zip");
        var service = new ZipService();

        // Act & Assert
        Assert.Throws<FileNotFoundException>(() => service.AddFile(zipPath, Path.Combine(tempDir, "nofile.txt"), "entry.txt"));
        Directory.Delete(tempDir, true);
    }

    [Fact(DisplayName = "ファイルのパスを指定して削除したとき該当エントリが削除される")]
    public void DeleteEntry_FilePath()
    {
        // Arrange
        string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir);
        string zipPath = Path.Combine(tempDir, "test.zip");
        using ZipArchive archive = ZipFile.Open(zipPath, ZipArchiveMode.Create);
        archive.CreateEntry("dir/file1.txt");
        archive.CreateEntry("dir/file2.txt");
        archive.Dispose();
        var service = new ZipService();

        // Act
        service.DeleteEntry(zipPath, "dir/file1.txt");

        // Assert
        using ZipArchive check = ZipFile.OpenRead(zipPath);
        Assert.DoesNotContain("dir/file1.txt", check.Entries.Select(e => e.FullName));
        Assert.Contains("dir/file2.txt", check.Entries.Select(e => e.FullName));
        Directory.Delete(tempDir, true);
    }

    [Fact(DisplayName = "ディレクトリのパスを指定して削除したとき配下のエントリがすべて削除される")]
    public void DeleteEntry_DirectoryPath()
    {
        // Arrange
        string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir);
        string zipPath = Path.Combine(tempDir, "test.zip");
        using ZipArchive archive = ZipFile.Open(zipPath, ZipArchiveMode.Create);
        archive.CreateEntry("dir/file1.txt");
        archive.CreateEntry("dir/file2.txt");
        archive.CreateEntry("other.txt");
        archive.Dispose();
        var service = new ZipService();

        // Act
        service.DeleteEntry(zipPath, "dir");

        // Assert
        using ZipArchive check = ZipFile.OpenRead(zipPath);
        Assert.DoesNotContain(check.Entries, e => e.FullName.StartsWith("dir/"));
        Assert.Single(check.Entries);
        Directory.Delete(tempDir, true);
    }

    [Fact(DisplayName = "一覧取得を行ったとき全エントリのパスが返される")]
    public void GetEntryNames_ReturnsList()
    {
        // Arrange
        string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir);
        string zipPath = Path.Combine(tempDir, "test.zip");
        using ZipArchive archive = ZipFile.Open(zipPath, ZipArchiveMode.Create);
        archive.CreateEntry("a.txt");
        archive.CreateEntry("b/c.txt");
        archive.Dispose();
        var service = new ZipService();

        // Act
        IReadOnlyList<string> names = service.GetEntryNames(zipPath);

        // Assert
        Assert.Contains("a.txt", names);
        Assert.Contains("b/c.txt", names);
        Directory.Delete(tempDir, true);
    }
}
