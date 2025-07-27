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
        var entry = new FileEntry("empty", _tempDir, true);
        var sut = new FileEntryService();

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
        File.WriteAllText( file, null );
        var entry = new FileEntry("parent", _tempDir, true);
        var sut = new FileEntryService();

        // Act
        var result = sut.GetChildren(entry);

        // Assert
        Assert.True( result.IsSuccess );
        var children = result.Value.ToArray();
        Assert.Equal( 2, children.Length );
        Assert.Contains( children, c => c.IsDirectory && c.Name == "sub" && c.AbsolutePath == subDir );
        Assert.Contains( children, c => !c.IsDirectory && c.Name == "file.txt" && c.AbsolutePath == file );
    }

    [Fact]
    public void ファイルエントリに対してGetChildrenを実行したとき失敗する() {
        // Arrange
        var filePath = Path.Join(_tempDir, "file.txt");
        File.WriteAllText( filePath, null );
        var entry = new FileEntry("file.txt", filePath, false);
        var sut = new FileEntryService();

        // Act
        var result = sut.GetChildren(entry);

        // Assert
        Assert.True( result.IsFailed );
        Assert.Contains( $"ディレクトリではないエントリが指定されました: {filePath}", result.Errors[0].Message );
    }

    [Fact]
    public void 存在しないディレクトリパスに対してGetChildrenを実行したとき失敗する() {
        // Arrange
        var notExistDir = Path.Join(_tempDir, "notExist");
        var entry = new FileEntry("notExist", notExistDir, true);
        var sut = new FileEntryService();

        // Act
        var result = sut.GetChildren(entry);

        // Assert
        Assert.False( result.IsSuccess );
    }

    #endregion
}