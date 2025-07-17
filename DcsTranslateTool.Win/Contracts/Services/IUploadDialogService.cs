namespace DcsTranslateTool.Win.Contracts.Services;

/// <summary>
/// アップロードダイアログを表示するサービスである。
/// </summary>
public interface IUploadDialogService {
    /// <summary>
    /// ダイアログを表示する。
    /// </summary>
    /// <param name="paths">アップロード対象ファイルパス</param>
    void ShowDialog( IEnumerable<string> paths );
}
