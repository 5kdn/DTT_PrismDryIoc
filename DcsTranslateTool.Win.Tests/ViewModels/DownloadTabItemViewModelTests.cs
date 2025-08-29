using DcsTranslateTool.Core.Enums;
using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.Enums;
using DcsTranslateTool.Win.ViewModels;

using Xunit;

namespace DcsTranslateTool.Win.Tests.ViewModels;

/// <summary>
/// <see cref="DownloadTabItemViewModel"/>のテストである。
/// </summary>
public class DownloadTabItemViewModelTests {
    #region ApplyFilter

    /// <summary>
    /// ApplyFilterはフィルタなしのとき元のルートが設定される。
    /// </summary>
    [Fact]
    [Trait( "Category", "WindowsOnly" )]
    public void ApplyFilterはフィルタなしのとき元のルートが設定される() {
        // Arrange
        var root = new FileEntryViewModel(new Entry("root", "", true));
        var vm = new DownloadTabItemViewModel(RootTabType.Aircraft, root);

        // Act
        vm.ApplyFilter( [] );

        // Assert
        Assert.Same( root, vm.Root );
    }

    /// <summary>
    /// ApplyFilterはFileChangeTypeを指定したとき該当するエントリのみ残る。
    /// </summary>
    [Fact]
    [Trait( "Category", "WindowsOnly" )]
    public void ApplyFilterはFileChangeTypeを指定したとき該当するエントリのみ残る() {
        // Arrange
        var root = new FileEntryViewModel(new Entry("root", "", true));
        var modified = new FileEntryViewModel(new Entry("mod.txt", "mod.txt", false, "a", "b"));
        var deleted = new FileEntryViewModel(new Entry("del.txt", "del.txt", false, null, "a"));
        root.Children.Add( modified );
        root.Children.Add( deleted );
        var vm = new DownloadTabItemViewModel(RootTabType.Aircraft, root);

        // Act
        vm.ApplyFilter( [FileChangeType.Modified] );

        // Assert
        Assert.Single( vm.Root.Children );
        Assert.Equal( "mod.txt", vm.Root.Children[0]!.Name );
    }

    #endregion
}