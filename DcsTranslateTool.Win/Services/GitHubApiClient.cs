using System.Diagnostics;
using System.IO;

using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Enums;
using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.Constants;
using DcsTranslateTool.Win.Converters;

using Octokit;

namespace DcsTranslateTool.Win.Services;

/// <summary>
/// GitHub API にアクセスするクライアント実装
/// </summary>
/// <param name="decryptService"><see cref="DecryptService"/> を注入する</param>
public class GitHubApiClient( DecryptService decryptService ) : IGitHubApiClient {
    /// <inheritdoc/>
    public async Task<IEnumerable<FileEntry>> GetFileEntriesAsync( string branch = "master" ) {
        Debug.WriteLine( "GitHubApiClient.GetRepositoryEntriesAsync called" );
        try {
            IGitHubClient client = await InstallationClientGenerator();
            Debug.WriteLine( $"TargetRepository: {TargetRepository.Owner}/{TargetRepository.Repo} heads/{branch}" );
            var gitRef = await client.Git.Reference.Get(TargetRepository.Owner, TargetRepository.Repo, $"heads/{branch}");
            var tree = await client.Git.Tree.GetRecursive(TargetRepository.Owner, TargetRepository.Repo, gitRef.Object.Sha);
            if(tree is null) return [];
            return tree.Tree.Select( item => TreeItemToFileEntryConverter.Convert( item ) );
        }
        catch(ApiException ex) {
            Debug.WriteLine( $"GitHub API の呼び出しに失敗しました1: {ex.Message}" );
            throw new Exception( "GitHub API の呼び出しに失敗しました1", ex );
        }
        finally {
            Debug.WriteLine( "GitHubApiClient.GetRepositoryEntriesAsync completed" );
        }
    }

    /// <inheritdoc/>
    public async Task CommitAsync( string branchName, IEnumerable<CommitFile> files, string message ) {
        try {
            IGitHubClient client = await InstallationClientGenerator();

            Reference reference = await client.Git.Reference.Get(TargetRepository.Owner, TargetRepository.Repo, $"heads/{branchName}");
            Commit latestCommit = await client.Git.Commit.Get(TargetRepository.Owner, TargetRepository.Repo, reference.Object.Sha);

            // AddOrUpdate処理
            List<CommitFile> addOrUpdateFiles = [.. files.Where( f => f.Operation == CommitOperationType.AddOrUpdate && f.LocalPath != null )];
            List<Task<NewTreeItem>> newTreeItemTasks = [
                .. addOrUpdateFiles.Select( async file => {
                    if(!File.Exists( file.LocalPath! )) throw new FileNotFoundException( "ファイルが存在しません", file.LocalPath );
                    string content = await File.ReadAllTextAsync(file.LocalPath!);
                    NewBlob newBlob = new() { Content = content, Encoding = EncodingType.Utf8 };
                    BlobReference blobRef = await client.Git.Blob.Create(TargetRepository.Owner, TargetRepository.Repo, newBlob);
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

            TreeResponse createdTreeRes = await client.Git.Tree.Create(TargetRepository.Owner, TargetRepository.Repo, newTree);
            NewCommit newCommit = new(message, createdTreeRes.Sha, latestCommit.Sha);
            Commit commit = await client.Git.Commit.Create(TargetRepository.Owner, TargetRepository.Repo, newCommit);
            await client.Git.Reference.Update( TargetRepository.Owner, TargetRepository.Repo, $"heads/{branchName}", new ReferenceUpdate( commit.Sha ) );
        }
        catch(ApiException ex) {
            throw new Exception( "GitHub API の呼び出しに失敗しました", ex );
        }
    }

    /// <inheritdoc/>
    public async Task CreateBranchAsync( string sourceBranch, string newBranch ) {
        try {
            IGitHubClient client = await InstallationClientGenerator();
            var gitRef = await client.Git.Reference.Get(TargetRepository.Owner, TargetRepository.Repo, $"heads/{sourceBranch}");
            var newRef =  new NewReference( $"refs/heads/{newBranch}", gitRef.Object.Sha );
            await client.Git.Reference.Create( TargetRepository.Owner, TargetRepository.Repo, newRef );
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
            await client.PullRequest.Create( TargetRepository.Owner, TargetRepository.Repo, newPR );
        }
        catch(ApiException ex) {
            throw new Exception( "GitHub API の呼び出しに失敗しました", ex );
        }
    }

    /// <inheritdoc/>
    public async Task<byte[]> GetFileAsync( string path, string branch = "master" ) {
        try {
            IGitHubClient client = await InstallationClientGenerator();
            var files = await client.Repository.Content.GetAllContentsByRef(TargetRepository.Owner, TargetRepository.Repo, path, branch);
            return System.Text.Encoding.UTF8.GetBytes( files[0].Content );
        }
        catch(ApiException ex) {
            throw new Exception( "GitHub API の呼び出しに失敗しました", ex );
        }
    }

    private async Task<IGitHubClient> InstallationClientGenerator() {
        Debug.WriteLine( "GitHubApiClient.InstallationClientGenerator called" );
        try {
            var privateKeySource = new GitHubJwt.StringPrivateKeySource(decryptService.GetMessage());
            var option = new GitHubJwt.GitHubJwtFactoryOptions
            {
                AppIntegrationId = GitHubAppSettings.AppId,
                ExpirationSeconds = 600   // 10 minutes is the maximum time allowed
            };
            var generator = new GitHubJwt.GitHubJwtFactory(privateKeySource, option);
            var jwtToken = generator.CreateEncodedJwtToken();
            var credential = new Credentials( jwtToken , AuthenticationType.Bearer);
            var appClient = new GitHubClient( new ProductHeaderValue(GitHubAppSettings.AppName) )
            {
                Credentials = credential
            };
            var installationToken = await appClient.GitHubApps.CreateInstallationToken( GitHubAppSettings.InstallationId );
            var token = installationToken.Token;
            var installationClient = new GitHubClient( new ProductHeaderValue( GitHubAppSettings.AppName ) )
            {
                Credentials = new Credentials( token, AuthenticationType.Oauth )
            };
            return installationClient;
        }
        catch(ApiException ex) {
            Debug.WriteLine( $"GitHub API の呼び出しに失敗しました: {ex.Message}" );
            throw new Exception( "GitHub API の呼び出しに失敗しました", ex );
        }
        catch(Exception ex) {
            Debug.WriteLine( $"GitHub API の呼び出しに失敗しました: {ex.Message}" );
            throw new Exception( "GitHub API の呼び出しに失敗しました", ex );
        }
        finally {
            Debug.WriteLine( "GitHubApiClient.InstallationClientGenerator completed" );
        }
    }
}