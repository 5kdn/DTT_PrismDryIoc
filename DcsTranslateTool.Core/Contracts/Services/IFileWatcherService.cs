using DcsTranslateTool.Core.Models;

namespace DcsTranslateTool.Core.Contracts.Services;

/// <summary>
/// ローカルディレクトリを監視するサービスのインターフェースである。
/// </summary>
public interface IFileWatcherService : IDisposable {
    /// <summary>
    /// 指定パスの監視を開始するメソッドである。
    /// </summary>
    /// <param name="path">監視対象のルートパス</param>
    void Watch( string path );

    /// <summary>
    /// 監視対象のファイルエントリを取得する非同期メソッドである。
    /// </summary>
    /// <returns>取得したファイルエントリのリスト</returns>
    Task<IReadOnlyList<FileEntry>> GetEntriesAsync();

    /// <summary>
    /// 監視対象のファイルエントリが変化したときに発火するイベントである。
    /// </summary>
    event Func<IReadOnlyList<FileEntry>, Task> EntriesChanged;
}