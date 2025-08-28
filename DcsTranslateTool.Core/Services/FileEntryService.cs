using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Helpers;
using DcsTranslateTool.Core.Models;

using FluentResults;

namespace DcsTranslateTool.Core.Services;

/// <summary>
/// ローカルディレクトリの子要素を取得するサービスである。
/// </summary>
/// <param name="rootPath">翻訳ファイルディレクトリのルートパス</param>
public class FileEntryService( string rootPath ) : IFileEntryService {
    /// <inheritdoc />
    public Result<IEnumerable<Entry>> GetChildren( Entry entry ) {
        if(!entry.IsDirectory) return Result.Fail( $"ディレクトリではないエントリが指定されました: {entry.Path}" );
        try {
            var target = Path.GetFullPath( Path.Join( rootPath, entry.Path ) );
            var root = Path.GetFullPath( rootPath );
            if(!target.StartsWith( root, StringComparison.Ordinal )) {
                return Result.Fail( "ルート外のパスが指定された" );
            }

            string[] dirs = Directory.GetDirectories( target );
            string[] files = Directory.GetFiles( target );
            List<Entry> result = [];
            foreach(var dir in dirs) {
                string relative = Path.GetRelativePath( root, dir );
                if(relative.StartsWith( ".." )) continue;
                result.Add( new Entry( Path.GetFileName( dir ), relative, true ) );
            }

            foreach(var file in files) {
                string relative = Path.GetRelativePath( root, file );
                if(relative.StartsWith( ".." )) continue;
                string? sha = GitBlobSha1Helper.Calculate( file );
                result.Add( new Entry( Path.GetFileName( file ), relative, false, sha ) );
            }

            return Result.Ok<IEnumerable<Entry>>( result );
        }
        catch(Exception ex) {
            return Result.Fail( ex.Message );
        }
    }
}