namespace DcsTranslateTool.Core.Contracts.Services;
/// <summary>
/// 指定ディレクトリの変化を監視するサービスである。
/// </summary>
public interface IFileWatcherService : IDisposable {
    /// <summary>
    /// 監視対象ディレクトリを設定し、変化があったときにコールバックを実行する。
    /// </summary>
    /// <param name="path">監視するディレクトリのパス</param>
    /// <param name="onChanged">変化時に実行する非同期コールバック</param>
    void Watch( string path, Func<Task> onChanged );
}