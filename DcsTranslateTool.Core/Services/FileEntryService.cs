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
        var result = GetChildrenRecursive( _path );
        if(result.IsFailed) return Task.FromResult<IReadOnlyList<FileEntry>>( [] );
        return Task.FromResult<IReadOnlyList<FileEntry>>( [.. result.Value] );
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