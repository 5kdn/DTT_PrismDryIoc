namespace DcsTranslateTool.Core.Models;

/// <summary>
/// ローカルのファイルやディレクトリのツリー構造を表現するクラス。
/// </summary>
public class FileTree {
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
    /// 子要素となる <see cref="FileTree"/> のリストを設定する。
    /// </summary>
    public List<FileTree> Children { get; set; } = [];
}
