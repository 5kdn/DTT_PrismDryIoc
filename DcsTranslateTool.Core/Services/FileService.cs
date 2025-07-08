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
    public T Read<T>( string folderPath, string fileName ) {
        var path = Path.Combine(folderPath, fileName);
        if(File.Exists( path )) {
            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>( json );
        }

        return default;
    }

    /// <inheritdoc/>
    public void Save<T>( string folderPath, string fileName, T content ) {
        if(!Directory.Exists( folderPath )) {
            Directory.CreateDirectory( folderPath );
        }

        var fileContent = JsonConvert.SerializeObject(content);
        File.WriteAllText( Path.Combine( folderPath, fileName ), fileContent, Encoding.UTF8 );
    }

    /// <inheritdoc/>
    public void Delete( string folderPath, string fileName ) {
        if(fileName != null && File.Exists( Path.Combine( folderPath, fileName ) )) {
            File.Delete( Path.Combine( folderPath, fileName ) );
        }
    }

    /// <inheritdoc/>
    public FileTree GetFileTree( string directoryPath ) {
        if(!Directory.Exists( directoryPath )) throw new DirectoryNotFoundException( $"不正なディレクトリパスが指定されました: {directoryPath}" );

        var trimmedPath = directoryPath.TrimEnd( Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar );
        return new FileTree()
        {
            Name = Path.GetFileName( directoryPath ),
            AbsolutePath = directoryPath,
            IsDirectory = true,
            Children = Directory.EnumerateFileSystemEntries( directoryPath ).Select( f => new FileTree()
            {
                Name = Path.GetFileName( f ),
                AbsolutePath = f,
                IsDirectory = Directory.Exists( f ),
            } ).ToList()
        };
    }
}
