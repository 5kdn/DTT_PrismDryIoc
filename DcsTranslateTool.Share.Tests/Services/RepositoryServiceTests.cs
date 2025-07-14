using DcsTranslateTool.Share.Models;
using DcsTranslateTool.Share.Services;

using Moq;

using Octokit;

using Xunit;

namespace DcsTranslateTool.Share.Tests.Services;

public class RepositoryServiceTests {
    [Fact( DisplayName = "RepositoryServiceは正常に呼び出される" )]
    public void CreateRepository_ReturnsRepository() {
        // Arrange & Act
        var mockGitHubApiClient = new Mock<IGitHubApiClient>();
        var service = new RepositoryService( mockGitHubApiClient.Object );

        // Act
        Assert.NotNull( service );
    }

    [Fact( DisplayName = "ツリーが取得できる" )]
    public async Task GetRepositoryTreeAsync_ShouldReturnTree() {
        // Arrange
        var mockClient = new Mock<IGitHubApiClient>();
        mockClient
            .Setup( c => c.GetRepositoryTreeAsync( It.IsAny<string>() ) )
            .ReturnsAsync( new List<TreeItem> {
                new("A", "mode", TreeType.Tree, 0, "sha", "url"),
                new("A/A1", "mode", TreeType.Tree, 0, "sha", "url"),
                new("A/A1/file.exp", "mode", TreeType.Blob, 128, "sha", "url"),
                new("B", "mode", TreeType.Tree, 0, "sha", "url"),
            } );
        var service = new RepositoryService( mockClient.Object );

        // Act
        var actual = await service.GetRepositoryTreeAsync();

        // Assert
        Assert.Equal( 2, actual.Count );
        Assert.Equal( new string[] { "A", "B" }, actual.Select( itm => itm.Name ).ToArray().OrderBy( x => x ) );
        Assert.Equal( "A", actual.First( itm => itm.Name == "A" ).AbsolutePath );
        Assert.True( actual.First( itm => itm.Name == "A" ).IsDirectory );
        Assert.Single( actual.First( itm => itm.Name == "A" ).Children );

        // A/A1
        Assert.Equal( "A/A1", actual.First( itm => itm.Name == "A" ).Children.First( itm => itm.Name == "A1" ).AbsolutePath );
        Assert.True( actual.First( itm => itm.Name == "A" ).Children.First( itm => itm.Name == "A1" ).IsDirectory );
        Assert.Single( actual.First( itm => itm.Name == "A" ).Children.First( itm => itm.Name == "A1" ).Children );

        Assert.Equal(
            "A/A1/file.exp",
            actual.First( itm => itm.Name == "A" )
            .Children.First( itm => itm.Name == "A1" )
            .Children.First( itm => itm.Name == "file.exp" ).AbsolutePath );
        Assert.False(
            actual.First( itm => itm.Name == "A" )
            .Children.First( itm => itm.Name == "A1" )
            .Children.First( itm => itm.Name == "file.exp" ).IsDirectory );
        Assert.Empty(
            actual.First( itm => itm.Name == "A" )
            .Children.First( itm => itm.Name == "A1" )
            .Children.First( itm => itm.Name == "file.exp" ).Children );

        Assert.Equal( "B", actual.First( itm => itm.Name == "B" ).AbsolutePath );
        Assert.True( actual.First( itm => itm.Name == "B" ).IsDirectory );
        Assert.Empty( actual.First( itm => itm.Name == "B" ).Children );

        Assert.True( true );
    }

    [Fact( DisplayName = "ファイルがバイト列で取得できる" )]
    public async Task GetFileAsync_ShouldReturnBytes() {
        // Arrange
        var expected = new byte[] { 0xE3, 0x82, 0xB5, 0xE3, 0x83, 0xB3, 0xE3, 0x83, 0x97, 0xE3, 0x83, 0xAB };
        var mockClient = new Mock<IGitHubApiClient>();
        mockClient
            .Setup( c => c.GetFileAsync( "path/to/file.exp", "master" ) )
            .ReturnsAsync( expected );
        var service = new RepositoryService( mockClient.Object );

        // Act
        var actual = await service.GetFileAsync("path/to/file.exp");

        // Assert
        Assert.Equal( expected, actual );
    }

    [Fact( DisplayName = "ブランチが作成できる" )]
    public async Task CreateBranchAsync_ShouldCreateBranch() {
        // Arrange
        var mockClient = new Mock<IGitHubApiClient>();
        mockClient
            .Setup( c => c.CreateBranchAsync( "master", "new-branch" ) )
            .Returns( Task.CompletedTask );
        var service = new RepositoryService( mockClient.Object );

        // Act
        await service.CreateBranchAsync( "new-branch" );

        // Assert
        mockClient.Verify( c => c.CreateBranchAsync( "master", "new-branch" ), Times.Once );
    }

    [Fact( DisplayName = "一つのファイルをコミットができる" )]
    public async Task CommitAsync_ShouldCommitSingleFile() {
        // Arrange
        var mockClient = new Mock<IGitHubApiClient>();
        mockClient
            .Setup( c => c.CommitAsync( "new-branch", It.IsAny<IEnumerable<CommitFile>>(), "commit message" ) )
            .Returns( Task.CompletedTask );
        var service = new RepositoryService( mockClient.Object );
        var commitFile = new CommitFile
        {
            LocalPath = "C://path/to/file.exp",
            RepoPath = "path/to/file.exp",
            Operation = CommitOperation.AddOrUpdate
        };

        // Act
        await service.CommitAsync( "new-branch", commitFile, "commit message" );

        // Assert
        mockClient.Verify( c =>
            c.CommitAsync( "new-branch", new CommitFile[] { commitFile }, "commit message" ),
            Times.Once );
    }

    [Fact( DisplayName = "複数ファイルコミットができる" )]
    public async Task CommitAsync_ShouldCommitFiles() {
        // Arrange
        var mockClient = new Mock<IGitHubApiClient>();
        mockClient
            .Setup( c => c.CommitAsync( "new-branch", It.IsAny<IEnumerable<CommitFile>>(), "commit message" ) )
            .Returns( Task.CompletedTask );
        var service = new RepositoryService( mockClient.Object );
        List<CommitFile> commitFiles = new ()
        {
            new(){LocalPath = @"D:\Projects\DCS\DTT_PrismDryIoc\obj\dummy1.txt",RepoPath = "path/to/file1.exp",Operation = CommitOperation.AddOrUpdate},
            new(){LocalPath = @"D:\Projects\DCS\DTT_PrismDryIoc\obj\dummy2.txt",RepoPath = "path/to/file2.exp",Operation = CommitOperation.AddOrUpdate},
            new(){LocalPath = @"D:\Projects\DCS\DTT_PrismDryIoc\obj\dummy3.txt",RepoPath = "path/to/file3.exp",Operation = CommitOperation.AddOrUpdate},
        };
        // Act
        await service.CommitAsync( "new-branch", commitFiles, "commit message" );

        // Assert
        mockClient.Verify( c => c.CommitAsync( "new-branch", commitFiles, "commit message" ), Times.Once );
    }

    [Fact( DisplayName = "プルリクエストが作成できる" )]
    public async Task CreatePullRequestAsync_ShouldCreatePullRequest() {
        // Arrange
        var expected = new PullRequest();
        var mockClient = new Mock<IGitHubApiClient>();
        mockClient
            .Setup( c => c.CreatePullRequestAsync( "new-branch", "master", "PR title", "message" ) )
            .Returns( Task.CompletedTask );
        var service = new RepositoryService( mockClient.Object );

        // Act
        await service.CreatePullRequestAsync( "new-branch", "PR title", "message" );

        // Assert
        mockClient.Verify( c =>
            c.CreatePullRequestAsync( "new-branch", "master", "PR title", "message" ),
            Times.Once );
    }

    [Fact( DisplayName = "ツリー取得時にAPIが失敗すると例外が送出される" )]
    public async Task GetRepositoryTreeAsync_ApiError_ThrowsException() {
        // Arrange
        var mockClient = new Mock<IGitHubApiClient>();
        mockClient
            .Setup( c => c.GetRepositoryTreeAsync( It.IsAny<string>() ) )
            .ThrowsAsync( new Exception( "api error" ) );
        var service = new RepositoryService( mockClient.Object );

        // Act & Assert
        await Assert.ThrowsAsync<Exception>( async () => {
            await service.GetRepositoryTreeAsync();
        } );
    }

    [Fact( DisplayName = "ファイル取得時にAPIが失敗すると例外が送出される" )]
    public async Task GetFileAsync_ApiError_ThrowsException() {
        // Arrange
        var mockClient = new Mock<IGitHubApiClient>();
        mockClient
            .Setup( c => c.GetFileAsync( It.IsAny<string>(), It.IsAny<string>() ) )
            .ThrowsAsync( new Exception( "api error" ) );
        var service = new RepositoryService( mockClient.Object );

        // Act & Assert
        await Assert.ThrowsAsync<Exception>( async () => {
            await service.GetFileAsync( "path" );
        } );
    }

    [Fact( DisplayName = "ブランチ作成時にAPIが失敗すると例外が送出される" )]
    public async Task CreateBranchAsync_ApiError_ThrowsException() {
        // Arrange
        var mockClient = new Mock<IGitHubApiClient>();
        mockClient
            .Setup( c => c.CreateBranchAsync( It.IsAny<string>(), It.IsAny<string>() ) )
            .ThrowsAsync( new Exception( "api error" ) );
        var service = new RepositoryService( mockClient.Object );

        // Act & Assert
        await Assert.ThrowsAsync<Exception>( async () => {
            await service.CreateBranchAsync( "branch" );
        } );
    }

    [Fact( DisplayName = "コミット時にAPIが失敗すると例外が送出される" )]
    public async Task CommitAsync_ApiError_ThrowsException() {
        // Arrange
        var mockClient = new Mock<IGitHubApiClient>();
        mockClient
            .Setup( c => c.CommitAsync( It.IsAny<string>(), It.IsAny<IEnumerable<CommitFile>>(), It.IsAny<string>() ) )
            .ThrowsAsync( new Exception( "api error" ) );
        var service = new RepositoryService( mockClient.Object );
        var commitFile = new CommitFile {
            LocalPath = "C://dummy.txt",
            RepoPath = "dummy.txt",
            Operation = CommitOperation.AddOrUpdate
        };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>( async () => {
            await service.CommitAsync( "branch", commitFile, "message" );
        } );
    }

    [Fact( DisplayName = "プルリクエスト作成時にAPIが失敗すると例外が送出される" )]
    public async Task CreatePullRequestAsync_ApiError_ThrowsException() {
        // Arrange
        var mockClient = new Mock<IGitHubApiClient>();
        mockClient
            .Setup( c => c.CreatePullRequestAsync( It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>() ) )
            .ThrowsAsync( new Exception( "api error" ) );
        var service = new RepositoryService( mockClient.Object );

        // Act & Assert
        await Assert.ThrowsAsync<Exception>( async () => {
            await service.CreatePullRequestAsync( "branch", "title", "message" );
        } );
    }
}
