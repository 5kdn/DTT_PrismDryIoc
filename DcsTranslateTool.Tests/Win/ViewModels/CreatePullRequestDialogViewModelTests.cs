using DcsTranslateTool.Win.ViewModels;

using Xunit;

namespace DcsTranslateTool.Win.Tests.ViewModels;
public class CreatePullRequestDialogViewModelTests {
    #region CanCreatePR

    [Fact]
    [Trait( "Category", "WindowsOnly" )]
    public void CanCreatePRは変更点と同意がいずれも未選択のときFalseになる() {
        // Arrange
        var vm = new CreatePullRequestDialogViewModel();

        // Act
        var result = vm.CanCreatePR;

        // Assert
        Assert.False( result );
    }

    [Fact]
    [Trait( "Category", "WindowsOnly" )]
    public void CanCreatePRは変更点のみチェックしたときFalseになる() {
        // Arrange
        var vm = new CreatePullRequestDialogViewModel();
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
        var vm = new CreatePullRequestDialogViewModel();
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
        var vm = new CreatePullRequestDialogViewModel();
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
        var vm = new CreatePullRequestDialogViewModel();
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
        var vm = new CreatePullRequestDialogViewModel();
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