using System.IO.Compression;

using DcsTranslateTool.Core.Services;

using Xunit;

namespace DcsTranslateTool.Tests.Core.Services;

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
        var result = sut.GetEntries(zipPath);

        // Assert
        Assert.True( result.IsSuccess );
        var entries = result.Value;
        Assert.Equal( 2, entries.Count );
        Assert.Contains( "foo.txt", entries );
        Assert.Contains( "bar/baz.txt", entries );
    }

    [Theory]
    [InlineData( "" )]
    [InlineData( " " )]
    public void Zipファイルパスが空のときGetEntriesを実行したときArgumentExceptionの失敗が返る( string targetPath ) {
        // Arrange
        var sut = new ZipService();

        // Act
        var result = sut.GetEntries(targetPath);

        // Assert
        Assert.True( result.IsFailed );
        Assert.Contains( "zip ファイルパスが null または空です", result.Errors[0].Message );
    }

    [Fact]
    public void ファイルが存在しないパスでGetEntriesを実行したときFileNotFoundExceptionの失敗が返る() {
        // Arrange
        var zipPath = Path.Join(_tempDir, "notfound.zip");
        var sut = new ZipService();

        // Act
        var result = sut.GetEntries( zipPath );

        // Assert
        Assert.True( result.IsFailed );
        Assert.Contains( "Could not find file", result.Errors[0].Message );
    }

    [Fact]
    public void 壊れたzipファイルでGetEntriesを実行したときInvalidDataExceptionの失敗が返る() {
        // Arrange
        var zipPath = Path.Join(_tempDir, "broken.zip");
        File.WriteAllText( zipPath, "not a zip archive!" );
        var sut = new ZipService();

        // Act
        var result = sut.GetEntries( zipPath );

        // Assert
        Assert.True( result.IsFailed );
        Assert.Contains( "End of Central Directory record could not be found.", result.Errors[0].Message );
    }

    [Fact]
    public void アクセス不可なzipファイルでGetEntriesを実行したときIOExceptionの失敗が返る() {
        // Arrange
        var zipPath = Path.Join(_tempDir, "locked.zip");
        using(var archive = ZipFile.Open( zipPath, ZipArchiveMode.Create )) {
            archive.CreateEntry( "foo.txt" );
        }
        using var fs = new FileStream(zipPath, FileMode.Open, FileAccess.Read, FileShare.None);
        var sut = new ZipService();

        // Act
        var result = sut.GetEntries(zipPath);

        // Assert
        Assert.True( result.IsFailed );
        Assert.Contains( "The process cannot access the file", result.Errors[0].Message );
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
        var result = service.AddEntry( zipPath, "data.bin", data );

        // Assert
        Assert.True( result.IsSuccess );

        using ZipArchive actual = ZipFile.OpenRead( zipPath );
        var actualEntry = actual.GetEntry("data.bin");
        Assert.NotNull( actualEntry );
        using var stream = actualEntry.Open();
        byte[] resultBytes = new byte[data.Length];
        int bytesRead = stream.Read(resultBytes, 0, resultBytes.Length);
        Assert.Equal( data.Length, bytesRead );
        Assert.Equal( data, resultBytes );
    }

    [Fact]
    [Trait( "Category", "WindowsOnly" )]
    public void Zipファイルパスが空でAddEntryバイト列版を呼ぶとArgumentExceptionになる() {
        // Arrange
        var service = new ZipService();
        byte[] data = [1, 2, 3];

        // Act
        var result = service.AddEntry( "", "foo.txt", data );

        // Assert
        Assert.True( result.IsFailed );
        Assert.Contains( "zip ファイルパスが null または空です", result.Errors[0].Message );
    }

    [Fact]
    [Trait( "Category", "WindowsOnly" )]
    public void Zipファイルが存在しないAddEntryバイト列版でFileNotFoundExceptionになる() {
        // Arrange
        var service = new ZipService();
        byte[] data = [1, 2, 3];

        // Act
        var result = service.AddEntry( "notfound.zip", "foo.txt", data );

        // Assert
        Assert.True( result.IsFailed );
        Assert.Contains( "ファイルが存在しません", result.Errors[0].Message );
    }

    [Fact]
    [Trait( "Category", "WindowsOnly" )]
    public void EntryPathが空でAddEntryバイト列版を呼ぶとArgumentExceptionになる() {
        // Arrange
        var service = new ZipService();
        var zipPath = Path.Join(_tempDir, "dummy.zip");
        using(ZipFile.Open( zipPath, ZipArchiveMode.Create )) { }
        byte[] data = [1, 2, 3];

        // Act
        var result = service.AddEntry( zipPath, "", data );

        // Assert
        Assert.True( result.IsFailed );
        Assert.Contains( "エントリーが null または空です", result.Errors[0].Message );
    }

    [Fact]
    [Trait( "Category", "WindowsOnly" )]
    public void Dataが空配列でAddEntryバイト列版を呼ぶとArgumentExceptionになる() {
        // Arrange
        var service = new ZipService();
        var zipPath = Path.Join(_tempDir, "dummy.zip");
        using(ZipFile.Open( zipPath, ZipArchiveMode.Create )) { }

        // Act
        var result = service.AddEntry( zipPath, "foo.txt", [] );

        // Assert
        Assert.True( result.IsFailed );
        Assert.Contains( "追加するデータが null または空です", result.Errors[0].Message );
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
        var result = service.DeleteEntry( zipPath, "a.txt" );

        // Assert
        Assert.True( result.IsSuccess );
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
        var result = service.DeleteEntry( zipPath, "dir" );

        // Assert
        Assert.True( result.IsSuccess );
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

        // Act
        var result = service.DeleteEntry( "", "foo.txt" );

        // Assert
        Assert.True( result.IsFailed );
        Assert.Contains( "zip ファイルパスが空です", result.Errors[0].Message );
    }

    [Fact]
    [Trait( "Category", "WindowsOnly" )]
    public void Zipファイルが存在しないDeleteEntryでFileNotFoundExceptionになる() {
        // Arrange
        var service = new ZipService();

        // Act
        var result = service.DeleteEntry( "notfound.zip", "foo.txt" );

        // Assert
        Assert.True( result.IsFailed );
        Assert.Contains( "ファイルが存在しません", result.Errors[0].Message );
    }

    [Fact]
    [Trait( "Category", "WindowsOnly" )]
    public void EntryPathが空でDeleteEntryしたときArgumentExceptionになる() {
        // Arrange
        var service = new ZipService();
        var zipPath = Path.Join(_tempDir, "dummy.zip");
        using(ZipFile.Open( zipPath, ZipArchiveMode.Create )) { }

        // Act
        var result = service.DeleteEntry( zipPath, "" );

        // Assert
        Assert.True( result.IsFailed );
        Assert.Contains( "zip ファイルパスが空です", result.Errors[0].Message );
    }

    #endregion
}