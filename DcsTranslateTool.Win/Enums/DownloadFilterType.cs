namespace DcsTranslateTool.Win.Enums;

/// <summary>
/// ダウンロード状態のフィルター種別を表す列挙体である。
/// </summary>
public enum DownloadFilterType {
    /// <summary>
    /// ダウンロード済みを対象とする。
    /// </summary>
    Downloaded,

    /// <summary>
    /// 未ダウンロードを対象とする。
    /// </summary>
    NotDownloaded,

    /// <summary>
    /// 新規ファイルを対象とする。
    /// </summary>
    New,

    /// <summary>
    /// 更新があるファイルを対象とする。
    /// </summary>
    Updated,

    /// <summary>
    /// 全てを対象とする。
    /// </summary>
    All,
}
