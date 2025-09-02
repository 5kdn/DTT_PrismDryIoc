using System.Windows;

using DcsTranslateTool.Win.Contracts.Services;

namespace DcsTranslateTool.Win.Services;

/// <summary>
/// WPFのDispatcherを用いてUIスレッドで処理を実行するサービスである。
/// </summary>
public class DispatcherService : IDispatcherService {
    /// <inheritdoc />
    public Task InvokeAsync( Func<Task> func ) => Application.Current.Dispatcher.InvokeAsync( func ).Task.Unwrap();
}