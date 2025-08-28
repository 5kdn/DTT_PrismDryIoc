using DcsTranslateTool.Core.Enums;
using DcsTranslateTool.Core.Helpers;
using DcsTranslateTool.Core.Models;

using Xunit;

namespace DcsTranslateTool.Core.Tests.Helpers;

public class EntryComparisonHelperTests {
    #region Merge

    [Fact]
    public void Mergeは両方に存在し内容が同じときUnchangedになる() {
        // Arrange
        var local = new Entry( "file.txt", "file.txt", false, "a" );
        var repo = new Entry( "file.txt", "file.txt", false, null, "a" );

        // Act
        var result = EntryComparisonHelper.Merge( [local], [repo] ).First();

        // Assert
        Assert.Equal( FileChangeType.Unchanged, result.ChangeType );
    }

    [Fact]
    public void Mergeはローカルのみ存在するときAddedになる() {
        // Arrange
        var local = new Entry( "file.txt", "file.txt", false, "a" );

        // Act
        var result = EntryComparisonHelper.Merge( [local], [] ).First();

        // Assert
        Assert.Equal( FileChangeType.Added, result.ChangeType );
    }

    [Fact]
    public void Mergeはリポジトリのみ存在するときDeletedになる() {
        // Arrange
        var repo = new Entry( "file.txt", "file.txt", false, null, "a" );

        // Act
        var result = EntryComparisonHelper.Merge( [], [repo] ).First();

        // Assert
        Assert.Equal( FileChangeType.Deleted, result.ChangeType );
    }

    [Fact]
    public void Mergeは内容が異なるときModifiedになる() {
        // Arrange
        var local = new Entry( "file.txt", "file.txt", false, "a" );
        var repo = new Entry( "file.txt", "file.txt", false, null, "b" );

        // Act
        var result = EntryComparisonHelper.Merge( [local], [repo] ).First();

        // Assert
        Assert.Equal( FileChangeType.Modified, result.ChangeType );
    }

    #endregion
}