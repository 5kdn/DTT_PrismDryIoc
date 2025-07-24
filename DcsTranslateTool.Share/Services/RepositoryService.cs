using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Share.Contracts.Services;
using DcsTranslateTool.Share.Models;

using Octokit;

namespace DcsTranslateTool.Share.Services;

public class RepositoryService( IGitHubApiClient gitHubApiClient ) : IRepositoryService {
    private const string MainBranch = "master";

    /// <inheritdoc/>
    public async Task<List<RepoEntry>> GetRepositoryEntryAsync() {
        var tree = await gitHubApiClient.GetRepositoryTreeAsync( MainBranch );
        return [.. tree.Select( treeItem => ConvertTreeItemToRepoEntry( treeItem ) )];
    }

    /// <inheritdoc/>
    public async Task<byte[]> GetFileAsync( string path ) => await gitHubApiClient.GetFileAsync( path );

    /// <inheritdoc/>
    public async Task CreateBranchAsync( string branchName ) =>
        await gitHubApiClient.CreateBranchAsync( MainBranch, branchName );

    /// <inheritdoc/>
    public async Task CommitAsync( string branchName, IEnumerable<CommitFile> files, string message ) =>
        await gitHubApiClient.CommitAsync( branchName, files, message );

    /// <inheritdoc/>
    public async Task CommitAsync( string branchName, CommitFile file, string message ) =>
        await CommitAsync( branchName, [file], message );

    /// <inheritdoc/>
    public async Task CreatePullRequestAsync( string branchName, string title, string message ) =>
        await gitHubApiClient.CreatePullRequestAsync( branchName, MainBranch, title, message );

    /// <summary>
    /// Octokitの<see cref="TreeItem"/>を<see cref="RepoEntry"/>に変換する
    /// </summary>
    /// <param name="item">ソースの<see cref="TreeItem"/></param>
    /// <returns>変換した<see cref="RepoEntry"/></returns>
    private static RepoEntry ConvertTreeItemToRepoEntry( TreeItem item ) {
        var path = item.Path;
        var pathParts = path.Split( '/' );
        var name = pathParts[^1];
        var isDirectory = item.Type == TreeType.Tree;

        return new RepoEntry( name, path, isDirectory );
    }
}
