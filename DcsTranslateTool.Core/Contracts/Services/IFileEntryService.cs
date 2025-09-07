using DcsTranslateTool.Core.Models;

using FluentResults;

namespace DcsTranslateTool.Core.Contracts.Services;
/// <summary>
/// ローカルディレクトリのファイル列挙と監視を行うサービスのインターフェースである
/// </summary>
public interface IFileEntryService : IDisposable {
    /// <summary>
    /// 指定ディレクトリ以下の全てのファイルを再帰的に取得する
    /// <para>
    /// ファイルのみを対象とし、フォルダは対象としない
    /// </para>
    /// </summary>
    /// <param name="path">探索するルートパス</param>
    /// <returns>フラットな <see cref="FileEntry"/> のコレクション</returns>
    Task<Result<IEnumerable<FileEntry>>> GetChildrenRecursiveAsync( string path );

    /// <summary>
    /// 指定パスの監視を開始するメソッドである
    /// </summary>
    /// <param name="path">監視対象のルートパス</param>
    void Watch( string path );

    /// <summary>
    /// 監視対象のファイルエントリを取得する非同期メソッドである
    /// </summary>
    /// <returns>取得したファイルエントリのリスト</returns>
    Task<IReadOnlyList<FileEntry>> GetEntriesAsync();

    /// <summary>
    /// 監視対象のファイルエントリが変化したときに発火するイベントである
    /// </summary>
    event Func<IReadOnlyList<FileEntry>, Task> EntriesChanged;
}