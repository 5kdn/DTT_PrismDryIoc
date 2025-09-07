using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Enums;
using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Core.Services;

using Moq;

using Xunit;

namespace DcsTranslateTool.Tests.Core.Services;

public class RepositoryServiceTests {
    #region GetFileEntryAsync

    [Fact]
    public async Task GetRepositoryTreeAsyncはAPIを呼び出す() {
        // Arrange
        var mockClient = new Mock<IGitHubApiClient>();
        var service = new RepositoryService( mockClient.Object );

        // Act
        await service.GetFileEntriesAsync();

        // Assert
        mockClient.Verify( m => m.GetFileEntriesAsync( It.IsAny<string>() ), Times.Once );
    }

    [Fact]
    public async Task GetRepositoryTreeAsyncはエントリ取得時に成功を返す() {
        // Arrange
        var expected = new FileEntry[] { new RepoFileEntry("a", "a", false, "sha") };
        var mockClient = new Mock<IGitHubApiClient>();
        mockClient
            .Setup( m => m.GetFileEntriesAsync( It.IsAny<string>() ) )
            .ReturnsAsync( expected );
        var service = new RepositoryService( mockClient.Object );

        // Act
        var result = await service.GetFileEntriesAsync();

        // Assert
        Assert.True( result.IsSuccess );
        Assert.Equal( expected, result.Value );
    }

    [Fact]
    public async Task GetRepositoryTreeAsyncはAPI例外時に失敗を返す() {
        // Arrange
        var mockClient = new Mock<IGitHubApiClient>();
        mockClient
            .Setup( m => m.GetFileEntriesAsync( It.IsAny<string>() ) )
            .ThrowsAsync( new Exception( "error" ) );
        var service = new RepositoryService( mockClient.Object );

        // Act
        var result = await service.GetFileEntriesAsync();

        // Assert
        Assert.True( result.IsFailed );
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
    public async Task GetFileAsyncはAPI例外時に失敗を返す() {
        // Arrange
        var mockClient = new Mock<IGitHubApiClient>();
        mockClient
            .Setup( c => c.GetFileAsync( It.IsAny<string>(), It.IsAny<string>() ) )
            .ThrowsAsync( new Exception( "error" ) );
        var service = new RepositoryService( mockClient.Object );

        // Act
        var result = await service.GetFileAsync( "path" );

        // Assert
        Assert.True( result.IsFailed );
    }

    #endregion

    #region CreateBranchAsync

    [Fact]
    public async Task CreateBranchAsyncはブランチ名が有効なときにAPIを呼び出す() {
        // Arrange
        var mockClient = new Mock<IGitHubApiClient>();
        mockClient
            .Setup( c => c.CreateBranchAsync( "default-branch-name", "new-branch" ) )
            .Returns( Task.CompletedTask );
        var service = new RepositoryService( mockClient.Object, "default-branch-name" );

        // Act
        await service.CreateBranchAsync( "new-branch" );

        // Assert
        mockClient.Verify( c => c.CreateBranchAsync( "default-branch-name", "new-branch" ), Times.Once );
    }

    [Fact]
    public async Task CreateBranchAsyncはAPI例外時に失敗を返す() {
        // Arrange
        var mockClient = new Mock<IGitHubApiClient>();
        mockClient
            .Setup( c => c.CreateBranchAsync( It.IsAny<string>(), It.IsAny<string>() ) )
            .ThrowsAsync( new Exception( "err" ) );
        var service = new RepositoryService( mockClient.Object );

        // Act
        var result = await service.CreateBranchAsync( "b" );

        // Assert
        Assert.True( result.IsFailed );
    }

    #endregion

    #region CommitAsync

    [Fact]
    public async Task CommitAsyncは複数ファイル指定時にAPIを呼び出す() {
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
    public async Task CommitAsyncは単一ファイル指定時にAPIを呼び出す() {
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
    public async Task CommitAsyncはAPI例外時に失敗を返す() {
        // Arrange
        var mockClient = new Mock<IGitHubApiClient>();
        mockClient.Setup( c => c.CommitAsync( It.IsAny<string>(), It.IsAny<IEnumerable<CommitFile>>(), It.IsAny<string>() ) )
                  .ThrowsAsync( new Exception( "err" ) );
        var service = new RepositoryService( mockClient.Object );
        var file = new CommitFile { LocalPath = "a", RepoPath = "b", Operation = CommitOperationType.AddOrUpdate };

        // Act
        var result = await service.CommitAsync( "b", file, "m" );

        // Assert
        Assert.True( result.IsFailed );
    }

    #endregion

    #region CreatePullRequestAsync

    [Fact]
    public async Task CreatePullRequestAsyncはAPIを呼び出す() {
        // Arrange
        var mockClient = new Mock<IGitHubApiClient>();
        mockClient
            .Setup( c => c.CreatePullRequestAsync( "feature", "master", "title", "msg" ) )
            .Returns( Task.CompletedTask );
        var service = new RepositoryService( mockClient.Object );

        // Act
        var result = await service.CreatePullRequestAsync( "feature", "title", "msg" );

        // Assert
        Assert.True( result.IsSuccess );
        mockClient.Verify( c => c.CreatePullRequestAsync( "feature", "master", "title", "msg" ), Times.Once );
    }

    [Fact]
    public async Task CreatePullRequestAsyncはAPI例外時に失敗を返す() {
        // Arrange
        var mockClient = new Mock<IGitHubApiClient>();
        mockClient
            .Setup( c => c.CreatePullRequestAsync( It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>() ) )
            .ThrowsAsync( new Exception( "err" ) );
        var service = new RepositoryService( mockClient.Object );

        // Act
        var result = await service.CreatePullRequestAsync( "feature", "title", "msg" );

        // Assert
        Assert.True( result.IsFailed );
    }

    #endregion
}