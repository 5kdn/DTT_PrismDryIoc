using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Helpers;
using DcsTranslateTool.Core.Models;

using FluentResults;

namespace DcsTranslateTool.Core.Services;

/// <summary>
/// ローカルディレクトリのファイル列挙と監視を行うサービスである。
/// </summary>
public class FileEntryService() : IFileEntryService {
    #region Fields

    private FileSystemWatcher? _watcher;
    private string _path = string.Empty;

    #endregion

    /// <inheritdoc />
    public void Dispose() {
        _watcher?.Dispose();
        GC.SuppressFinalize( this );
    }

    /// <inheritdoc />
    public event Func<IReadOnlyList<FileEntry>, Task>? EntriesChanged;

    /// <inheritdoc />
    public async Task<Result<IEnumerable<FileEntry>>> GetChildrenRecursiveAsync( string path ) {
        if(!Path.Exists( path )) return Result.Fail( $"指定されたパスが存在しません: {path}" );

        List<FileEntry> result = [];
        try {
            foreach(var entryPath in Directory.GetFiles( path, "*", SearchOption.AllDirectories )) {
                bool isDir = Directory.Exists( entryPath );
                string relative = Path.GetRelativePath( path, entryPath ).Replace( "\\", "/" );
                string name = Path.GetFileName( entryPath );
                string? sha = isDir ? null : await GitBlobSha1Helper.CalculateAsync( entryPath );
                result.Add( new LocalFileEntry( name, relative, isDir, sha ) );
            }
            return result;
        }
        catch(Exception ex) {
            return Result.Fail( ex.Message );
        }
    }

    /// <inheritdoc />
    public void Watch( string path ) {
        _path = path;
        _watcher?.Dispose();
        if(!Directory.Exists( path )) return;

        _watcher = new FileSystemWatcher( path ) { IncludeSubdirectories = true };
        _watcher.Changed += OnFileSystemChanged;
        _watcher.Created += OnFileSystemChanged;
        _watcher.Deleted += OnFileSystemChanged;
        _watcher.Renamed += OnFileSystemChanged;
        _watcher.EnableRaisingEvents = true;

        _ = NotifyAsync();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<FileEntry>> GetEntriesAsync() {
        if(string.IsNullOrEmpty( _path )) return await Task.FromResult<IReadOnlyList<FileEntry>>( [] );
        var result = await GetChildrenRecursiveAsync( _path );
        if(result.IsFailed) return await Task.FromResult<IReadOnlyList<FileEntry>>( [] );
        return await Task.FromResult<IReadOnlyList<FileEntry>>( [.. result.Value] );
    }

    /// <summary>
    /// ファイルシステムの変更イベントを処理する。
    /// </summary>
    /// <param name="sender">イベント発生元</param>
    /// <param name="e">イベント引数</param>
    private async void OnFileSystemChanged( object sender, FileSystemEventArgs e ) => await NotifyAsync();

    /// <summary>
    /// 変更通知イベントを発火する。
    /// </summary>
    private async Task NotifyAsync() {
        if(EntriesChanged is null) return;
        var entries = await GetEntriesAsync();
        await EntriesChanged.Invoke( entries );
    }
}