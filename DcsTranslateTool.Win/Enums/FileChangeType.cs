namespace DcsTranslateTool.Win.Enums;

/// <summary>
/// ファイルの変更種別である。
/// </summary>
public enum FileChangeType {
    /// <summary>
    /// ローカル・リポジトリ両方に存在し、変更がない状態である。
    /// </summary>
    Unchanged,

    /// <summary>
    /// リポジトリにのみ存在する状態である。
    /// </summary>
    RepoOnly,

    /// <summary>
    /// ローカルにのみ存在する状態である。
    /// </summary>
    LocalOnly,

    /// <summary>
    /// ローカル・リポジトリ両方に存在し、内容に差分が有る状態である。
    /// </summary>
    Modified,
}