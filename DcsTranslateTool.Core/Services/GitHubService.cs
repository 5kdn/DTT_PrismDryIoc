using Octokit;
using DcsTranslateTool.Core.Contracts.Services;

namespace DcsTranslateTool.Core.Services;

/// <summary>
/// GitHub を操作するサービス
/// </summary>
public class GitHubService : IGitHubService
{
    private const string Owner = "5kdn";
    private const string Repo = "test_DCS";
    private const string MainBranch = "master";
    private const string DevelopBranch = "develop";
    private readonly GitHubClient _client;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public GitHubService()
    {
        _client = new GitHubClient(new ProductHeaderValue("DcsTranslateTool"));
    }

    /// <inheritdoc/>
    public async Task<Repository> GetRepositoryAsync()
        => await _client.Repository.Get(Owner, Repo);

    /// <inheritdoc/>
    public async Task<IReadOnlyList<TreeItem>> GetRepositoryTreeAsync()
    {
        var mainRef = await _client.Git.Reference.Get(Owner, Repo, $"heads/{MainBranch}");
        var tree = await _client.Git.Tree.GetRecursive(Owner, Repo, mainRef.Object.Sha);
        return tree.Tree;
    }

    /// <inheritdoc/>
    public async Task<byte[]> GetFileAsync(string path)
    {
        var files = await _client.Repository.Content.GetAllContentsByRef(Owner, Repo, path, MainBranch);
        return System.Text.Encoding.UTF8.GetBytes(files[0].Content);
    }

    /// <inheritdoc/>
    public async Task CreateBranchAsync(string branchName)
    {
        var developRef = await _client.Git.Reference.Get(Owner, Repo, $"heads/{DevelopBranch}");
        var newRef = new NewReference($"refs/heads/{branchName}", developRef.Object.Sha);
        await _client.Git.Reference.Create(Owner, Repo, newRef);
    }

    /// <inheritdoc/>
    public async Task CommitAsync(string branchName, IEnumerable<string> paths, string message)
    {
        var branchRef = await _client.Git.Reference.Get(Owner, Repo, $"heads/{branchName}");
        var latestCommit = await _client.Git.Commit.Get(Owner, Repo, branchRef.Object.Sha);

        var newTree = new NewTree();
        foreach (var path in paths)
        {
            var blob = new NewBlob
            {
                Encoding = EncodingType.Base64,
                Content = Convert.ToBase64String(await File.ReadAllBytesAsync(path))
            };
            var blobResult = await _client.Git.Blob.Create(Owner, Repo, blob);
            var treeItem = new NewTreeItem
            {
                Path = System.IO.Path.GetFileName(path),
                Mode = "100644",
                Type = TreeType.Blob,
                Sha = blobResult.Sha
            };
            newTree.Tree.Add(treeItem);
        }
        newTree.BaseTree = latestCommit.Tree.Sha;
        var createdTree = await _client.Git.Tree.Create(Owner, Repo, newTree);
        var newCommit = new NewCommit(message, createdTree.Sha, branchRef.Object.Sha);
        var commit = await _client.Git.Commit.Create(Owner, Repo, newCommit);
        await _client.Git.Reference.Update(Owner, Repo, $"heads/{branchName}", new ReferenceUpdate(commit.Sha));
    }

    /// <inheritdoc/>
    public async Task<PullRequest> CreatePullRequestAsync(string branchName, string title, string body)
    {
        var pr = new NewPullRequest(title, branchName, DevelopBranch) { Body = body };
        return await _client.PullRequest.Create(Owner, Repo, pr);
    }
}
