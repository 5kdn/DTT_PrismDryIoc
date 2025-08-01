using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Win.Contracts.Services;
using DcsTranslateTool.Win.ViewModels;

using Moq;

using Xunit;

namespace DcsTranslateTool.Win.Tests.ViewModels;
public class CreatePullRequestDialogViewModelTests {
    #region CanCreatePR

    [Fact]
    [Trait( "Category", "WindowsOnly" )]
    public void CanCreatePRは変更点と同意がいずれも未選択のときFalseになる() {
        // Arrange
        var mockedAppSettingsService = new Mock<IAppSettingsService>();
        var mockedRepositoryService = new Mock<IRepositoryService>();
        var vm = new CreatePullRequestDialogViewModel(mockedAppSettingsService.Object, mockedRepositoryService.Object);

        // Act
        var result = vm.CanCreatePR;

        // Assert
        Assert.False( result );
    }

    [Fact]
    [Trait( "Category", "WindowsOnly" )]
    public void CanCreatePRは変更点のみチェックしたときFalseになる() {
        // Arrange
        var mockedAppSettingsService = new Mock<IAppSettingsService>();
        var mockedRepositoryService = new Mock<IRepositoryService>();
        var vm = new CreatePullRequestDialogViewModel(mockedAppSettingsService.Object, mockedRepositoryService.Object);
        foreach(var kind in vm.PullRequestChangeKinds) kind.IsChecked = true;

        // Act
        var result = vm.CanCreatePR;

        // Assert
        Assert.False( result );
    }

    [Fact]
    [Trait( "Category", "WindowsOnly" )]
    public void CanCreatePRはすべての変更点未チェックで同意のみtrueのときFalseになる() {
        // Arrange
        var mockedAppSettingsService = new Mock<IAppSettingsService>();
        var mockedRepositoryService = new Mock<IRepositoryService>();
        var vm = new CreatePullRequestDialogViewModel(mockedAppSettingsService.Object, mockedRepositoryService.Object);
        foreach(var item in vm.AgreementItems) item.IsAgreed = true;

        // Act
        var result = vm.CanCreatePR;

        // Assert
        Assert.False( result );
    }

    [Fact]
    [Trait( "Category", "WindowsOnly" )]
    public void CanCreatePRは変更点と同意がともにチェック済みのときTrueになる() {
        // Arrange
        var mockedAppSettingsService = new Mock<IAppSettingsService>();
        var mockedRepositoryService = new Mock<IRepositoryService>();
        var vm = new CreatePullRequestDialogViewModel(mockedAppSettingsService.Object, mockedRepositoryService.Object);
        vm.PullRequestChangeKinds.First().IsChecked = true;
        foreach(var item in vm.AgreementItems) item.IsAgreed = true;

        // Act
        var result = vm.CanCreatePR;

        // Assert
        Assert.True( result );
    }

    #endregion

    #region PRTitle

    [Fact]
    [Trait( "Category", "WindowsOnly" )]
    public void PRTitleはチェックした変更点のDisplayNameがタイトルに含まれるようになる() {
        // TODO: テストを作成
    }

    #endregion

    #region PRComment

    [Fact]
    [Trait( "Category", "WindowsOnly" )]
    public void PRCommentは値を設定したとき同じ値が取得できるようになる() {
        // Arrange
        var mockedAppSettingsService = new Mock<IAppSettingsService>();
        var mockedRepositoryService = new Mock<IRepositoryService>();
        var vm = new CreatePullRequestDialogViewModel(mockedAppSettingsService.Object, mockedRepositoryService.Object);
        const string expected = "Test Comment";

        // Act
        vm.PRComment = expected;

        // Assert
        Assert.Equal( expected, vm.PRComment );
    }

    #endregion

    #region SelectedChangeKinds

    [Fact]
    [Trait( "Category", "WindowsOnly" )]
    public void SelectedChangeKindsはIsCheckedな変更種別のみ返すようになる() {
        // Arrange
        var mockedAppSettingsService = new Mock<IAppSettingsService>();
        var mockedRepositoryService = new Mock<IRepositoryService>();
        var vm = new CreatePullRequestDialogViewModel(mockedAppSettingsService.Object, mockedRepositoryService.Object);
        vm.PullRequestChangeKinds[0].IsChecked = true;
        vm.PullRequestChangeKinds[1].IsChecked = false;
        vm.PullRequestChangeKinds[2].IsChecked = true;

        // Act
        var kinds = vm.SelectedChangeKinds.ToList();

        // Assert
        Assert.Contains( vm.PullRequestChangeKinds[0].Kind, kinds );
        Assert.DoesNotContain( vm.PullRequestChangeKinds[1].Kind, kinds );
        Assert.Contains( vm.PullRequestChangeKinds[2].Kind, kinds );
    }

    #endregion
}
