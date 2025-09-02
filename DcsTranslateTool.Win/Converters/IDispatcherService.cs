namespace DcsTranslateTool.Win.Contracts.Services;

/// <summary>
/// UIスレッドで処理を実行するサービスの契約である。
/// </summary>
public interface IDispatcherService {
    /// <summary>
    /// 指定した処理をUIスレッドで非同期に実行する。
    /// </summary>
    /// <param name="func">実行する処理</param>
    /// <returns>非同期操作を表すタスク</returns>
    Task InvokeAsync( Func<Task> func );
}