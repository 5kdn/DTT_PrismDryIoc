using System.Linq;
using System.Collections.Generic;
using DcsTranslateTool.Share.Models;
using DcsTranslateTool.Win.ViewModels;
using Xunit;

namespace DcsTranslateTool.Win.Tests.ViewModels;

public class RepoTreeItemViewModelTests {
    [Fact(DisplayName = "ディレクトリのチェックが子要素へ伝搬する" )]
    public void DirectoryCheck_PropagatesToChildren() {
        // Arrange
        var root = new RepoTree {
            Name = "root",
            AbsolutePath = "root",
            IsDirectory = true,
            Children = new List<RepoTree> {
                new RepoTree {
                    Name = "dir",
                    AbsolutePath = "root/dir",
                    IsDirectory = true,
                    Children = new List<RepoTree> {
                        new RepoTree { Name = "file", AbsolutePath = "root/dir/file", IsDirectory = false }
                    }
                }
            }
        };
        var vm = new RepoTreeItemViewModel( root );

        // Act
        vm.IsChecked = true;

        // Assert
        Assert.True( vm.Children[0].IsChecked );
        Assert.True( vm.Children[0].Children[0].IsChecked );
    }

    [Fact(DisplayName = "チェックされたファイルのみが列挙される" )]
    public void GetCheckedFiles_ReturnsOnlyCheckedFiles() {
        // Arrange
        var root = new RepoTree {
            Name = "root",
            AbsolutePath = "root",
            IsDirectory = true,
            Children = new List<RepoTree> {
                new RepoTree {
                    Name = "dir",
                    AbsolutePath = "root/dir",
                    IsDirectory = true,
                    Children = new List<RepoTree> {
                        new RepoTree { Name = "file1", AbsolutePath = "root/dir/file1", IsDirectory = false },
                        new RepoTree { Name = "file2", AbsolutePath = "root/dir/file2", IsDirectory = false }
                    }
                }
            }
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
