using System.Text;

using DcsTranslateTool.Core.Services;

using Newtonsoft.Json;

using Xunit;

namespace DcsTranslateTool.Core.Tests.Services;

public class DummyClass( string Name, int age ) {
    public string Name { get; set; } = Name;
    public int Age { get; set; } = age;
}

public class FileServiceTests : IDisposable {
    private readonly string _tempDir;

    public FileServiceTests() {
        _tempDir = Path.Join( Path.GetTempPath(), Guid.NewGuid().ToString() );
        Directory.CreateDirectory( _tempDir );
    }

    public void Dispose() {
        if(Directory.Exists( _tempDir )) Directory.Delete( _tempDir, true );
        GC.SuppressFinalize( this );
    }

    #region ReadFromJson<T>

    [Fact]
    public void ファイルが存在しない状態でReadFromJsonを実行したときnullが返る() {
        // Arrange
        var sut = new FileService();

        // Act
        var actual = sut.ReadFromJson<string>(_tempDir, "notfound.json");

        // Assert
        Assert.Null( actual );
    }

    [Fact]
    public void ファイルが存在する状態でReadFromJsonを実行したとき内容が返る() {
        // Arrange
        string filePath = Path.Join(_tempDir, "data.json");
        File.WriteAllText( filePath, "{\"name\": \"John\",\"age\": \"42\"}" );
        var sut = new FileService();

        // Act
        var actual = sut.ReadFromJson<DummyClass>(_tempDir, "data.json");

        // Assert
        Assert.NotNull( actual );
        Assert.IsType<DummyClass>( actual );
        Assert.Equal( "John", actual.Name );
        Assert.Equal( 42, actual.Age );
    }

    [Fact]
    public void 不正なJSONファイルが存在する状態でReadFromJsonを実行したときJsonExceptionが送出される() {
        // Arrange
        string filePath = Path.Join(_tempDir, "data.json");
        File.WriteAllText( filePath, "{ invalid json }" );
        var sut = new FileService();

        // Act & Assert
        Assert.Throws<JsonException>( () => sut.ReadFromJson<string>( _tempDir, "data.json" ) );
    }

    [Fact]
    [Trait( "Category", "WindowsOnly" )]
    public void ファイル読み込み時にIOExceptionが発生したときIOExceptionがラップされて送出される() {
        // Arrange
        var sut = new FileService();
        var tempFile = Path.Join(_tempDir, "ioerror.json");
        File.WriteAllText( tempFile, JsonConvert.SerializeObject( "data" ) );
        // ファイルを排他ロックしてIOExceptionを強制
        using var fs = new FileStream(tempFile, FileMode.Open, FileAccess.Read, FileShare.None);

        // Act & Assert
        Assert.Throws<IOException>( () => sut.ReadFromJson<string>( _tempDir, "ioerror.json" ) );
    }

    [Fact]
    public void FolderPathが空のときReadFromJsonを実行したときArgumentExceptionが送出される() {
        // Arrange
        var sut = new FileService();

        // Act & Assert
        Assert.Throws<ArgumentException>( () => sut.ReadFromJson<string>( "", "data.json" ) );
    }

    [Fact]
    public void FileNameが空のときReadFromJsonを実行したときArgumentExceptionが送出される() {
        // Arrange
        var sut = new FileService();

        // Act & Assert
        Assert.Throws<ArgumentException>( () => sut.ReadFromJson<string>( _tempDir, "" ) );
    }

    #endregion

    #region SaveToJson<T>

    [Fact]
    public void フォルダが存在する状態でSaveToJsonを実行したときファイルが保存される() {
        // Arrange
        string filePath = Path.Join(_tempDir, "data.json");
        var sut = new FileService();

        // Act
        sut.SaveToJson<DummyClass>( _tempDir, "data.json", new DummyClass( "John", 42 ) );

        // Assert
        Assert.True( File.Exists( filePath ) );
        var actual = JsonConvert.DeserializeObject<DummyClass>(File.ReadAllText(filePath));
        Assert.Equal( "John", actual.Name );
        Assert.Equal( 42, actual.Age );
    }

    [Fact]
    public void フォルダが存在しない状態でSaveToJsonを実行したときフォルダが作成されファイルが保存される() {
        // Arrange
        var newDir = Path.Join(_tempDir, "NewDir");
        const string fileName = "data.json";
        const string content = "test";
        var sut = new FileService();

        // Act
        sut.SaveToJson( newDir, fileName, content );

        // Assert
        var path = Path.Join(newDir, fileName);
        Assert.True( File.Exists( path ) );
        var actual = JsonConvert.DeserializeObject<string>(File.ReadAllText(path));
        Assert.Equal( content, actual );
    }

    [Fact]
    public void シリアライズできない型でSaveToJsonを実行したときJsonExceptionが送出される() {
        // Arrange
        var sut = new FileService();
        var content = new StreamReader(new MemoryStream()); // シリアライズ不可型

        // Act & Assert
        Assert.Throws<JsonException>( () => sut.SaveToJson( _tempDir, "data.json", content ) );
    }

    [Fact]
    public void フォルダ作成や書き込み時にIOExceptionが発生したときIOExceptionがラップされて送出される() {
        // Arrange
        const string fileName = "ioerror.json";
        const string content = "test";
        var newDir = Path.Join(_tempDir, "NewDir");
        Directory.CreateDirectory( newDir );
        var targetFile = Path.Join(newDir, fileName);
        File.WriteAllText( targetFile, "dummy" );
        // ファイルをロックしてIOExceptionを強制
        using var fs = new FileStream(targetFile, FileMode.Open, FileAccess.Write, FileShare.None);
        var sut = new FileService();

        // Act & Assert
        Assert.Throws<IOException>( () => sut.SaveToJson( newDir, fileName, content ) );
    }

    [Fact]
    public void FolderPathが空のときSaveToJsonを実行したときArgumentExceptionが送出される() {
        // Arrange
        var sut = new FileService();

        // Act & Assert
        Assert.Throws<ArgumentException>( () => sut.SaveToJson<string>( "", "data.json", "dummy" ) );
    }

    [Fact]
    public void FileNameが空のときSaveToJsonを実行したときArgumentExceptionが送出される() {
        // Arrange
        var sut = new FileService();

        // Act & Assert
        Assert.Throws<ArgumentException>( () => sut.SaveToJson<string>( _tempDir, "", "dummy" ) );
    }

    #endregion

    #region Delete

    [Fact]
    public void ファイルが存在する状態でDeleteを実行したときファイルが削除される() {
        // Arrange
        string filePath = Path.Join(_tempDir, "data.json");
        File.WriteAllText( filePath, "dummy" );
        var sut = new FileService();

        // Act
        sut.Delete( _tempDir, "data.json" );

        // Assert
        Assert.False( File.Exists( filePath ) );
    }

    [Fact]
    public void ファイルが存在しない状態でDeleteを実行したとき例外が発生しない() {
        // Arrange
        var sut = new FileService();

        // Act
        var ex = Record.Exception(() => sut.Delete(_tempDir, "data.json"));

        // Assert
        Assert.Null( ex );
    }

    [Fact]
    public void ファイル削除時にIOExceptionが発生したときIOExceptionがラップされて送出される() {
        // Arrange
        string filePath = Path.Join(_tempDir, "data.json");
        File.WriteAllText( filePath, "dummy" );
        var sut = new FileService();
        // ファイルをロックしてIOExceptionを強制
        using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);

        // Act & Assert
        Assert.Throws<IOException>( () => sut.Delete( _tempDir, "data.json" ) );
    }

    [Fact]
    public void FolderPathが空のときDeleteを実行したときArgumentExceptionが送出される() {
        // Arrange
        var sut = new FileService();

        // Act & Assert
        Assert.Throws<ArgumentException>( () => sut.Delete( "", "data.json" ) );
    }

    [Fact]
    public void FileNameが空のときDeleteを実行したときArgumentExceptionが送出される() {
        // Arrange
        var sut = new FileService();

        // Act & Assert
        Assert.Throws<ArgumentException>( () => sut.Delete( _tempDir, "" ) );
    }

    #endregion

    #region SaveAsync<string>

    [Fact]
    public async Task 有効なパスでSaveAsyncを実行したときファイルが保存される() {
        // Arrange
        var path = Path.Join(_tempDir, "test.txt");
        const string content = "abc";
        var sut = new FileService();

        // Act
        await sut.SaveAsync( path, content );

        // Assert
        Assert.True( File.Exists( path ) );
        Assert.Equal( content, File.ReadAllText( path ) );
    }

    [Fact]
    public async Task ディレクトリが存在しないパスでSaveAsyncを実行したときディレクトリが作成されファイルが保存される() {
        // Arrange
        var dir = Path.Join(_tempDir, "newdir");
        var path = Path.Join(dir, "test.txt");
        const string content = "abc";
        var sut = new FileService();

        // Act
        await sut.SaveAsync( path, content );

        // Assert
        Assert.True( File.Exists( path ) );
        Assert.Equal( content, File.ReadAllText( path ) );
    }

    [Fact]
    public async Task 保存先パスが空でSaveAsyncを実行したときArgumentExceptionが送出される() {
        // Arrange
        var sut = new FileService();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>( async () => await sut.SaveAsync( "", "abc" ) );
    }

    [Fact]
    public async Task 書き込み時にIOExceptionが発生したときIOExceptionがラップされて送出される() {
        // Arrange
        var file = Path.Join(_tempDir, "locked.txt");
        File.WriteAllText( file, "lock" );
        await using var fs = new FileStream(file, FileMode.Open, FileAccess.Write, FileShare.None);
        var sut = new FileService();

        // Act & Assert
        await Assert.ThrowsAsync<IOException>( async () => await sut.SaveAsync( file, "test" ) );
    }

    #endregion

    #region SaveAsync<byte[]>

    [Fact]
    public async Task 有効なパスでバイト配列のSaveAsyncを実行したときファイルが保存される() {
        // Arrange
        var path = Path.Join(_tempDir, "test2.bin");
        var content = Encoding.UTF8.GetBytes("abc");
        var sut = new FileService();

        // Act
        await sut.SaveAsync( path, content );

        // Assert
        Assert.True( File.Exists( path ) );
        Assert.Equal( content, File.ReadAllBytes( path ) );
    }

    [Fact]
    public async Task ディレクトリが存在しないパスでバイト配列のSaveAsyncを実行したときディレクトリが作成されファイルが保存される() {
        // Arrange
        var dir = Path.Join(_tempDir, "newdir2");
        var path = Path.Join(dir, "test2.bin");
        var content = Encoding.UTF8.GetBytes("abc");
        var sut = new FileService();

        // Act
        await sut.SaveAsync( path, content );

        // Assert
        Assert.True( File.Exists( path ) );
        Assert.Equal( content, File.ReadAllBytes( path ) );
    }

    [Fact]
    public async Task 保存先パスがnullでバイト配列のSaveAsyncを実行したときArgumentExceptionが送出される() {
        // Arrange
        var sut = new FileService();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>( async () => await sut.SaveAsync( null!, Encoding.UTF8.GetBytes( "abc" ) ) );
    }

    [Fact]
    public async Task 保存先パスが空でバイト配列のSaveAsyncを実行したときArgumentExceptionが送出される() {
        // Arrange
        var sut = new FileService();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>( async () => await sut.SaveAsync( "", Encoding.UTF8.GetBytes( "abc" ) ) );
    }

    [Fact]
    public async Task バイト配列の書き込み時にIOExceptionが発生したときIOExceptionがラップされて送出される() {
        // Arrange
        var file = Path.Join(_tempDir, "locked2.bin");
        File.WriteAllText( file, "lock" );
        await using var fs = new FileStream(file, FileMode.Open, FileAccess.Write, FileShare.None);
        var sut = new FileService();

        // Act & Assert
        await Assert.ThrowsAsync<IOException>( async () => await sut.SaveAsync( file, Encoding.UTF8.GetBytes( "test" ) ) );
    }

    #endregion
}
