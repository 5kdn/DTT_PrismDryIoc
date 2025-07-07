using System.IO.Compression;

using DcsTranslateTool.Share.Services;

using Xunit;

namespace DcsTranslateTool.Share.Tests.Services;

public class ZipServiceTests : IDisposable {
    private readonly string _tempDir;
    public ZipServiceTests() {
        _tempDir = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid().ToString()
        );
        Directory.CreateDirectory( _tempDir );
    }

    public void Dispose() => Directory.Delete( _tempDir, true );

    [Fact( DisplayName = "一覧取得を行ったとき全エントリのパスが返される" )]
    public void GetEntries_ReturnsList() {
        // Arrange
        string zipPath = Path.Combine(_tempDir, "test.zip");
        // create zip
        using(var zip = ZipFile.Open( zipPath, ZipArchiveMode.Create )) {
            zip.CreateEntry( "a.txt" );
            zip.CreateEntry( "b/c.txt" );
        }
        var service = new ZipService();

        // Act
        IReadOnlyList<string> entries = service.GetEntries(zipPath);

        // Assert
        Assert.Equal( 2, entries.Count );
        Assert.Contains( "a.txt", entries );
        Assert.Contains( "b/c.txt", entries );
    }

    [Fact( DisplayName = "存在しないzipファイルを指定したとき例外が送出される" )]
    public void GetEntries_FileNotFound_Throws() {
        // Arrange & Act
        string noExistPath = Path.Combine(_tempDir, "no_exist.zip");
        var service = new ZipService();

        // Assert
        Assert.Throws<FileNotFoundException>( () => {
            service.GetEntries( noExistPath );
        } );
    }

    [Fact( DisplayName = "ファイルパスを指定して追加したとき指定パスのエントリが作成される" )]
    public void AddEntry_AddsFile() {
        // Arrange
        string zipPath = Path.Combine(_tempDir, "test.zip");
        string filePath = Path.Combine(_tempDir, "file.txt");
        File.WriteAllText( filePath, "sample" );
        using(var zip = ZipFile.Open( zipPath, ZipArchiveMode.Create )) { }

        var service = new ZipService();

        // Act
        service.AddEntry( zipPath, "entry/of/file.txt", filePath );

        // Assert
        using ZipArchive actualZip = ZipFile.OpenRead(zipPath);
        Assert.Single( actualZip.Entries );
        Assert.Equal( "entry/of/file.txt", actualZip.Entries[0].FullName );
        var actualFileText = new StreamReader( actualZip.Entries[0].Open() ).ReadToEnd();
        Assert.Equal( "sample", actualFileText );
    }

    [Fact( DisplayName = "AddEntry時存在しないzipファイルを指定した時例外が送出される" )]
    public void AddEntry_FileNotFound_Throws_ZipFilePath() {
        // Arrange
        string zipPath = Path.Combine(_tempDir, "test.zip");
        string filePath = Path.Combine(_tempDir, "file.txt");
        File.WriteAllText( filePath, "sample" );
        var service = new ZipService();

        // Act & Assert
        Assert.Throws<FileNotFoundException>( () => {
            service.AddEntry( zipPath, "path/to/entry", filePath );
        } );
    }

    [Fact( DisplayName = "AddEntry時追加ファイルに存在しないファイルを指定した時例外が送出される" )]
    public void AddEntry_FileNotFound_Throws_FilePath() {
        // Arrange
        string zipPath = Path.Combine(_tempDir, "test.zip");
        string filePath = Path.Combine(_tempDir, "no_exist.txt");
        using(var zip = ZipFile.Open( zipPath, ZipArchiveMode.Create )) { }
        var service = new ZipService();

        // Act & Assert
        Assert.Throws<FileNotFoundException>( () => {
            service.AddEntry( zipPath, "path/to/entry", filePath );
        } );
    }

    [Fact( DisplayName = "AddEntry時追加ファイルにディレクトリを指定した時例外が送出される" )]
    public void AddEntry_FileNotFound_Throws_FilePath_With_DirectoryPath() {
        // Arrange
        string zipPath = Path.Combine(_tempDir, "test.zip");
        string dirPath = Path.Combine(_tempDir, "dir");
        Directory.CreateDirectory( dirPath );
        using(var zip = ZipFile.Open( zipPath, ZipArchiveMode.Create )) { }
        var service = new ZipService();

        // Act & Assert
        Assert.Throws<FileNotFoundException>( () => {
            service.AddEntry( zipPath, "path/to/entry", dirPath );
        } );
    }

    [Fact( DisplayName = "バイト列を指定して追加したとき指定パスのエントリが作成される" )]
    public void AddEntry_AddsBytes() {
        // Arrange
        byte[] expectedData = [0x1, 0x2, 0x3];
        string zipPath = Path.Combine(_tempDir, "test.zip");
        using(var zip = ZipFile.Open( zipPath, ZipArchiveMode.Create )) { }
        var service = new ZipService();

        // Act
        service.AddEntry( zipPath, "entry/of/bytes.bin", expectedData );

        // Assert
        using ZipArchive actualZip = ZipFile.OpenRead(zipPath);
        Assert.Single( actualZip.Entries );
        Assert.Equal( "entry/of/bytes.bin", actualZip.Entries[0].FullName );

        var ms = new MemoryStream();
        actualZip.Entries[0].Open().CopyTo( ms );
        byte[] actualData = ms.ToArray();
        Assert.Equal( expectedData, actualData );
    }

    [Fact( DisplayName = "AddEntry時存在しないzipファイルを指定した時例外が送出される" )]
    public void AddEntry_FileNotFound_Throws_With_No_Exist_ZipPath() {
        // Arrange
        byte[] expectedData = [0x1, 0x2, 0x3];
        string zipPath = Path.Combine(_tempDir, "no_exist.zip");
        var service = new ZipService();

        // Act & Assert
        Assert.Throws<FileNotFoundException>( () => {
            service.AddEntry( zipPath, "entry/of/bytes.bin", expectedData );
        } );
    }

    [Fact( DisplayName = "ファイルのパスを指定して削除したとき該当エントリが削除される" )]
    public void DeleteEntry_FilePath() {
        // Arrange
        string zipPath = Path.Combine(_tempDir, "test.zip");
        // create zip
        using(var zip = ZipFile.Open( zipPath, ZipArchiveMode.Create )) {
            zip.CreateEntry( "dir/file1.txt" );
            zip.CreateEntry( "dir/file2.txt" );
        }
        var service = new ZipService();

        // Act
        service.DeleteEntry( zipPath, "dir/file1.txt" );

        // Assert
        using ZipArchive actualZip = ZipFile.OpenRead(zipPath);
        string[] actualEntries = actualZip.Entries.Select(e=>e.FullName).ToArray();

        Assert.Single( actualEntries );
        Assert.DoesNotContain( "dir/file1.txt", actualEntries );
        Assert.Contains( "dir/file2.txt", actualEntries );
    }

    [Fact( DisplayName = "ディレクトリのパスを指定して削除したとき配下のエントリがすべて削除される" )]
    public void DeleteEntry_DirectoryPath() {
        // Arrange
        string zipPath = Path.Combine(_tempDir, "test.zip");
        using(var zip = ZipFile.Open( zipPath, ZipArchiveMode.Create )) {
            zip.CreateEntry( "dir/file1.txt" );
            zip.CreateEntry( "dir/file2.txt" );
            zip.CreateEntry( "other.txt" );
        }
        var service = new ZipService();

        // Act
        service.DeleteEntry( zipPath, "dir" );

        // Assert
        using ZipArchive actualZip = ZipFile.OpenRead(zipPath);
        string[] actualEntries = actualZip.Entries.Select(e=>e.FullName).ToArray();
        Assert.Single( actualEntries );
        Assert.DoesNotContain( "dir/", actualEntries );
        Assert.Contains( "other.txt", actualEntries );
    }
}
