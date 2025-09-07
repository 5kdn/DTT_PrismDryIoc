using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Models;

using FluentResults;

namespace DcsTranslateTool.Core.Services;

/// <summary>
/// GitHub API クライアント <see cref="IGitHubApiClient"/> を用いてリポジトリを操作するサービス
/// </summary>
/// <param name="gitHubApiClient"><see cref="IGitHubApiClient"/> の実装</param>
/// <param name="defaultBranch">既定ブランチ名</param>
public class RepositoryService( IGitHubApiClient gitHubApiClient, string defaultBranch = "master" ) : IRepositoryService {
    private readonly string _defaultBranch = defaultBranch;

    /// <inheritdoc/>
    public async Task<Result<IEnumerable<FileEntry>>> GetFileEntriesAsync() {
        try {
            var entries = await gitHubApiClient.GetFileEntriesAsync( _defaultBranch );
            return Result.Ok( entries );
        }
        catch(Exception ex) {
            return Result.Fail( new Error( "リポジトリエントリーの取得に失敗しました" ).CausedBy( ex ) );
        }
    }

    /// <inheritdoc/>
    public async Task<Result<byte[]>> GetFileAsync( string path ) {
        try {
            var data =  await gitHubApiClient.GetFileAsync( path );
            return Result.Ok( data );
        }
        catch(Exception ex) {
            return Result.Fail( new Error( "ファイルの取得に失敗しました" ).CausedBy( ex ) );
        }
    }

    /// <inheritdoc/>
    public async Task<Result> CreateBranchAsync( string branchName ) {
        try {
            await gitHubApiClient.CreateBranchAsync( _defaultBranch, branchName );
            return Result.Ok();
        }
        catch(Exception ex) {
            return Result.Fail( new Error( "ブランチの作成に失敗しました" ).CausedBy( ex ) );
        }
    }

    /// <inheritdoc/>
    public async Task<Result> CommitAsync( string branchName, IEnumerable<CommitFile> files, string message ) {
        try {
            await gitHubApiClient.CommitAsync( branchName, files, message );
            return Result.Ok();
        }
        catch(Exception ex) {
            return Result.Fail( new Error( "コミットに失敗しました" ).CausedBy( ex ) );
        }
    }

    /// <inheritdoc/>
    public async Task<Result> CommitAsync( string branchName, CommitFile file, string message ) =>
        await CommitAsync( branchName, [file], message );

    /// <inheritdoc/>
    public async Task<Result> CreatePullRequestAsync( string branchName, string title, string message ) {
        try {
            await gitHubApiClient.CreatePullRequestAsync( branchName, _defaultBranch, title, message );
            return Result.Ok();
        }
        catch(Exception ex) {
            return Result.Fail( new Error( "プルリクエストの作成に失敗しました" ).CausedBy( ex ) );
        }
    }
}