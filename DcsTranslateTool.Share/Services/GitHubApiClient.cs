using DcsTranslateTool.Share.Models;

using Octokit;

namespace DcsTranslateTool.Share.Services;

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
    Task<IReadOnlyList<TreeItem>> GetRepositoryTreeAsync( string branch = "master" );

    /// <summary>
    /// 指定したパス・ブランチのファイルを取得する。
    /// </summary>
    /// <param name="path">取得するファイルのリポジトリ内パス。</param>
    /// <param name="branch">取得対象のブランチ名。省略時は "master"。</param>
    /// <returns>ファイルの内容を <see cref="byte"/> 配列で返す。</returns>
    Task<byte[]> GetFileAsync( string path, string branch = "master" );

    /// <summary>
    /// 指定したブランチを元に新しいブランチを作成する。
    /// </summary>
    /// <param name="sourceBranch">元となるブランチ名。</param>
    /// <param name="newBranch">作成する新しいブランチ名。</param>
    Task CreateBranchAsync( string sourceBranch, string newBranch );

    /// <summary>
    /// 指定したファイル群を指定ブランチにコミットする。
    /// </summary>
    /// <param name="branchName">コミット対象のブランチ名。</param>
    /// <param name="files">コミット対象のファイル情報一覧。</param>
    /// <param name="message">コミットメッセージ。</param>
    Task CommitAsync( string branchName, IEnumerable<CommitFile> files, string message );

    /// <summary>
    /// プルリクエストを作成する。
    /// </summary>
    /// <param name="sourceBranch">プルリクエストのソースブランチ名。</param>
    /// <param name="targetBranch">プルリクエストのターゲットブランチ名。</param>
    /// <param name="title">プルリクエストのタイトル。</param>
    /// <param name="body">プルリクエストの本文。</param>
    Task CreatePullRequestAsync( string sourceBranch, string targetBranch, string title, string body );
}

/// <summary>
/// GitHub API にアクセスするクライアント実装。
/// </summary>
public class GitHubApiClient : IGitHubApiClient {
    private readonly string _owner;
    private readonly string _repo;
    private readonly string _appName;
    private readonly int _appId;
    private readonly long _installationId;

    /// <summary>
    /// <see cref="GitHubApiClient"/> の新しいインスタンスを初期化する。
    /// </summary>
    /// <param name="owner">リポジトリ所有者名</param>
    /// <param name="repo">リポジトリ名</param>
    /// <param name="appName">GitHubアプリケーション名</param>
    /// <param name="appId">GitHubアプリケーションID</param>
    /// <param name="installationId">インストールID</param>
    public GitHubApiClient( string owner, string repo, string appName, int appId, long installationId ) {
        _owner = owner;
        _repo = repo;
        _appName = appName;
        _appId = appId;
        _installationId = installationId;
    }

    /// <inheritdoc/>
    public async Task CommitAsync( string branchName, IEnumerable<CommitFile> files, string message ) {
        IGitHubClient client = await InstallationClientGenerator();

        Reference reference = await client.Git.Reference.Get(_owner, _repo, $"heads/{branchName}");
        Commit latestCommit = await client.Git.Commit.Get(_owner, _repo, reference.Object.Sha);

        // AddOrUpdate処理
        List<CommitFile> addOrUpdateFiles = files.Where( f => f.Operation == CommitOperation.AddOrUpdate & f.LocalPath != null ).ToList();
        List<Task<NewTreeItem>> newTreeItemTasks = addOrUpdateFiles.Select( async file => {
            string content = await File.ReadAllTextAsync(file.LocalPath!);
            NewBlob newBlob = new NewBlob{Content= content, Encoding = EncodingType.Utf8};
            BlobReference blobRef = await client.Git.Blob.Create(_owner, _repo, newBlob);
            return new NewTreeItem
            {
                Path = file.RepoPath,
                Mode = "100644",
                Type = TreeType.Blob,
                Sha = blobRef.Sha
            };
        } ).ToList();
        List<NewTreeItem> newTreeItems = (await Task.WhenAll(newTreeItemTasks)).ToList();

        NewTree newTree = new NewTree { BaseTree = latestCommit.Tree.Sha };
        foreach(NewTreeItem item in newTreeItems) newTree.Tree.Add( item );

        TreeResponse createdTreeRes = await client.Git.Tree.Create(_owner, _repo, newTree);
        NewCommit newCommit = new NewCommit(message, createdTreeRes.Sha, latestCommit.Sha);
        Commit commit = await client.Git.Commit.Create(_owner, _repo, newCommit);
        await client.Git.Reference.Update( _owner, _repo, $"heads/{branchName}", new ReferenceUpdate( commit.Sha ) );
    }

    /// <inheritdoc/>
    public async Task CreateBranchAsync( string sourceBranch, string newBranch ) {
        IGitHubClient client = await InstallationClientGenerator();
        var gitRef = await client.Git.Reference.Get(_owner, _repo, $"heads/{sourceBranch}");
        var newRef =  new NewReference( $"refs/heads/{newBranch}", gitRef.Object.Sha );
        await client.Git.Reference.Create( _owner, _repo, newRef );
    }

    /// <inheritdoc/>
    public async Task CreatePullRequestAsync(
        string sourceBranch,
        string targetBranch,
        string title,
        string message ) {
        IGitHubClient client = await InstallationClientGenerator();
        var newPR = new NewPullRequest(title, sourceBranch, targetBranch){Body = message};
        var pr = await client.PullRequest.Create(_owner,_repo, newPR);
    }

    /// <inheritdoc/>
    public async Task<byte[]> GetFileAsync( string path, string branch = "master" ) {
        IGitHubClient client = await InstallationClientGenerator();
        var files = await client.Repository.Content.GetAllContentsByRef(_owner, _repo, path, branch);
        return System.Text.Encoding.UTF8.GetBytes( files[0].Content );
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<TreeItem>> GetRepositoryTreeAsync( string branch = "master" ) {
        IGitHubClient client = await InstallationClientGenerator();
        var gitRef = await client.Git.Reference.Get(_owner, _repo, $"heads/{branch}");
        var tree = await client.Git.Tree.GetRecursive(_owner, _repo, gitRef.Object.Sha);
        return tree.Tree;
    }

    private async Task<IGitHubClient> InstallationClientGenerator() {
        var generator = new GitHubJwt.GitHubJwtFactory(
            new GitHubJwt.EnvironmentVariablePrivateKeySource("PRIVATE_KEY"),
            new GitHubJwt.GitHubJwtFactoryOptions
            {
                AppIntegrationId = _appId,
                ExpirationSeconds = 600   // 10 minutes is the maximum time allowed
            }
        );
        var jwtToken = generator.CreateEncodedJwtToken();
        var credential = new Credentials( jwtToken , AuthenticationType.Bearer);
        var appClient = new GitHubClient( new ProductHeaderValue(_appName) )
        {
            Credentials = credential
        };
        var installationToken = await appClient.GitHubApps.CreateInstallationToken( _installationId );
        var token = installationToken.Token;
        var installationClient = new GitHubClient( new ProductHeaderValue( _appName ) )
        {
            Credentials = new Credentials( token, AuthenticationType.Oauth )
        };
        return installationClient;
    }
}
