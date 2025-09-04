using MaterialDesignThemes.Wpf;

namespace DcsTranslateTool.Win.Contracts.Services;

/// <summary>
/// Snackbar を表示するサービスのインターフェースである。
/// </summary>
public interface ISnackbarService {
    /// <summary>
    /// Snackbar のメッセージキューを取得するプロパティである。
    /// </summary>
    ISnackbarMessageQueue MessageQueue { get; }

    /// <summary>
    /// メッセージを表示する。
    /// </summary>
    /// <param name="message">表示するメッセージ</param>
    void Show( string message );
}