using DcsTranslateTool.Core.Services;
using Xunit;

namespace DcsTranslateTool.Core.Tests.Services;

public class GitHubServiceTests
{
    [Fact(DisplayName = "リポジトリ情報が取得できる" )]
    public async Task GetRepositoryAsync_ShouldReturnRepository()
    {
        // Arrange
        var service = new GitHubService();

        // Act
        var repo = await service.GetRepositoryAsync();

        // Assert
        Assert.Equal("5kdn/test_DCS", repo.FullName);
    }

    [Fact(DisplayName = "ツリーが取得できる" )]
    public async Task GetRepositoryTreeAsync_ShouldReturnTree()
    {
        // Arrange
        var service = new GitHubService();

        // Act
        var tree = await service.GetRepositoryTreeAsync();

        // Assert
        Assert.NotEmpty(tree);
    }

    [Fact(DisplayName = "ファイルがバイト列で取得できる" )]
    public async Task GetFileAsync_ShouldReturnBytes()
    {
        // Arrange
        var service = new GitHubService();

        // Act
        var bytes = await service.GetFileAsync(".github/workflows/release_on_merge_to_master.yml");

        // Assert
        Assert.NotEmpty(bytes);
    }
}
