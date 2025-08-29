using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.Enums;
using DcsTranslateTool.Win.ViewModels;

using Xunit;

namespace DcsTranslateTool.Win.Tests.ViewModels;
public class FileEntryViewModelTests {
    [Fact]
    public void コンストラクタでモデルが設定される() {
        // Arrange
        var model = new FileEntry( "file", "path/to/file", false );

        // Act
        var vm = new FileEntryViewModel(model);

        // Assert
        Assert.NotNull( vm );
        Assert.Equal( model, vm.Model );
    }

    [Fact]
    public void ディレクトリでないとき子要素が作られない() {
        // Arrange
        var model = new FileEntry("Parent.exp", "Parent.exp", false);
        // Act
        var vm = new FileEntryViewModel(model);

        // Assert
        Assert.Empty( vm.Children );
    }

    [Fact]
    public void ディレクトリでCheckStateをCheckedに設定したとき子も同じ状態になる() {
        // Arrange
        var parentModel = new FileEntry("Parent", "Parent", true);
        var childModel = new FileEntry ("Child.exp", "Parent/Child.exp", false);
        var childViewModel = new FileEntryViewModel(childModel);

        var vm = new FileEntryViewModel(parentModel);
        vm.Children.Add( childViewModel );

        // Act
        vm.CheckState = CheckState.Checked;

        // Assert
        Assert.Equal( CheckState.Checked, vm.CheckState );
        Assert.Equal( CheckState.Checked, childViewModel.CheckState );
    }

    #region GetCheckedModelRecursice

    [Fact]
    [Trait( "Category", "WindowsOnly" )]
    public void GetCheckedModelRecursiceは選択された子のみ返す() {
        // Arrange
        var parentModel = new FileEntry("Parent", "Parent", true);
        var childModel1 = new FileEntry("Child1", "Parent/Child1", false);
        var childModel2 = new FileEntry("Child2", "Parent/Child2", false);

        var childVm1 = new FileEntryViewModel(childModel1);
        var childVm2 = new FileEntryViewModel(childModel2) { CheckState = CheckState.Checked };

        var parentVm = new FileEntryViewModel(parentModel);
        parentVm.Children.Clear();
        parentVm.Children.Add( childVm1 );
        parentVm.Children.Add( childVm2 );

        // Act
        var result = parentVm.GetCheckedModelRecursice();

        // Assert
        Assert.DoesNotContain( childModel1, result );
        Assert.Contains( childModel2, result );
        Assert.DoesNotContain( parentModel, result );
    }

    #endregion
}