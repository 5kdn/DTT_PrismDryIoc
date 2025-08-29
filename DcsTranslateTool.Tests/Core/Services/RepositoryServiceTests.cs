using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Enums;
using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Core.Services;

using Moq;

using Xunit;

namespace DcsTranslateTool.Core.Tests.Services;

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

    #endregion

    #region CreateBranchAsync

    [Fact]
    public async Task CreateBranchAsyncはブランチ名が有効なときにAPIを呼び出す() {
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

    #endregion

}