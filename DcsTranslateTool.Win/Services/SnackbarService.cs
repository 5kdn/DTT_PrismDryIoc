using DcsTranslateTool.Win.Contracts.Services;

using MaterialDesignThemes.Wpf;

namespace DcsTranslateTool.Win.Services;

/// <summary>
/// Snackbar を表示するサービスである。
/// </summary>
public class SnackbarService : ISnackbarService {
    /// <inheritdoc/>
    public ISnackbarMessageQueue MessageQueue { get; } = new SnackbarMessageQueue();

    /// <inheritdoc/>
    public void Show( string message ) => MessageQueue.Enqueue( message );
}