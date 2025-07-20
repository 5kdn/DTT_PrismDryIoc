using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.ViewModels;

using Xunit;

namespace DcsTranslateTool.Win.Tests.ViewModels;

public class FileTreeItemViewModelTests {
    [Fact( DisplayName = "ディレクトリのチェックが子要素へ伝搬する" )]
    public void DirectoryCheck_PropagatesToChildren() {
        // Arrange
        var root = new FileTree
        {
            Name = "root",
            AbsolutePath = "root",
            IsDirectory = true,
            Children = new () {
                new () {
                    Name = "dir",
                    AbsolutePath = "root/dir",
                    IsDirectory = true,
                    Children = new () {
                        new (){ Name = "file", AbsolutePath = "root/dir/file", IsDirectory = false }
                    }
                }
            }
        };
        var vm = new FileTreeItemViewModel(root);

        // Act
        vm.IsChecked = true;

        // Assert
        Assert.True( vm.Children[0].IsChecked );
        Assert.True( vm.Children[0].Children[0].IsChecked );
    }

    [Fact( DisplayName = "チェックされたファイルのみが列挙される" )]
    public void GetCheckedFiles_ReturnsOnlyCheckedFiles() {
        // Arrange
        var root = new FileTree
        {
            Name = "root",
            AbsolutePath = "root",
            IsDirectory = true,
            Children = new List<FileTree> {
                new FileTree {
                    Name = "dir",
                    AbsolutePath = "root/dir",
                    IsDirectory = true,
                    Children = new List<FileTree> {
                        new FileTree { Name = "file1", AbsolutePath = "root/dir/file1", IsDirectory = false },
                        new FileTree { Name = "file2", AbsolutePath = "root/dir/file2", IsDirectory = false }
                    }
                }
            }
        };
        var vm = new FileTreeItemViewModel(root);
        vm.Children[0].Children[1].IsChecked = true;

        // Act
        var files = vm.GetCheckedFiles().ToList();

        // Assert
        Assert.Single( files );
        Assert.Equal( "root/dir/file2", files[0].AbsolutePath );
    }
}