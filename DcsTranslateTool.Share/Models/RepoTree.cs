namespace DcsTranslateTool.Share.Models;

/// <summary>
/// リポジトリ内のファイルやディレクトリのツリー構造を表現するクラス。
/// </summary>
public class RepoTree {
    /// <summary>
    /// ファイルまたはディレクトリの名称を設定する。
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// ファイルまたはディレクトリの絶対パスを設定する。
    /// </summary>
    public required string AbsolutePath { get; set; }

    /// <summary>
    /// ディレクトリかどうかを設定する。
    /// ディレクトリの場合は <see langword="true"/>、ファイルの場合は <see langword="false"/> とする。
    /// </summary>
    public required bool IsDirectory { get; set; }

    /// <summary>
    /// 子要素となる <see cref="RepoTree"/> のリストを設定する。
    /// </summary>
    public List<RepoTree> Children { get; set; } = new();
}
