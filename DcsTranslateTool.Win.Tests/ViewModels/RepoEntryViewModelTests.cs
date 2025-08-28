using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.Enums;
using DcsTranslateTool.Win.ViewModels;

using Xunit;

namespace DcsTranslateTool.Win.Tests.ViewModels;
public class RepoEntryViewModelTests {
    #region SetSelectRecursive

    [Theory]
    [InlineData( true )]
    [InlineData( false )]
    [Trait( "Category", "WindowsOnly" )]
    public void SetSelectRecursiveはファイルエントリで呼び出したとき自身のIsSelectedのみが設定値になる( bool value ) {
        // Arrange
        var fileEntry = new RepoEntry("file.txt", "file.txt", value);
        var fileVm = new RepoEntryViewModel(fileEntry);

        // Act
        fileVm.SetSelectRecursive( value );

        // Assert
        Assert.Equal( value, fileVm.IsSelected );
    }

    [Theory]
    [InlineData( true )]
    [InlineData( false )]
    [Trait( "Category", "WindowsOnly" )]
    public void SetSelectRecursiveは子なしディレクトリで呼び出したとき自身のIsSelectedが設定値_になる( bool value ) {
        // Arrange
        var entry = new RepoEntry("folder", "folder", value);
        var vm = new RepoEntryViewModel(entry);

        // Act
        vm.SetSelectRecursive( value );

        // Assert
        Assert.Equal( value, vm.IsSelected );
    }

    [Theory]
    [InlineData( true )]
    [InlineData( false )]
    [Trait( "Category", "WindowsOnly" )]
    public void SetSelectRecursiveは子を持つディレクトリで呼び出したとき自身と全ての子のIsSelectedが設定値になる( bool value ) {
        // Arrange
        var parent = new RepoEntry("parent", "parent", true);
        var child1 = new RepoEntry("child1", "parent/child1", false);
        var child2 = new RepoEntry("child2", "parent/child2", false);
        var child3 = new RepoEntry("child2", "parent/child2", true);

        var parentVm = new RepoEntryViewModel(parent);
        var childVm1 = new RepoEntryViewModel(child1);
        var childVm2 = new RepoEntryViewModel(child2);
        var childVm3 = new RepoEntryViewModel(child3);

        parentVm.Children.Add( childVm1 );
        parentVm.Children.Add( childVm2 );
        parentVm.Children.Add( childVm3 );

        // Act
        parentVm.SetSelectRecursive( value );

        // Assert
        Assert.Equal( value, parentVm.IsSelected );
        Assert.Equal( value, childVm1.IsSelected );
        Assert.Equal( value, childVm2.IsSelected );
        Assert.Equal( value, childVm3.IsSelected );
    }

    [Theory]
    [InlineData( true )]
    [InlineData( false )]
    [Trait( "Category", "WindowsOnly" )]
    public void SetSelectRecursiveは孫要素を持つとき全階層のIsSelectedが設定値になる( bool value ) {
        // Arrange
        var parent = new RepoEntry("parent", "parent", true);
        var child1 = new RepoEntry("child", "parent/child1", true);
        var child2 = new RepoEntry("child", "parent/child2", false);
        var grandChild1 = new RepoEntry("grandChild", "parent/child1/grandChild1", false);
        var grandChild2 = new RepoEntry("grandChild", "parent/child1/grandChild2", true);

        var parentVm = new RepoEntryViewModel(parent);
        var childVm1 = new RepoEntryViewModel(child1);
        var childVm2 = new RepoEntryViewModel(child2);
        var grandChildVm1 = new RepoEntryViewModel(grandChild1);
        var grandChildVm2 = new RepoEntryViewModel(grandChild2);

        parentVm.Children.Add( childVm1 );
        parentVm.Children.Add( childVm2 );
        childVm1.Children.Add( grandChildVm1 );
        childVm2.Children.Add( grandChildVm2 );

        // Act
        parentVm.SetSelectRecursive( value );

        // Assert
        Assert.Equal( value, parentVm.IsSelected );
        Assert.Equal( value, childVm1.IsSelected );
        Assert.Equal( value, childVm2.IsSelected );
        Assert.Equal( value, grandChildVm1.IsSelected );
        Assert.Equal( value, grandChildVm2.IsSelected );
    }

    #endregion

    #region ApplyFilter

    [Fact]
    [Trait("Category", "WindowsOnly")]
    public void ApplyFilterはフィルタがDownloadedのときStatusがDownloadedのノードのみIsVisibleがTrueになる() {
        // Arrange
        var parent = new RepoEntry("parent", "parent", true);
        var child1 = new RepoEntry("child1", "parent/child1", false);
        var child2 = new RepoEntry("child2", "parent/child2", false);

        var parentVm = new RepoEntryViewModel(parent, DownloadStatus.NotDownloaded);
        var childVm1 = new RepoEntryViewModel(child1, DownloadStatus.Downloaded);
        var childVm2 = new RepoEntryViewModel(child2, DownloadStatus.NotDownloaded);

        parentVm.Children.Add(childVm1);
        parentVm.Children.Add(childVm2);

        // Act
        parentVm.ApplyFilter(new[]{DownloadStatus.Downloaded});

        // Assert
        Assert.True(parentVm.IsVisible);
        Assert.True(childVm1.IsVisible);
        Assert.False(childVm2.IsVisible);
    }

    [Fact]
    [Trait("Category", "WindowsOnly")]
    public void ApplyFilterは子が全て非表示のとき親ディレクトリも非表示になる() {
        // Arrange
        var parent = new RepoEntry("parent", "parent", true);
        var child = new RepoEntry("child", "parent/child", false);

        var parentVm = new RepoEntryViewModel(parent, DownloadStatus.NotDownloaded);
        var childVm = new RepoEntryViewModel(child, DownloadStatus.NotDownloaded);
        parentVm.Children.Add(childVm);

        // Act
        parentVm.ApplyFilter(new[]{DownloadStatus.Downloaded});

        // Assert
        Assert.False(parentVm.IsVisible);
        Assert.False(childVm.IsVisible);
    }

    #endregion
}