using DcsTranslateTool.Core.Enums;

namespace DcsTranslateTool.Core.Models;

/// <summary>
/// GitHub へコミットするファイルの情報を管理するレコード。
/// コミット操作種別、ローカルファイルパス、リポジトリ内パスを保持する。
/// </summary>
public record CommitFile {
    /// <summary>
    /// コミット操作の種別（追加・更新・削除）を指定する。
    /// </summary>
    public CommitOperationType Operation;

    /// <summary>
    /// コミット対象のローカルファイルのパスを指定する。
    /// ファイルを削除（git rm）する場合は <see langword="null"/> とする。
    /// </summary>
    public string? LocalPath;

    /// <summary>
    /// リポジトリ内のファイルパスを指定する。
    /// </summary>
    public required string RepoPath;
}