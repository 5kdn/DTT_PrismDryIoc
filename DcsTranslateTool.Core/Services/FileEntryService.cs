using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Models;

using FluentResults;

namespace DcsTranslateTool.Core.Services;
public class FileEntryService : IFileEntryService {
    /// <inheritdoc/>
    public Result<IEnumerable<FileEntry>> GetChildren( FileEntry entry ) {
        if(!entry.IsDirectory) return Result.Fail( $"ディレクトリではないエントリが指定されました: {entry.AbsolutePath}" );
        string[] dirs;
        string[] files;
        try {
            dirs = Directory.GetDirectories( entry.AbsolutePath );
            files = Directory.GetFiles( entry.AbsolutePath );
            List<FileEntry> result = [];
            foreach(var dir in dirs) result.Add( new( Path.GetFileName( dir ), dir, true ) );
            foreach(var file in files) result.Add( new( Path.GetFileName( file ), file, false ) );
            return Result.Ok<IEnumerable<FileEntry>>( result );
        }
        catch(Exception ex) {
            return Result.Fail( ex.Message );
        }
    }
}