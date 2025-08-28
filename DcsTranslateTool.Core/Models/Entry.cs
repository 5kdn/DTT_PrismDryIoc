using DcsTranslateTool.Core.Enums;

namespace DcsTranslateTool.Core.Models;

/// <summary>
/// ファイルまたはディレクトリのエントリを表現するクラスである。
/// </summary>
/// <param name="name">名称</param>
/// <param name="path">翻訳ルートまたはリポジトリルートからのパス</param>
/// <param name="isDirectory">ディレクトリかどうか</param>
/// <param name="localSha">ローカルのSHA1</param>
/// <param name="repoSha">リポジトリのSHA1</param>
public class Entry(
    string name,
    string path,
    bool isDirectory,
    string? localSha = null,
    string? repoSha = null
) {
    /// <summary>
    /// ファイルまたはディレクトリの名称を取得する。
    /// </summary>
    public string Name => name;

    /// <summary>
    /// 翻訳ルートまたはリポジトリルートからのパスを取得する。
    /// </summary>
    public string Path => path;

    /// <summary>
    /// ディレクトリかどうかを取得する。
    /// ディレクトリの場合は <see langword="true"/>
    /// ファイルの場合は <see langword="false"/> とする。
    /// </summary>
    public bool IsDirectory => isDirectory;

    /// <summary>
    /// ローカルのSHA1を取得する。
    /// </summary>
    public string? LocalSha => localSha;

    /// <summary>
    /// リポジトリのSHA1を取得する。
    /// </summary>
    public string? RepoSha => repoSha;

    /// <summary>
    /// ローカルとリポジトリの差分種別を取得する。
    /// </summary>
    public FileChangeType ChangeType =>
        LocalSha switch
        {
            null when RepoSha is null => FileChangeType.Unchanged,
            null => FileChangeType.Deleted,
            _ when RepoSha is null => FileChangeType.Added,
            _ when LocalSha == RepoSha => FileChangeType.Unchanged,
            _ => FileChangeType.Modified,
        };
}