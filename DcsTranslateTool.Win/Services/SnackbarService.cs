using DcsTranslateTool.Win.Contracts.Services;

using MaterialDesignThemes.Wpf;

namespace DcsTranslateTool.Win.Services;

/// <summary>
/// Snackbar を表示するサービス
/// </summary>
public class SnackbarService : ISnackbarService {
    private readonly SnackbarMessageQueue _messageQueue = new();

    /// <inheritdoc/>
    public ISnackbarMessageQueue MessageQueue => _messageQueue;

    /// <inheritdoc/>
    public void Show(
        string message,
        string? actionContent = null,
        Action? actionHandler = null,
        object? actionArgument = null,
        TimeSpan? duration = null ) =>
        MessageQueue.Enqueue(
            message,
            actionContent,
            actionHandler is null ? null : _ => actionHandler(),
            actionArgument,
            false,
            false,
            duration );

    /// <inheritdoc/>
    public void Clear() => _messageQueue.Clear();
}