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
        Assert.Empty( result );
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
        var children = sut.GetChildren(entry).ToArray();

        // Assert
        Assert.Equal( 2, children.Length );
        Assert.Contains( children, c => c.IsDirectory && c.Name == "sub" && c.AbsolutePath == subDir );
        Assert.Contains( children, c => !c.IsDirectory && c.Name == "file.txt" && c.AbsolutePath == file );
    }

    [Fact]
    public void ファイルエントリに対してGetChildrenを実行したとき空列挙が返る() {
        // Arrange
        var filePath = Path.Join(_tempDir, "file.txt");
        File.WriteAllText( filePath, null );
        var entry = new FileEntry("file.txt", filePath, false);
        var sut = new FileEntryService();

        // Act
        var result = sut.GetChildren(entry);

        // Assert
        Assert.Empty( result );
    }

    [Fact]
    public void 存在しないディレクトリパスに対してGetChildrenを実行したとき空列挙が返る() {
        // Arrange
        var notExistDir = Path.Join(_tempDir, "notExist");
        var entry = new FileEntry("notExist", notExistDir, true);
        var sut = new FileEntryService();

        // Act
        var result = sut.GetChildren(entry);

        // Assert
        Assert.Empty( result );
    }

    [Fact]
    public void ディレクトリ列挙時に例外が発生したとき空列挙が返る() {
        // Arrange
        var noPermDir = Path.Join(_tempDir, "noPermDir");
        Directory.CreateDirectory( noPermDir );
        // アクセス不可にする
        var dirInfo = new DirectoryInfo(noPermDir);
        dirInfo.Attributes |= FileAttributes.ReadOnly;
        var entry = new FileEntry("noPermDir", noPermDir, true);
        var sut = new FileEntryService();

        try {
            // Act
            var result = sut.GetChildren(entry);

            // Assert
            Assert.Empty( result );
        }
        finally {
            dirInfo.Attributes &= ~FileAttributes.ReadOnly;
        }
    }

    [Fact]
    public void アクセス不可のディレクトリに対してGetChildrenを実行したとき空列挙が返る() {
        // Arrange
        var dir = Path.Join(_tempDir, "protected");
        Directory.CreateDirectory( dir );
        var dirInfo = new DirectoryInfo(dir);

        // アクセス不可属性（Windowsの場合は読み取り専用だけでは防げないので実際にはACL等で対処するのが理想ですが、ここでは疑似的な例）
        dirInfo.Attributes |= FileAttributes.ReadOnly;
        var entry = new FileEntry("protected", dir, true);
        var sut = new FileEntryService();
        try {
            // Act
            var result = sut.GetChildren(entry);

            // Assert
            Assert.Empty( result );
        }
        finally {
            // 元に戻す（削除できるようにする）
            dirInfo.Attributes &= ~FileAttributes.ReadOnly;
        }
    }
    #endregion
}