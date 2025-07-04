namespace DcsTranslateTool.Contracts.Services;

/// <summary>
/// GitHub リポジトリ操作の契約を提供する
/// </summary>
public interface IRepositoryService
{
    /// <summary>
    /// リポジトリ情報を取得する
    /// </summary>
    /// <returns>リポジトリ情報</returns>
    Task<Octokit.Repository> GetRepositoryAsync();

    /// <summary>
    /// リポジトリツリーを取得する
    /// </summary>
    /// <returns>リポジトリツリー</returns>
    Task<IReadOnlyList<Octokit.TreeItem>> GetRepositoryTreeAsync();

    /// <summary>
    /// ファイルをバイナリ形式で取得する
    /// </summary>
    /// <param name="path">ファイルパス</param>
    /// <returns>ファイルのバイト列</returns>
    Task<byte[]> GetFileAsync(string path);

    /// <summary>
    /// ブランチを作成する
    /// </summary>
    /// <param name="branchName">作成するブランチ名</param>
    Task CreateBranchAsync(string branchName);

    /// <summary>
    /// ファイルをコミットする
    /// </summary>
    /// <param name="branchName">コミット先ブランチ名</param>
    /// <param name="paths">コミットするローカルパス一覧</param>
    /// <param name="message">コミットメッセージ</param>
    Task CommitAsync(string branchName, IEnumerable<string> paths, string message);

    /// <summary>
    /// プルリクエストを作成する
    /// </summary>
    /// <param name="branchName">PR のブランチ名</param>
    /// <param name="title">PR タイトル</param>
    /// <param name="body">PR 本文</param>
    /// <returns>作成されたプルリクエスト</returns>
    Task<Octokit.PullRequest> CreatePullRequestAsync(string branchName, string title, string body);
}
