using System.Windows;

using DcsTranslateTool.Win.Contracts.Services;

namespace DcsTranslateTool.Win.Services;

/// <summary>
/// WPF の Dispatcher を用いてUIスレッドで処理を実行するサービス
/// </summary>
public class DispatcherService : IDispatcherService {
    /// <inheritdoc />
    public Task InvokeAsync( Func<Task> func ) => Application.Current.Dispatcher.InvokeAsync( func ).Task.Unwrap();
}