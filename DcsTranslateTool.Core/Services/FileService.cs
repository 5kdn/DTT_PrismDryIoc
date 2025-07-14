using System.Text;

using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Models;

using Newtonsoft.Json;

namespace DcsTranslateTool.Core.Services;

/// <summary>
/// JSON ファイルの読み書きを行うサービス
/// </summary>
public class FileService : IFileService {
    /// <inheritdoc/>
    public T ReadFromJson<T>( string folderPath, string fileName ) {
        if(string.IsNullOrWhiteSpace( folderPath ))
            throw new ArgumentException( "値が null または空です", nameof(folderPath) );
        if(string.IsNullOrWhiteSpace( fileName ))
            throw new ArgumentException( "値が null または空です", nameof(fileName) );

        var path = Path.Combine( folderPath, fileName );
        if(!File.Exists( path )) return default;

        try {
            var json = File.ReadAllText( path );
            return JsonConvert.DeserializeObject<T>( json );
        }
        catch(IOException ex) {
            throw new IOException( $"ファイルの読み込みに失敗した: {path}", ex );
        }
        catch(JsonException ex) {
            throw new JsonException( $"JSON の解析に失敗した: {path}", ex );
        }
    }

    /// <inheritdoc/>
    public void SaveToJson<T>( string folderPath, string fileName, T content ) {
        if(string.IsNullOrWhiteSpace( folderPath ))
            throw new ArgumentException( "値が null または空です", nameof(folderPath) );
        if(string.IsNullOrWhiteSpace( fileName ))
            throw new ArgumentException( "値が null または空です", nameof(fileName) );

        try {
            if(!Directory.Exists( folderPath )) {
                Directory.CreateDirectory( folderPath );
            }

            var fileContent = JsonConvert.SerializeObject( content );
            File.WriteAllText( Path.Combine( folderPath, fileName ), fileContent, Encoding.UTF8 );
        }
        catch(IOException ex) {
            throw new IOException( $"ファイルの保存に失敗した: {Path.Combine( folderPath, fileName )}", ex );
        }
        catch(JsonException ex) {
            throw new JsonException( "JSON へのシリアライズに失敗した", ex );
        }
    }

    /// <inheritdoc/>
    public void Delete( string folderPath, string fileName ) {
        if(string.IsNullOrWhiteSpace( folderPath ))
            throw new ArgumentException( "値が null または空です", nameof(folderPath) );
        if(string.IsNullOrWhiteSpace( fileName ))
            throw new ArgumentException( "値が null または空です", nameof(fileName) );

        var path = Path.Combine( folderPath, fileName );
        if(!File.Exists( path )) return;

        try {
            File.Delete( path );
        }
        catch(IOException ex) {
            throw new IOException( $"ファイルの削除に失敗した: {path}", ex );
        }
    }

    /// <inheritdoc/>
    public FileTree GetFileTree( string directoryPath ) {
        if(!Directory.Exists( directoryPath ))
            throw new DirectoryNotFoundException( $"不正なディレクトリパスが指定されました: {directoryPath}" );

        try {
            var trimmedPath = directoryPath.TrimEnd( Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar );
            return new FileTree
            {
                Name = Path.GetFileName( directoryPath ),
                AbsolutePath = directoryPath,
                IsDirectory = true,
                Children = Directory.EnumerateFileSystemEntries( directoryPath )
                    .Select( f => new FileTree
                    {
                        Name = Path.GetFileName( f ),
                        AbsolutePath = f,
                        IsDirectory = Directory.Exists( f ),
                    } )
                    .OrderBy( f => f.Name )
                    .ToList()
            };
        }
        catch(IOException ex) {
            throw new IOException( $"ファイルツリーの取得に失敗した: {directoryPath}", ex );
        }
    }

    /// <inheritdoc/>
    public async Task SaveAsync( string path, string content ) {
        if(string.IsNullOrWhiteSpace( path ))
            throw new ArgumentException( "値が null または空です", nameof(path) );

        try {
            Directory.CreateDirectory( Path.GetDirectoryName( path ) );
            await File.WriteAllTextAsync( path, content );
        }
        catch(IOException ex) {
            throw new IOException( $"ファイルの保存に失敗した: {path}", ex );
        }
    }

    /// <inheritdoc/>
    public async Task SaveAsync( string path, byte[] content ) {
        if(string.IsNullOrWhiteSpace( path ))
            throw new ArgumentException( "値が null または空です", nameof(path) );

        try {
            Directory.CreateDirectory( Path.GetDirectoryName( path ) );
            await File.WriteAllBytesAsync( path, content );
        }
        catch(IOException ex) {
            throw new IOException( $"ファイルの保存に失敗した: {path}", ex );
        }
    }
}
