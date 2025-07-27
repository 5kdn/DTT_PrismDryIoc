using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Models;

namespace DcsTranslateTool.Core.Services;

public class RepositoryService( IGitHubApiClient gitHubApiClient ) : IRepositoryService {
    private const string MainBranch = "master";

    /// <inheritdoc/>
    public async Task<IReadOnlyList<RepoEntry>> GetRepositoryEntryAsync() =>
        await gitHubApiClient.GetRepositoryEntriesAsync( MainBranch );

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
}
