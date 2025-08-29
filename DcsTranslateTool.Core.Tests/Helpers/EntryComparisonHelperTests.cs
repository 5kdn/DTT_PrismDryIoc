using DcsTranslateTool.Core.Helpers;
using DcsTranslateTool.Core.Models;

using Xunit;

namespace DcsTranslateTool.Core.Tests.Helpers;

public class EntryComparisonHelperTests {
    #region Merge

    [Fact]
    public void Mergeは両方に存在し内容が同じときローカルとリポジトリのSHA1が同じになる() {
        // Arrange
        var local = new Entry( "file.txt", "file.txt", false, "a" );
        var repo = new Entry( "file.txt", "file.txt", false, null, "a" );

        // Act
        var result = EntryComparisonHelper.Merge( [local], [repo] ).First();

        // Assert
        Assert.Equal( "a", result.LocalSha );
        Assert.Equal( "a", result.RepoSha );
    }

    [Fact]
    public void Mergeはローカルのみ存在するときローカルのSHA1のみ設定される() {
        // Arrange
        var local = new Entry( "file.txt", "file.txt", false, "a" );

        // Act
        var result = EntryComparisonHelper.Merge( [local], [] ).First();

        // Assert
        Assert.Equal( "a", result.LocalSha );
        Assert.Null( result.RepoSha );
    }

    [Fact]
    public void Mergeはリポジトリのみ存在するときリポジトリのSHA1のみ設定される() {
        // Arrange
        var repo = new Entry( "file.txt", "file.txt", false, null, "a" );

        // Act
        var result = EntryComparisonHelper.Merge( [], [repo] ).First();

        // Assert
        Assert.Null( result.LocalSha );
        Assert.Equal( "a", result.RepoSha );
    }

    [Fact]
    public void Mergeは内容が異なるときローカルとリポジトリのSHA1が異なる() {
        // Arrange
        var local = new Entry( "file.txt", "file.txt", false, "a" );
        var repo = new Entry( "file.txt", "file.txt", false, null, "b" );

        // Act
        var result = EntryComparisonHelper.Merge( [local], [repo] ).First();

        // Assert
        Assert.Equal( "a", result.LocalSha );
        Assert.Equal( "b", result.RepoSha );
    }

    #endregion
}