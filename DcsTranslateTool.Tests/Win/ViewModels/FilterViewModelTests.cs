using DcsTranslateTool.Win.Enums;
using DcsTranslateTool.Win.ViewModels;

using Xunit;

namespace DcsTranslateTool.Tests.Win.ViewModels;

public class FilterViewModelTests {
    [Fact]
    public void Allをtrueにするとすべての個別フラグがtrueになる() {
        // Arrange
#pragma warning disable IDE0017 // オブジェクトの初期化を簡略化します
        var vm = new FilterViewModel()
        {
            LocalOnly = false,
            RepoOnly = false,
            Modified = false,
            Unchanged = false,
        };
#pragma warning restore IDE0017 // オブジェクトの初期化を簡略化します

        // Act
        vm.All = true;

        // Assert
        Assert.True( vm.LocalOnly );
        Assert.True( vm.RepoOnly );
        Assert.True( vm.Modified );
        Assert.True( vm.Unchanged );
    }

    [Fact]
    public void Allをfalseにするとすべての個別フラグがfalseになる() {
        // Arrange
#pragma warning disable IDE0017 // オブジェクトの初期化を簡略化します
        var vm = new FilterViewModel();
#pragma warning restore IDE0017 // オブジェクトの初期化を簡略化します

        // Act
        vm.All = false;

        // Assert
        Assert.False( vm.Unchanged );
        Assert.False( vm.RepoOnly );
        Assert.False( vm.LocalOnly );
        Assert.False( vm.Modified );
    }

    [Fact]
    public void 個別フラグをfalseにするとAllがfalseになる() {
        // Arrange
#pragma warning disable IDE0017 // オブジェクトの初期化を簡略化します
        var vm = new FilterViewModel();
#pragma warning restore IDE0017 // オブジェクトの初期化を簡略化します

        // Act
        vm.Unchanged = false;

        // Assert
        Assert.False( vm.All );
    }

    [Fact]
    public void 個別フラグを全てtrueにするとAllがtrueになる() {
        // Arrange
        var vm = new FilterViewModel()
        {
            LocalOnly = false,
            RepoOnly = false,
            Modified = false,
            Unchanged = false,
        };

        // Act
        vm.LocalOnly = true;
        vm.RepoOnly = true;
        vm.Modified = true;
        vm.Unchanged = true;

        // Assert
        Assert.True( vm.All );
    }

    [Fact]
    public void GetActiveTypesは選択された種別のみを返す() {
        // Arrange
        var vm = new FilterViewModel()
        {
            LocalOnly = true,
            RepoOnly = true,
        };

        // Act
        var types = vm.GetActiveTypes().ToArray();

        // Assert
        Assert.Contains( FileChangeType.LocalOnly, types );
        Assert.Contains( FileChangeType.RepoOnly, types );
    }

    [Fact]
    public void Unchangedを変更するとFiltersChangedが発火する() {
        // Arrange
        var vm = new FilterViewModel();
        var raised = false;
        vm.FiltersChanged += ( _, _ ) => raised = true;

        // Act
        vm.Unchanged = false;

        // Assert
        Assert.True( raised );
    }
}