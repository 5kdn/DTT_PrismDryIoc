using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Core.Services;

using Xunit;

namespace DcsTranslateTool.Core.Tests.Services;
public class FileEntryServiceTests : IDisposable {
    private readonly string _tempDir;

    public FileEntryServiceTests() {
        _tempDir = Path.Join( Path.GetTempPath(), Guid.NewGuid().ToString() );
        Directory.CreateDirectory( _tempDir );
    }

    public void Dispose() {
        if(Directory.Exists( _tempDir )) Directory.Delete( _tempDir, true );
        GC.SuppressFinalize( this );
    }

    #region GetChildren

    [Fact]
    public void ディレクトリが空の状態でGetChildrenを実行したとき空列挙が返る() {
        // Arrange
        var entry = new Entry("empty", "", true);
        var sut = new FileEntryService( _tempDir );

        // Act
        var result = sut.GetChildren(entry);

        // Assert
        Assert.True( result.IsSuccess );
        Assert.Equal( [], result.Value );
    }

    [Fact]
    public void ディレクトリ内にサブディレクトリとファイルが存在する状態でGetChildrenを実行したとき子要素が全て返る() {
        // Arrange
        var subDir = Path.Join(_tempDir, "sub");
        Directory.CreateDirectory( subDir );
        var file = Path.Join(_tempDir, "file.txt");
        File.WriteAllText( file, "test" );
        var entry = new Entry("parent", "", true);
        var sut = new FileEntryService( _tempDir );

        // Act
        var result = sut.GetChildren(entry);

        // Assert
        Assert.True( result.IsSuccess );
        var children = result.Value.ToArray();
        Assert.Equal( 2, children.Length );
        Assert.Contains( children, c => c.IsDirectory && c.Name == "sub" && c.Path == "sub" );
        Assert.Contains( children, c => !c.IsDirectory && c.Name == "file.txt" && c.Path == "file.txt" && c.LocalSha != null );
    }

    [Fact]
    public void ファイルエントリに対してGetChildrenを実行したとき失敗する() {
        // Arrange
        var filePath = Path.Join(_tempDir, "file.txt");
        File.WriteAllText( filePath, "test" );
        var entry = new Entry("file.txt", "file.txt", false);
        var sut = new FileEntryService( _tempDir );

        // Act
        var result = sut.GetChildren(entry);

        // Assert
        Assert.True( result.IsFailed );
        Assert.Contains( "ディレクトリではないエントリが指定されました", result.Errors[0].Message );
    }

    [Fact]
    public void 存在しないディレクトリパスに対してGetChildrenを実行したとき失敗する() {
        // Arrange
        var notExistDir = Path.Join(_tempDir, "notExist");
        var entry = new Entry("notExist", "notExist", true);
        var sut = new FileEntryService( _tempDir );

        // Act
        var result = sut.GetChildren(entry);

        // Assert
        Assert.False( result.IsSuccess );
    }

    #endregion

    #region GetChildrenRecursive

    [Fact]
    public void ディレクトリにネストされたファイルがある状態でGetChildrenRecursiveを実行したとき全ての子要素が返る() {
        // Arrange
        var subDir = Path.Join( _tempDir, "sub" );
        Directory.CreateDirectory( subDir );
        var nested = Path.Join( subDir, "nested.txt" );
        File.WriteAllText( nested, "test" );
        var sut = new FileEntryService( _tempDir );

        // Act
        var result = sut.GetChildrenRecursive( _tempDir );

        // Assert
        Assert.True( result.IsSuccess );
        Assert.Single( result.Value );
        Assert.Contains( result.Value, e => e.Path == "sub/nested.txt" );
    }

    [Fact]
    public void 存在しないディレクトリパスでGetChildrenRecursiveを実行したとき失敗する() {
        // Arrange
        var notExist = Path.Join( _tempDir, "none" );
        var sut = new FileEntryService( _tempDir );

        // Act
        var result = sut.GetChildrenRecursive( notExist );

        // Assert
        Assert.True( result.IsFailed );
    }

    #endregion
}
