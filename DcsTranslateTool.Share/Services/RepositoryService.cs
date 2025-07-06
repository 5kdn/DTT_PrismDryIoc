using DcsTranslateTool.Share.Contracts.Services;
using DcsTranslateTool.Share.Models;

using Octokit;

namespace DcsTranslateTool.Share.Services;

public class RepositoryService : IRepositoryService {
    private const string MainBranch = "master";
    private readonly IGitHubApiClient _gitHubApiClient;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public RepositoryService( IGitHubApiClient gitHubApiClient ) => _gitHubApiClient = gitHubApiClient;

    /// <inheritdoc/>
    public async Task<List<RepoTree>> GetRepositoryTreeAsync() {
        RepoTree root = new()
        {
            Name ="",
            AbsolutePath = "",
            IsDirectory = true,
        };
        var tree = await _gitHubApiClient.GetRepositoryTreeAsync( MainBranch );
        tree.ToList().ForEach( item => AddTreeToModel( root, item ) );

        return root.Children;
    }

    /// <inheritdoc/>
    public async Task<byte[]> GetFileAsync( string path ) => await _gitHubApiClient.GetFileAsync( path );

    /// <inheritdoc/>
    public async Task CreateBranchAsync( string branchName ) =>
        await _gitHubApiClient.CreateBranchAsync( MainBranch, branchName );

    /// <inheritdoc/>
    public async Task CommitAsync( string branchName, IEnumerable<CommitFile> files, string message ) =>
        await _gitHubApiClient.CommitAsync( branchName, files, message );

    /// <inheritdoc/>
    public async Task CommitAsync( string branchName, CommitFile file, string message ) =>
        await CommitAsync( branchName, new CommitFile[] { file }, message );

    /// <inheritdoc/>
    public async Task CreatePullRequestAsync( string branchName, string title, string message ) =>
        await _gitHubApiClient.CreatePullRequestAsync( branchName, MainBranch, title, message );

    /// <summary>
    /// Octokitの<see cref="TreeItem"/>を<see cref="IRepoTreeModel"/>に変換して、ツリー構造をモデルに追加する。
    /// </summary>
    /// <param name="root">ツリーのルートオブジェクト</param>
    /// <param name="item">ツリーに追加するアイテム</param>
    private static void AddTreeToModel( RepoTree root, TreeItem item ) {
        var parts = item.Path.Split( '/' );
        var current = root;

        parts[..^1].ToList().ForEach( part => {
            var next = current.Children.FirstOrDefault( c => c.Name == part && c.IsDirectory );
            if(next == null) {
                next = new RepoTree { Name = part, AbsolutePath = item.Path, IsDirectory = true };
                current.Children.Add( next );
            }
            current = next;
        } );

        var last = parts[^1];
        if(!current.Children.Any( c => c.Name == last )) {
            current.Children.Add( new RepoTree { Name = last, AbsolutePath = item.Path, IsDirectory = item.Type == TreeType.Tree } );
        }
    }
}
