using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Core.Services;

using Xunit;

namespace DcsTranslateTool.Win.Tests.Services;
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

    [Fact]
    public void ディレクトリが存在するとき子ディレクトリとファイルが取得できる() {
        // Arrange
        var subDir = Path.Join(_tempDir, "subdir");
        Directory.CreateDirectory( subDir );
        var file = Path.Join(_tempDir, "file.txt");
        File.WriteAllText( file, "test" );
        var entry = new FileEntry(_tempDir, true);
        var sut = new FileEntryService();

        // Act
        var children = sut.GetChildren(entry).ToList();

        // Assert
        Assert.Contains( children, c => c.IsDirectory && c.Name == "subdir" );
        Assert.Contains( children, c => !c.IsDirectory && c.Name == "file.txt" );
        Assert.Equal( 2, children.Count );
    }

    [Fact]
    public void ファイルを指定したとき子要素は返されない() {
        // Arrange
        var file = Path.Join(_tempDir, "file.txt");
        File.WriteAllText( file, "test" );
        var entry = new FileEntry(file, false);
        var sut = new FileEntryService();

        // Act
        var children = sut.GetChildren(entry).ToList();

        // Assert
        Assert.Empty( children );
    }

    [Fact]
    public void 存在しないディレクトリを指定したとき空列挙を返す() {
        // Arrange
        var notExistPath = Path.Join(_tempDir, "not_exist_dir");
        var entry = new FileEntry(notExistPath, true);
        var sut = new FileEntryService();

        // Act
        var children = sut.GetChildren(entry).ToList();

        // Assert
        Assert.Empty( children );
    }
}