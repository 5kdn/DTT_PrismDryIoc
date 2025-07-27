using System.IO.Compression;

using DcsTranslateTool.Core.Services;

using Xunit;

namespace DcsTranslateTool.Core.Tests.Services;

public class ZipServiceTests : IDisposable {
    private readonly string _tempDir;

    public ZipServiceTests() {
        _tempDir = Path.Join(
            Path.GetTempPath(),
            Guid.NewGuid().ToString()
        );
        Directory.CreateDirectory( _tempDir );
    }

    public void Dispose() {
        Directory.Delete( _tempDir, true );
        GC.SuppressFinalize( this );
    }

    #region GetEntries

    [Fact]
    public void 正常なzipファイルでGetEntriesを実行したとき全エントリー名が返る() {
        // Arrange
        var zipPath = Path.Join(_tempDir, "test.zip");
        using(var archive = ZipFile.Open( zipPath, ZipArchiveMode.Create )) {
            archive.CreateEntry( "foo.txt" );
            archive.CreateEntry( "bar/baz.txt" );
        }
        var sut = new ZipService();

        // Act
        var entries = sut.GetEntries(zipPath);

        // Assert
        Assert.Equal( 2, entries.Count );
        Assert.Contains( "foo.txt", entries );
        Assert.Contains( "bar/baz.txt", entries );
    }

    [Theory]
    [InlineData( "" )]
    [InlineData( " " )]
    public void Zipファイルパスが空のときGetEntriesを実行したときArgumentExceptionが送出される( string targetPath ) {
        // Arrange
        var sut = new ZipService();

        // Act & Assert
        Assert.Throws<ArgumentException>( () => sut.GetEntries( targetPath ) );
    }

    [Fact]
    public void ファイルが存在しないパスでGetEntriesを実行したときFileNotFoundExceptionが送出される() {
        // Arrange
        var zipPath = Path.Join(_tempDir, "notfound.zip");
        var sut = new ZipService();

        // Act & Assert
        Assert.Throws<FileNotFoundException>( () => sut.GetEntries( zipPath ) );
    }

    [Fact]
    public void 壊れたzipファイルでGetEntriesを実行したときInvalidDataExceptionが送出される() {
        // Arrange
        var zipPath = Path.Join(_tempDir, "broken.zip");
        File.WriteAllText( zipPath, "not a zip archive!" );
        var sut = new ZipService();

        // Act & Assert
        Assert.Throws<InvalidDataException>( () => sut.GetEntries( zipPath ) );
    }

    [Fact]
    public void アクセス不可なzipファイルでGetEntriesを実行したときIOExceptionが送出される() {
        // Arrange
        var zipPath = Path.Join(_tempDir, "locked.zip");
        using(var archive = ZipFile.Open( zipPath, ZipArchiveMode.Create )) {
            archive.CreateEntry( "foo.txt" );
        }
        using var fs = new FileStream(zipPath, FileMode.Open, FileAccess.Read, FileShare.None);
        var sut = new ZipService();

        // Act & Assert
        Assert.Throws<IOException>( () => sut.GetEntries( zipPath ) );
    }

    #endregion

    #region AddEntry(byte[])

    [Fact]
    [Trait( "Category", "WindowsOnly" )]
    public void 正常にバイト列をzipに追加できるAddEntryバイト列版() {
        // Arrange
        var service = new ZipService();
        var zipPath = Path.Join(_tempDir, "dummy.zip");
        byte[] data = [ 1, 2, 3, 4, 5 ];
        using(ZipFile.Open( zipPath, ZipArchiveMode.Create )) { }

        // Act
        service.AddEntry( zipPath, "data.bin", data );

        // Assert
        using ZipArchive actual = ZipFile.OpenRead( zipPath );
        var entry = actual.GetEntry("data.bin");
        Assert.NotNull( entry );
        using var stream = entry.Open();
        byte[] result = new byte[data.Length];
        int bytesRead = stream.Read(result, 0, result.Length);
        Assert.Equal( data.Length, bytesRead );
        Assert.Equal( data, result );
    }

    [Fact]
    [Trait( "Category", "WindowsOnly" )]
    public void Zipファイルパスが空でAddEntryバイト列版を呼ぶとArgumentExceptionになる() {
        // Arrange
        var service = new ZipService();
        byte[] data = [1, 2, 3];

        // Act & Assert
        Assert.Throws<ArgumentException>( () => service.AddEntry( "", "foo.txt", data ) );
    }

    [Fact]
    [Trait( "Category", "WindowsOnly" )]
    public void Zipファイルが存在しないAddEntryバイト列版でFileNotFoundExceptionになる() {
        // Arrange
        var service = new ZipService();
        byte[] data = [1, 2, 3];

        // Act & Assert
        Assert.Throws<FileNotFoundException>( () => service.AddEntry( "notfound.zip", "foo.txt", data ) );
    }

    [Fact]
    [Trait( "Category", "WindowsOnly" )]
    public void EntryPathが空でAddEntryバイト列版を呼ぶとArgumentExceptionになる() {
        // Arrange
        var service = new ZipService();
        var zipPath = Path.Join(_tempDir, "dummy.zip");
        using(ZipFile.Open( zipPath, ZipArchiveMode.Create )) { }
        byte[] data = [1, 2, 3];

        // Act & Assert
        Assert.Throws<ArgumentException>( () => service.AddEntry( zipPath, "", data ) );
    }

    [Fact]
    [Trait( "Category", "WindowsOnly" )]
    public void Dataが空配列でAddEntryバイト列版を呼ぶとArgumentExceptionになる() {
        // Arrange
        var service = new ZipService();
        var zipPath = Path.Join(_tempDir, "dummy.zip");
        using(ZipFile.Open( zipPath, ZipArchiveMode.Create )) { }

        // Act & Assert
        Assert.Throws<ArgumentException>( () => service.AddEntry( zipPath, "foo.txt", [] ) );
    }

    #endregion

    #region DeleteEntry

    [Fact]
    [Trait( "Category", "WindowsOnly" )]
    public void DeleteEntryは正常にエントリを削除できる() {
        // Arrange
        var service = new ZipService();
        var zipPath = Path.Join(_tempDir, "dummy.zip");
        using(var archive = ZipFile.Open( zipPath, ZipArchiveMode.Create )) {
            archive.CreateEntry( "a.txt" );
            archive.CreateEntry( "b.txt" );
            archive.CreateEntry( "dir/c.txt" );
        }

        // Act
        service.DeleteEntry( zipPath, "a.txt" );

        // Assert
        using var actual = ZipFile.OpenRead( zipPath );
        Assert.Null( actual.GetEntry( "a.txt" ) );
        Assert.NotNull( actual.GetEntry( "b.txt" ) );
        Assert.NotNull( actual.GetEntry( "dir/c.txt" ) );
    }

    [Fact]
    [Trait( "Category", "WindowsOnly" )]
    public void ディレクトリエントリを指定してDeleteEntryしたとき配下全て削除できる() {
        // Arrange
        var service = new ZipService();
        var zipPath = Path.Join(_tempDir, "dummy.zip");
        using(var archive = ZipFile.Open( zipPath, ZipArchiveMode.Create )) {
            archive.CreateEntry( "dir/a.txt" );
            archive.CreateEntry( "dir/b.txt" );
            archive.CreateEntry( "other.txt" );
        }

        // Act
        service.DeleteEntry( zipPath, "dir" );

        // Assert
        using var actual = ZipFile.OpenRead( zipPath );
        Assert.Null( actual.GetEntry( "dir/a.txt" ) );
        Assert.Null( actual.GetEntry( "dir/b.txt" ) );
        Assert.NotNull( actual.GetEntry( "other.txt" ) );
    }

    [Fact]
    [Trait( "Category", "WindowsOnly" )]
    public void Zipファイルパスが空でDeleteEntryしたときArgumentExceptionになる() {
        // Arrange
        var service = new ZipService();

        // Act & Assert
        Assert.Throws<ArgumentException>( () => service.DeleteEntry( "", "foo.txt" ) );
    }

    [Fact]
    [Trait( "Category", "WindowsOnly" )]
    public void Zipファイルが存在しないDeleteEntryでFileNotFoundExceptionになる() {
        // Arrange
        var service = new ZipService();

        // Act & Assert
        Assert.Throws<FileNotFoundException>( () => service.DeleteEntry( "notfound.zip", "foo.txt" ) );
    }

    [Fact]
    [Trait( "Category", "WindowsOnly" )]
    public void EntryPathが空でDeleteEntryしたときArgumentExceptionになる() {
        // Arrange
        var service = new ZipService();
        var zipPath = Path.Join(_tempDir, "dummy.zip");
        using(ZipFile.Open( zipPath, ZipArchiveMode.Create )) { }

        // Act & Assert
        Assert.Throws<ArgumentException>( () => service.DeleteEntry( zipPath, "" ) );
    }

    #endregion
}
