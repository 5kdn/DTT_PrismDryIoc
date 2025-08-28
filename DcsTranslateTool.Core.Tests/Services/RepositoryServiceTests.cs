using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Enums;
using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Core.Services;

using Moq;

using Xunit;

namespace DcsTranslateTool.Core.Tests.Services;

public class RepositoryServiceTests {
    #region GetRepositoryEntryAsync

    [Fact]
    public async Task GetRepositoryTreeAsyncはリポジトリツリーが存在するときにRepoEntryリストを返す() {
        // Arrange
        var mockClient = new Mock<IGitHubApiClient>();
        var service = new RepositoryService( mockClient.Object );

        // Act
        await service.GetRepositoryEntryAsync();

        // Assert
        mockClient.Verify( m => m.GetRepositoryEntriesAsync( It.IsAny<string>() ), Times.Once );
    }

    [Fact]
    public async Task GetRepositoryTreeAsyncはAPI例外が発生すると例外が伝播する() {
        // Arrange
        var mockClient = new Mock<IGitHubApiClient>();
        mockClient
            .Setup( c => c.GetRepositoryEntriesAsync( It.IsAny<string>() ) )
            .ThrowsAsync( new Exception( "API error" ) );
        var service = new RepositoryService( mockClient.Object );

        // Act & Assert
        await Assert.ThrowsAsync<Exception>( async () => await service.GetRepositoryEntryAsync() );
    }

    #endregion

    #region GetFileAsync

    [Fact]
    public async Task GetFileAsyncはファイルパスが正しいときにバイト配列を返す() {
        // Arrange
#pragma warning disable IDE0230 // UTF-8 文字列リテラルを使用する
        var expected = new byte[] { 0xE3, 0x82, 0xB5, 0xE3, 0x83, 0xB3, 0xE3, 0x83, 0x97, 0xE3, 0x83, 0xAB };
#pragma warning restore IDE0230 // UTF-8 文字列リテラルを使用する
        var mockClient = new Mock<IGitHubApiClient>();
        mockClient
            .Setup( c => c.GetFileAsync( "path/to/file.exp", "master" ) )
            .ReturnsAsync( expected );
        var service = new RepositoryService( mockClient.Object );

        // Act
        var result = await service.GetFileAsync("path/to/file.exp");

        // Assert
        Assert.True( result.IsSuccess );
        Assert.Equal( expected, result.Value );
    }

    [Fact]
    public async Task GetFileAsyncはAPI例外が発生したとき例外が伝播する() {
        // Arrange
        var mockClient = new Mock<IGitHubApiClient>();
        mockClient
            .Setup( c => c.GetFileAsync( It.IsAny<string>(), It.IsAny<string>() ) )
            .ThrowsAsync( new Exception( "API error" ) );
        var service = new RepositoryService( mockClient.Object );

        // Act & Assert
        await Assert.ThrowsAsync<Exception>( async () => await service.GetFileAsync( "path/to/file.exp" ) );
    }

    #endregion

    #region CreateBranchAsync

    [Fact]
    public async Task CreateBranchAsyncはブランチ名が有効なときにAPIが呼び出される() {
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

    [Fact]
    public async Task CreateBranchAsyncはAPI例外が発生したとき例外が伝播する() {
        // Arrange
        var mockClient = new Mock<IGitHubApiClient>();
        mockClient
            .Setup( c => c.CreateBranchAsync( It.IsAny<string>(), It.IsAny<string>() ) )
            .ThrowsAsync( new Exception( "API error" ) );
        var service = new RepositoryService( mockClient.Object );

        // Act & Assert
        await Assert.ThrowsAsync<Exception>( async () => await service.CreateBranchAsync( "new-branch" ) );
    }

    #endregion

    #region CommitAsync

    [Fact]
    public async Task CommitAsyncは複数ファイル指定時にAPIが呼び出される() {
        // Arrange
        var mockClient = new Mock<IGitHubApiClient>();
        mockClient
            .Setup( c => c.CommitAsync( "new-branch", It.IsAny<IEnumerable<CommitFile>>(), "commit message" ) )
            .Returns( Task.CompletedTask );
        var service = new RepositoryService( mockClient.Object );
        List<CommitFile> commitFiles =
        [
            new(){LocalPath = @"D:\Projects\DCS\DTT_PrismDryIoc\obj\dummy1.txt",RepoPath = "path/to/file1.exp",Operation = CommitOperationType.AddOrUpdate},
            new(){LocalPath = @"D:\Projects\DCS\DTT_PrismDryIoc\obj\dummy2.txt",RepoPath = "path/to/file2.exp",Operation = CommitOperationType.AddOrUpdate},
            new(){LocalPath = @"D:\Projects\DCS\DTT_PrismDryIoc\obj\dummy3.txt",RepoPath = "path/to/file3.exp",Operation = CommitOperationType.AddOrUpdate},
        ];

        // Act
        await service.CommitAsync( "new-branch", commitFiles, "commit message" );

        // Assert
        mockClient.Verify( c => c.CommitAsync( "new-branch", commitFiles, "commit message" ), Times.Once );
    }

    [Fact]
    public async Task CommitAsyncは単一ファイル指定時にAPIが呼び出される() {
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
            Operation = CommitOperationType.AddOrUpdate
        };

        // Act
        await service.CommitAsync( "new-branch", commitFile, "commit message" );

        // Assert
        mockClient.Verify( c =>
            c.CommitAsync( "new-branch", new CommitFile[] { commitFile }, "commit message" ),
            Times.Once );
    }

    [Fact]
    public async Task CommitAsyncはAPI例外が発生したとき_例外が伝播する() {
        // Arrange
        var mockClient = new Mock<IGitHubApiClient>();
        mockClient
            .Setup( c => c.CommitAsync( It.IsAny<string>(), It.IsAny<IEnumerable<CommitFile>>(), It.IsAny<string>() ) )
            .ThrowsAsync( new Exception( "API Error" ) );
        var service = new RepositoryService( mockClient.Object );
        List<CommitFile> commitFiles =
        [
            new(){LocalPath = @"D:\Projects\DCS\DTT_PrismDryIoc\obj\dummy1.txt",RepoPath = "path/to/file1.exp",Operation = CommitOperationType.AddOrUpdate},
        ];

        // Act & Assert
        await Assert.ThrowsAsync<Exception>( async () => await service.CommitAsync( "new-branch", commitFiles, "commit message" ) );
    }

    #endregion

    #region CreatePullRequestAsync

    //[Fact]
    //public async Task CreatePullRequestAsyncは有効なパラメータでAPIが呼び出される() {
    //    // Arrange
    //    var expected = new PullRequest();
    //    var mockClient = new Mock<IGitHubApiClient>();
    //    mockClient
    //        .Setup( c => c.CreatePullRequestAsync( "new-branch", "master", "PR title", "message" ) )
    //        .Returns( Task.CompletedTask );
    //    var service = new RepositoryService( mockClient.Object );

    //    // Act
    //    await service.CreatePullRequestAsync( "new-branch", "PR title", "message" );

    //    // Assert
    //    mockClient.Verify( c =>
    //        c.CreatePullRequestAsync( "new-branch", "master", "PR title", "message" ),
    //        Times.Once );
    //}

    [Fact]
    public async Task CreatePullRequestAsyncはAPI例外が発生したとき例外が伝播する() {
        // Arrange
        var mockClient = new Mock<IGitHubApiClient>();
        mockClient
            .Setup( c => c.CreatePullRequestAsync( It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>() ) )
            .ThrowsAsync( new Exception( "API Error" ) );
        var service = new RepositoryService( mockClient.Object );

        // Act & Assert
        await Assert.ThrowsAsync<Exception>( async () => await service.CreatePullRequestAsync( "new-branch", "PR title", "message" ) );
    }

    #endregion

}