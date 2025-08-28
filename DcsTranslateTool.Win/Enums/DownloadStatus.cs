namespace DcsTranslateTool.Win.Enums;

/// <summary>
/// ダウンロード状態を表す列挙体である。
/// </summary>
public enum DownloadStatus {
    /// <summary>
    /// ダウンロード済みである。
    /// </summary>
    Downloaded,

    /// <summary>
    /// 未ダウンロードである。
    /// </summary>
    NotDownloaded,

    /// <summary>
    /// 新規ファイルである。
    /// </summary>
    New,

    /// <summary>
    /// 更新があるファイルである。
    /// </summary>
    Updated,
}
