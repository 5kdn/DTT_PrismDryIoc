using DcsTranslateTool.Share.Models;
using DcsTranslateTool.Win.ViewModels;

using Xunit;

namespace DcsTranslateTool.Win.Tests.ViewModels;

public class RepoTreeItemViewModelTests {
    [Fact( DisplayName = "RepoTreeItemViewModelはRepoTreeから正しく初期化される" )]
    public void RepoTreeItemViewModel_InitializesFromRepoTree() {
        // Arrange
        RepoTree tree = new()
        {
            Name = "TestRepo",
            AbsolutePath = "/path/to/repo",
            IsDirectory = true,
            Children = [
                new(){ Name = "Child1", AbsolutePath = "/path/to/repo/child1", IsDirectory = true },
                new(){ Name = "Child2.exp", AbsolutePath = "/path/to/repo/child2.exp", IsDirectory = false }
            ]
        };

        // Act
        var vm = new RepoTreeItemViewModel( tree );

        // Assert
        Assert.Equal( "TestRepo", vm.Name );
        Assert.Equal( "/path/to/repo", vm.AbsolutePath );
        Assert.True( vm.IsDirectory );
        Assert.Equal( 2, vm.Children.Count );
        Assert.Equal( "Child1", vm.Children[0].Name );
        Assert.Equal( "/path/to/repo/child1", vm.Children[0].AbsolutePath );
        Assert.True( vm.Children[0].IsDirectory );
        Assert.Equal( "Child2.exp", vm.Children[1].Name );
        Assert.Equal( "/path/to/repo/child2.exp", vm.Children[1].AbsolutePath );
        Assert.False( vm.Children[1].IsDirectory );
        Assert.Empty( vm.Children[1].Children );
    }

    [Fact( DisplayName = "ディレクトリのチェックが子要素へ伝搬する" )]
    public void DirectoryCheck_PropagatesToChildren() {
        // Arrange
        RepoTree tree = new()
        {
            Name = "TestRepo",
            AbsolutePath = "/path/to/repo",
            IsDirectory = true,
            Children = [
                new(){
                    Name = "Child1",
                    AbsolutePath = "/path/to/repo/child1",
                    IsDirectory = true,
                    Children=[
                        new(){ Name = "GrandChild1.exp", AbsolutePath = "/path/to/repo/child1/grandchild1.exp", IsDirectory = false },
                    ],
                },
                new(){ Name = "Child2.exp", AbsolutePath = "/path/to/repo/child2.exp", IsDirectory = false }
            ]
        };
        var vm = new RepoTreeItemViewModel( tree )
        {
            // Act
            IsChecked = true
        };

        // Assert
        Assert.True( vm.Children[0].IsChecked );
        Assert.True( vm.Children[0].Children[0].IsChecked );
        Assert.True( vm.Children[1].IsChecked );
    }

    [Fact( DisplayName = "チェックされたファイルのみが列挙される" )]
    public void GetCheckedFiles_ReturnsOnlyCheckedFiles() {
        // Arrange
        RepoTree root = new()
        {
            Name = "root",
            AbsolutePath = "root",
            IsDirectory = true,
            Children = [
                new() {
                    Name = "dir",
                    AbsolutePath = "root/dir",
                    IsDirectory = true,
                    Children = [
                        new() { Name = "file1", AbsolutePath = "root/dir/file1", IsDirectory = false },
                        new() { Name = "file2", AbsolutePath = "root/dir/file2", IsDirectory = false }
                    ]
                },
                new() { Name = "file3", AbsolutePath = "root/file3", IsDirectory = false }
            ]
        };
        var vm = new RepoTreeItemViewModel( root );
        vm.Children[0].Children[1].IsChecked = true;

        // Act
        var files = vm.GetCheckedFiles().ToList();

        // Assert
        Assert.Single( files );
        Assert.Equal( "root/dir/file2", files[0].AbsolutePath );
    }
}
