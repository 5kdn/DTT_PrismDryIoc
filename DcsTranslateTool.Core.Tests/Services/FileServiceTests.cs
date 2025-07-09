using DcsTranslateTool.Core.Services;

using Newtonsoft.Json;

using Xunit;

namespace DcsTranslateTool.Core.Tests.XUnit;

public class FileServiceTests : IDisposable {
    private readonly string _tempDir;
    private readonly string _fileName;
    private readonly string _fileData;
    private readonly string _filePath;

    public FileServiceTests() {
        _tempDir = Path.Combine( Path.GetTempPath(), Guid.NewGuid().ToString() );
        Directory.CreateDirectory( _tempDir );

        _fileName = "Tests.json";
        _fileData = "Lorem ipsum dolor sit amet";
        _filePath = Path.Combine( _tempDir, _fileName );
    }

    public void Dispose() {
        if(Directory.Exists( _tempDir )) Directory.Delete( _tempDir, true );
    }


    [Fact( DisplayName = "Save関数は正常にファイルを書き出せる" )]
    public void TestSaveFile() {
        // Arrange
        var fileService = new FileService();

        // Act
        fileService.SaveToJson( _tempDir, _fileName, _fileData );

        // Assert
        if(File.Exists( _filePath )) {
            var jsonContentFile = File.ReadAllText(_filePath);
            var contentFile = JsonConvert.DeserializeObject<string>(jsonContentFile);
            Assert.Equal( _fileData, contentFile );
        }
        else {
            Assert.Fail( $"File not exist: {_filePath}" );
        }
    }

    [Fact( DisplayName = "Read関数は正常にファイルを読み込める" )]
    public void TestReadFile() {
        // Arrange
        File.WriteAllText( _filePath, JsonConvert.SerializeObject( _fileData ) );
        var fileService = new FileService();

        // Act
        var cacheData = fileService.ReadFromJson<string>(_tempDir, _fileName);

        // Assert
        Assert.Equal( _fileData, cacheData );
    }

    [Fact( DisplayName = "Delete関数は正常にファイルを削除する" )]
    public void TestDeleteFile() {
        // Arrange
        File.WriteAllText( _filePath, _fileData );
        var fileService = new FileService();

        // Act
        fileService.Delete( _tempDir, _fileName );

        // Assert
        Assert.False( File.Exists( _filePath ) );
    }

    [Fact( DisplayName = "GetFileTree関数は正常にFileTreeを返す" )]
    public void GetFileTree_Returns_FileTree() {
        Directory.CreateDirectory( Path.Combine( _tempDir, "b", "d" ) );
        File.WriteAllText( Path.Combine( _tempDir, "a.txt" ), "" );
        File.WriteAllText( Path.Combine( _tempDir, "b", "c.txt" ), "" );
        var service = new FileService();
        // Act
        var actual = service.GetFileTree( _tempDir );
        // Assert
        Assert.NotNull( actual );
        Assert.Equal( Path.GetFileName( _tempDir ), actual.Name );
        Assert.Equal( _tempDir, actual.AbsolutePath );
        Assert.True( actual.IsDirectory );
        Assert.Equal( 2, actual.Children.Count );

        var child0 = actual.Children[0];
        Assert.NotNull( child0 );
        Assert.Equal( "a.txt", child0.Name );
        Assert.Equal( Path.Combine( _tempDir, "a.txt" ), child0.AbsolutePath );
        Assert.False( child0.IsDirectory );
        Assert.Empty( child0.Children );

        var child1 = actual.Children[1];
        Assert.NotNull( child1 );
        Assert.Equal( "b", child1.Name );
        Assert.Equal( Path.Combine( _tempDir, "b" ), child1.AbsolutePath );
        Assert.True( child1.IsDirectory );
        Assert.Empty( child1.Children );


    }
}
