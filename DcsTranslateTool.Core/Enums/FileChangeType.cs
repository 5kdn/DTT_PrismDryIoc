namespace DcsTranslateTool.Core.Enums;

/// <summary>
/// ファイルの変更種別である。
/// </summary>
public enum FileChangeType {
    /// <summary>
    /// 変更がない状態である。
    /// </summary>
    Unchanged,

    /// <summary>
    /// 削除された状態である。
    /// </summary>
    Deleted,

    /// <summary>
    /// 新規に追加された状態である。
    /// </summary>
    Added,

    /// <summary>
    /// 内容が変更された状態である。
    /// </summary>
    Modified,
}