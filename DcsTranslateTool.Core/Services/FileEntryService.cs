using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Helpers;
using DcsTranslateTool.Core.Models;

using FluentResults;

namespace DcsTranslateTool.Core.Services;

/// <summary>
/// ローカルディレクトリの子要素を取得するサービスである。
/// </summary>
public class FileEntryService() : IFileEntryService {
    /// <inheritdoc />
    public Result<IEnumerable<FileEntry>> GetChildrenRecursive( string path ) {
        if(!Path.Exists( path )) return Result.Fail( $"指定されたパスが存在しません: {path}" );

        List<FileEntry> result = [];
        try {
            foreach(var file in Directory.GetFiles( path, "*", SearchOption.AllDirectories )) {
                result.Add( new LocalFileEntry(
                    Path.GetFileName( file ),
                    Path.GetRelativePath( path, file ).Replace( "\\", "/" ),
                    Directory.Exists( file ),
                    GitBlobSha1Helper.Calculate( file )
                ) );
            }
            return result;
        }
        catch(Exception ex) {
            return Result.Fail( ex.Message );
        }
    }
}