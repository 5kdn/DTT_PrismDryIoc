using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Models;

namespace DcsTranslateTool.Core.Services;
public class FileEntryService : IFileEntryService {
    /// <inheritdoc/>
    public IEnumerable<FileEntry> GetChildren( FileEntry entry ) {
        if(!entry.IsDirectory) yield break;
        string[] dirs;
        string[] files;
        try {
            dirs = Directory.GetDirectories( entry.AbsolutePath );
            files = Directory.GetFiles( entry.AbsolutePath );
        }
        catch {
            yield break;
        }
        foreach(var dir in dirs) yield return new FileEntry( Path.GetFileName( dir ), dir, true );
        foreach(var file in files) yield return new FileEntry( Path.GetFileName( file ), file, false );
    }
}
