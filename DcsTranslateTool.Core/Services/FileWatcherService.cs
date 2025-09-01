using DcsTranslateTool.Core.Contracts.Services;

namespace DcsTranslateTool.Core.Services;

/// <summary>
/// <see cref="FileSystemWatcher"/>を用いてディレクトリの変化を監視するサービスである。
/// </summary>
public class FileWatcherService : IFileWatcherService {
    #region Fields

    private FileSystemWatcher? _watcher;
    private SynchronizationContext? _context;
    private Func<Task>? _onChanged;

    #endregion

    /// <summary>
    /// 監視を停止し、リソースを破棄する。
    /// </summary>
    public void Dispose() {
        if(_watcher is null) return;

        _watcher.EnableRaisingEvents = false;
        _watcher.Dispose();
        _watcher = null;

        GC.SuppressFinalize( this );
    }

    /// <inheritdoc />
    public void Watch( string path, Func<Task> onChanged ) {
        _context = SynchronizationContext.Current;
        _onChanged = onChanged;
        _watcher = new FileSystemWatcher( path )
        {
            IncludeSubdirectories = true,
            EnableRaisingEvents = true,
        };
        _watcher.Created += OnChanged;
        _watcher.Deleted += OnChanged;
        _watcher.Changed += OnChanged;
        _watcher.Renamed += OnRenamed;
    }

    /// <summary>
    /// ファイルシステムに変更があった際に呼び出されるイベントハンドラ。
    /// <para>
    /// このメソッドは <see cref="FileSystemWatcher"/> からの変更通知を受け取り、
    /// 登録済みの非同期コールバックを <see cref="SynchronizationContext"/> にポストする。
    /// </para>
    /// </summary>
    /// <param name="sender">イベントを発生させたオブジェクト（通常は <see cref="FileSystemWatcher"/>）。</param>
    /// <param name="e">変更の詳細情報を含む <see cref="FileSystemEventArgs"/>。</param>
    /// <remarks>
    /// - <c>_context</c> または <c>_onChanged</c> が <c>null</c> の場合は処理を行わない。  
    /// - コールバックは非同期で実行され、UI スレッドなど特定の同期コンテキスト上で処理される。  
    /// </remarks>
    /// <exception cref="ObjectDisposedException">
    /// <c>_context</c> が既に破棄されている場合に発生する可能性がある。
    /// </exception>
    private void OnChanged( object sender, FileSystemEventArgs e ) {
        if(_context is null || _onChanged is null) return;
        _context.Post( async _ => await _onChanged(), null );
    }

    private void OnRenamed( object sender, RenamedEventArgs e ) => OnChanged( sender, e );

}