using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Models;

using FluentResults;

namespace DcsTranslateTool.Core.Services;

public class RepositoryService( IGitHubApiClient gitHubApiClient ) : IRepositoryService {
    private const string MainBranch = "master";

    /// <inheritdoc/>
    public async Task<Result<IEnumerable<RepoEntry>>> GetRepositoryEntryAsync() =>
        Result.Ok( await gitHubApiClient.GetRepositoryEntriesAsync( MainBranch ) );

    /// <inheritdoc/>
    public async Task<Result<byte[]>> GetFileAsync( string path ) => Result.Ok( await gitHubApiClient.GetFileAsync( path ) );

    /// <inheritdoc/>
    public async Task<Result> CreateBranchAsync( string branchName ) {
        await gitHubApiClient.CreateBranchAsync( MainBranch, branchName );
        return Result.Ok();
    }

    /// <inheritdoc/>
    public async Task<Result> CommitAsync( string branchName, IEnumerable<CommitFile> files, string message ) {
        await gitHubApiClient.CommitAsync( branchName, files, message );
        return Result.Ok();
    }

    /// <inheritdoc/>
    public async Task<Result> CommitAsync( string branchName, CommitFile file, string message ) {
        await CommitAsync( branchName, [file], message );
        return Result.Ok();
    }

    /// <inheritdoc/>
    public async Task<Result> CreatePullRequestAsync( string branchName, string title, string message ) {
        await gitHubApiClient.CreatePullRequestAsync( branchName, MainBranch, title, message );
        return Result.Ok();
    }
}
