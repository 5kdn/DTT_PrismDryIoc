using DcsTranslateTool.Core.Models;

namespace DcsTranslateTool.Core.Contracts.Services;
/// <summary>
/// GitHub API クライアントのインターフェース。
/// リポジトリツリーの取得、ファイル取得、ブランチ作成、コミット、プルリクエスト作成を行う。
/// </summary>
public interface IGitHubApiClient {
    /// <summary>
    /// 指定したブランチのリポジトリツリーを取得する。
    /// </summary>
    /// <param name="branch">取得対象のブランチ名。省略時は "master"。</param>
    /// <returns><see cref="TreeItem"/> のリストを返す。</returns>
    /// <exception cref="Exception">GitHub API の呼び出しに失敗した場合に送出する。</exception>
    Task<IEnumerable<FileEntry>> GetFileEntriesAsync( string branch = "master" );

    /// <summary>
    /// 指定したパス・ブランチのファイルを取得する。
    /// </summary>
    /// <param name="path">取得するファイルのリポジトリ内パス。</param>
    /// <param name="branch">取得対象のブランチ名。省略時は "master"。</param>
    /// <returns>ファイルの内容を <see cref="byte"/> 配列で返す。</returns>
    /// <exception cref="Exception">GitHub API の呼び出しに失敗した場合に送出する。</exception>
    Task<byte[]> GetFileAsync( string path, string branch = "master" );

    /// <summary>
    /// 指定したブランチを元に新しいブランチを作成する。
    /// </summary>
    /// <param name="sourceBranch">元となるブランチ名。</param>
    /// <param name="newBranch">作成する新しいブランチ名。</param>
    /// <exception cref="Exception">GitHub API の呼び出しに失敗した場合に送出する。</exception>
    Task CreateBranchAsync( string sourceBranch, string newBranch );

    /// <summary>
    /// 指定したファイル群を指定ブランチにコミットする。
    /// </summary>
    /// <param name="branchName">コミット対象のブランチ名。</param>
    /// <param name="files">コミット対象のファイル情報一覧。</param>
    /// <param name="message">コミットメッセージ。</param>
    /// <exception cref="FileNotFoundException">コミット対象ファイルが存在しない場合に送出する。</exception>
    /// <exception cref="Exception">GitHub API の呼び出しに失敗した場合に送出する。</exception>
    Task CommitAsync( string branchName, IEnumerable<CommitFile> files, string message );

    /// <summary>
    /// プルリクエストを作成する。
    /// </summary>
    /// <param name="sourceBranch">プルリクエストのソースブランチ名。</param>
    /// <param name="targetBranch">プルリクエストのターゲットブランチ名。</param>
    /// <param name="title">プルリクエストのタイトル。</param>
    /// <param name="body">プルリクエストの本文。</param>
    /// <exception cref="Exception">GitHub API の呼び出しに失敗した場合に送出する。</exception>
    Task CreatePullRequestAsync( string sourceBranch, string targetBranch, string title, string body );
}