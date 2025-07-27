using System.IO;

using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Enums;
using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.Converters;

using Octokit;

namespace DcsTranslateTool.Win.Services;

/// <summary>
/// GitHub API にアクセスするクライアント実装。
/// </summary>
/// <remarks>
/// <see cref="GitHubApiClient"/> の新しいインスタンスを初期化する。
/// </remarks>
/// <param name="owner">リポジトリ所有者名</param>
/// <param name="repo">リポジトリ名</param>
/// <param name="appName">GitHubアプリケーション名</param>
/// <param name="appId">GitHubアプリケーションID</param>
/// <param name="installationId">インストールID</param>
public class GitHubApiClient( string owner, string repo, string appName, int appId, long installationId ) : IGitHubApiClient {
    /// <inheritdoc/>
    public async Task<IReadOnlyList<RepoEntry>> GetRepositoryEntriesAsync( string branch = "master" ) {
        try {
            IGitHubClient client = await InstallationClientGenerator();
            var gitRef = await client.Git.Reference.Get(owner, repo, $"heads/{branch}");
            var tree = await client.Git.Tree.GetRecursive(owner, repo, gitRef.Object.Sha);
            if(tree is null) return [];
            return (IReadOnlyList<RepoEntry>)tree.Tree.Select( item => TreeItemToRepoEntryConverter.Convert( item ) );
        }
        catch(ApiException ex) {
            throw new Exception( "GitHub API の呼び出しに失敗しました", ex );
        }
    }

    /// <inheritdoc/>
    public async Task CommitAsync( string branchName, IEnumerable<CommitFile> files, string message ) {
        try {
            IGitHubClient client = await InstallationClientGenerator();

            Reference reference = await client.Git.Reference.Get(owner, repo, $"heads/{branchName}");
            Commit latestCommit = await client.Git.Commit.Get(owner, repo, reference.Object.Sha);

            // AddOrUpdate処理
            List<CommitFile> addOrUpdateFiles = [.. files.Where( f => f.Operation == CommitOperationType.AddOrUpdate && f.LocalPath != null )];
            List<Task<NewTreeItem>> newTreeItemTasks = [
                .. addOrUpdateFiles.Select( async file => {
                    if(!File.Exists( file.LocalPath! )) throw new FileNotFoundException( "ファイルが存在しません", file.LocalPath );
                    string content = await File.ReadAllTextAsync(file.LocalPath!);
                    NewBlob newBlob = new() { Content = content, Encoding = EncodingType.Utf8 };
                    BlobReference blobRef = await client.Git.Blob.Create(owner, repo, newBlob);
                    return new NewTreeItem
                    {
                        Path = file.RepoPath,
                        Mode = "100644",
                        Type = TreeType.Blob,
                        Sha = blobRef.Sha
                    };
                } )
            ];
            List<NewTreeItem> newTreeItems = [.. await Task.WhenAll(newTreeItemTasks)];

            NewTree newTree = new() { BaseTree = latestCommit.Tree.Sha };
            foreach(NewTreeItem item in newTreeItems) newTree.Tree.Add( item );

            TreeResponse createdTreeRes = await client.Git.Tree.Create(owner, repo, newTree);
            NewCommit newCommit = new(message, createdTreeRes.Sha, latestCommit.Sha);
            Commit commit = await client.Git.Commit.Create(owner, repo, newCommit);
            await client.Git.Reference.Update( owner, repo, $"heads/{branchName}", new ReferenceUpdate( commit.Sha ) );
        }
        catch(ApiException ex) {
            throw new Exception( "GitHub API の呼び出しに失敗しました", ex );
        }
    }

    /// <inheritdoc/>
    public async Task CreateBranchAsync( string sourceBranch, string newBranch ) {
        try {
            IGitHubClient client = await InstallationClientGenerator();
            var gitRef = await client.Git.Reference.Get(owner, repo, $"heads/{sourceBranch}");
            var newRef =  new NewReference( $"refs/heads/{newBranch}", gitRef.Object.Sha );
            await client.Git.Reference.Create( owner, repo, newRef );
        }
        catch(ApiException ex) {
            throw new Exception( "GitHub API の呼び出しに失敗しました", ex );
        }
    }

    /// <inheritdoc/>
    public async Task CreatePullRequestAsync(
        string sourceBranch,
        string targetBranch,
        string title,
        string message ) {
        try {
            IGitHubClient client = await InstallationClientGenerator();
            var newPR = new NewPullRequest(title, sourceBranch, targetBranch){Body = message};
            await client.PullRequest.Create( owner, repo, newPR );
        }
        catch(ApiException ex) {
            throw new Exception( "GitHub API の呼び出しに失敗しました", ex );
        }
    }

    /// <inheritdoc/>
    public async Task<byte[]> GetFileAsync( string path, string branch = "master" ) {
        try {
            IGitHubClient client = await InstallationClientGenerator();
            var files = await client.Repository.Content.GetAllContentsByRef(owner, repo, path, branch);
            return System.Text.Encoding.UTF8.GetBytes( files[0].Content );
        }
        catch(ApiException ex) {
            throw new Exception( "GitHub API の呼び出しに失敗しました", ex );
        }
    }

    private async Task<IGitHubClient> InstallationClientGenerator() {
        var generator = new GitHubJwt.GitHubJwtFactory(
            new GitHubJwt.EnvironmentVariablePrivateKeySource("PRIVATE_KEY"),
            new GitHubJwt.GitHubJwtFactoryOptions
            {
                AppIntegrationId = appId,
                ExpirationSeconds = 600   // 10 minutes is the maximum time allowed
            }
        );
        var jwtToken = generator.CreateEncodedJwtToken();
        var credential = new Credentials( jwtToken , AuthenticationType.Bearer);
        var appClient = new GitHubClient( new ProductHeaderValue(appName) )
        {
            Credentials = credential
        };
        var installationToken = await appClient.GitHubApps.CreateInstallationToken( installationId );
        var token = installationToken.Token;
        var installationClient = new GitHubClient( new ProductHeaderValue( appName ) )
        {
            Credentials = new Credentials( token, AuthenticationType.Oauth )
        };
        return installationClient;
    }
}
