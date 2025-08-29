using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.Contracts.ViewModels.Factories;
using DcsTranslateTool.Win.Enums;
using DcsTranslateTool.Win.ViewModels;

using FluentResults;

using Moq;

using Xunit;

namespace DcsTranslateTool.Win.Tests.ViewModels;

/// <summary>
/// <see cref="EntryViewModel"/> のテストである。
/// </summary>
public class EntryViewModelTests {
    [Fact]
    /// <summary>
    /// コンストラクタでモデルが設定される。
    /// </summary>
    public void コンストラクタでモデルが設定される() {
        // Arrange
        var factoryMock = new Mock<IEntryViewModelFactory>();
        var serviceMock = new Mock<IFileEntryService>();
        var model = new Entry( "file", "path/to/file", false );

        // Act
        var vm = new EntryViewModel(factoryMock.Object, serviceMock.Object, model);

        // Assert
        Assert.NotNull( vm );
        Assert.Equal( model, vm.Model );
    }

    [Fact]
    /// <summary>
    /// ディレクトリを読み込んだ時に子要素が生成される。
    /// </summary>
    public void ディレクトリを読み込んだ時に子要素が生成される() {
        // Arrange
        var factoryMock = new Mock<IEntryViewModelFactory>();
        var serviceMock = new Mock<IFileEntryService>();

        var parentModel = new Entry("Parent", "Parent", true);
        var childModel = new Entry ("Child.exp", "Parent/Child.exp", false);
        var childViewModel = new EntryViewModel(factoryMock.Object, serviceMock.Object, childModel);

        serviceMock
            .Setup( s => s.GetChildren( parentModel ) )
            .Returns( Result.Ok<IEnumerable<Entry>>( [childModel] ) );
        factoryMock
            .Setup( f => f.Create( childModel, It.IsAny<EntryViewModel?>(), CheckState.Unchecked ) )
            .Returns( childViewModel );

        var vm = new EntryViewModel(factoryMock.Object, serviceMock.Object, parentModel);

        // Act
        vm.LoadChildren();

        // Assert
        Assert.Single( vm.Children );
        Assert.Equal( "Child.exp", vm.Children[0]?.Name );
        Assert.Equal( "Parent/Child.exp", vm.Children[0]?.Path );
        Assert.False( vm.Children[0]?.IsDirectory );
        Assert.Equal( childViewModel, vm.Children[0] );
    }

    [Fact]
    /// <summary>
    /// 子要素を複数回取得しても子要素が重複しない。
    /// </summary>
    public void 子要素を複数回取得しても子要素が重複しない() {
        // Arrange
        var factoryMock = new Mock<IEntryViewModelFactory>();
        var serviceMock = new Mock<IFileEntryService>();
        var parentModel = new Entry("Parent", "Parent", true);
        var childModel = new Entry ("Child.exp", "Parent/Child.exp", false);
        var childViewModel = new EntryViewModel(factoryMock.Object, serviceMock.Object, childModel);

        serviceMock
            .Setup( s => s.GetChildren( parentModel ) )
            .Returns( Result.Ok<IEnumerable<Entry>>( [childModel] ) );
        factoryMock
            .Setup( f => f.Create( childModel, It.IsAny<EntryViewModel?>(), CheckState.Unchecked ) )
            .Returns( childViewModel );

        var vm = new EntryViewModel(factoryMock.Object, serviceMock.Object, parentModel);

        // Act
        vm.LoadChildren();
        vm.LoadChildren();
        vm.LoadChildren();

        // Assert
        Assert.Single( vm.Children );
        Assert.Equal( "Child.exp", vm.Children[0]?.Name );
        serviceMock.Verify( s => s.GetChildren( parentModel ), Times.Once ); // 1回のみ呼ばれる
    }

    [Fact]
    /// <summary>
    /// ディレクトリでないとき子要素が作られない。
    /// </summary>
    public void ディレクトリでないとき子要素が作られない() {
        // Arrange
        var factoryMock = new Mock<IEntryViewModelFactory>();
        var serviceMock = new Mock<IFileEntryService>();
        var model = new Entry("Parent.exp", "Parent.exp", false);
        // Act
        var vm = new EntryViewModel(factoryMock.Object, serviceMock.Object, model);

        // Assert
        Assert.Empty( vm.Children );
    }

    [Fact]
    /// <summary>
    /// ディレクトリでCheckStateをCheckedに設定したとき子も同じ状態になる。
    /// </summary>
    public void ディレクトリでCheckStateをCheckedに設定したとき子も同じ状態になる() {
        // Arrange
        var factoryMock = new Mock<IEntryViewModelFactory>();
        var serviceMock = new Mock<IFileEntryService>();
        var parentModel = new Entry("Parent", "Parent", true);
        var childModel = new Entry ("Child.exp", "Parent/Child.exp", false);
        var childViewModel = new EntryViewModel(factoryMock.Object, serviceMock.Object, childModel);

        var vm = new EntryViewModel(factoryMock.Object, serviceMock.Object, parentModel);
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
    /// <summary>
    /// GetCheckedModelRecursiceは選択された子のみ返す。
    /// </summary>
    public void GetCheckedModelRecursiceは選択された子のみ返す() {
        // Arrange
        var factoryMock = new Mock<IEntryViewModelFactory>();
        var serviceMock = new Mock<IFileEntryService>();
        var parentModel = new Entry("Parent", "Parent", true);
        var childModel1 = new Entry("Child1", "Parent/Child1", false);
        var childModel2 = new Entry("Child2", "Parent/Child2", false);

        var childVm1 = new EntryViewModel(factoryMock.Object, serviceMock.Object, childModel1);
        var childVm2 = new EntryViewModel(factoryMock.Object, serviceMock.Object, childModel2) { CheckState = CheckState.Checked };

        var parentVm = new EntryViewModel(factoryMock.Object, serviceMock.Object, parentModel);
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