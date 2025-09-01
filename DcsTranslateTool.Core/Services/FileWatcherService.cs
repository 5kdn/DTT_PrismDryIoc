using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Models;

namespace DcsTranslateTool.Core.Services;

/// <summary>
/// ファイルシステムを監視して変更を通知するサービスである。
/// </summary>
public class FileWatcherService( IFileEntryService _fileEntryService ) : IFileWatcherService {
    #region Fields

    private FileSystemWatcher? _watcher;
    private string _path = string.Empty;

    #endregion

    public void Dispose() {
        _watcher?.Dispose();

        GC.SuppressFinalize( this );
    }

    /// <inheritdoc />
    public event Func<IReadOnlyList<FileEntry>, Task>? EntriesChanged;

    /// <inheritdoc />
    public void Watch( string path ) {
        _path = path;
        _watcher?.Dispose();
        if(!Directory.Exists( path )) return;

        _watcher = new FileSystemWatcher( path )
        {
            IncludeSubdirectories = true,
            EnableRaisingEvents = true,
        };
        _watcher.Changed += OnFileSystemChanged;
        _watcher.Created += OnFileSystemChanged;
        _watcher.Deleted += OnFileSystemChanged;
        _watcher.Renamed += OnFileSystemChanged;

        _ = NotifyAsync();
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<FileEntry>> GetEntriesAsync() {
        if(string.IsNullOrEmpty( _path )) return Task.FromResult<IReadOnlyList<FileEntry>>( [] );
        var result = _fileEntryService.GetChildrenRecursive( _path );
        if(result.IsFailed) return Task.FromResult<IReadOnlyList<FileEntry>>( [] );
        return Task.FromResult<IReadOnlyList<FileEntry>>( [.. result.Value] );
    }

    /// <summary>
    /// ファイルシステムの変更イベントを処理する。
    /// </summary>
    /// <param name="sender">イベント発生元</param>
    /// <param name="e">イベント引数</param>
    private async void OnFileSystemChanged( object sender, FileSystemEventArgs e ) {
        await NotifyAsync();
    }

    /// <summary>
    /// 変更通知イベントを発火する。
    /// </summary>
    private async Task NotifyAsync() {
        if(EntriesChanged is null) return;
        var entries = await GetEntriesAsync();
        await EntriesChanged.Invoke( entries );
    }
}