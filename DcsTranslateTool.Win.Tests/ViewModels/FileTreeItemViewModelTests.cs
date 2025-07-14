using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.ViewModels;
using Xunit;

namespace DcsTranslateTool.Win.Tests.ViewModels;

public class FileTreeItemViewModelTests {
    [Fact(DisplayName = "拡張子がmizのファイルは強調対象になる" )]
    public void IsHighlightedFile_ReturnsTrueForMiz() {
        // Arrange
        var tree = new FileTree { Name = "test.miz", AbsolutePath = "path/test.miz", IsDirectory = false };
        var vm = new FileTreeItemViewModel( tree );

        // Act
        var result = vm.IsHighlightedFile;

        // Assert
        Assert.True( result );
    }

    [Fact(DisplayName = "拡張子がpdfのファイルは強調対象になる" )]
    public void IsHighlightedFile_ReturnsTrueForPdf() {
        // Arrange
        var tree = new FileTree { Name = "manual.pdf", AbsolutePath = "path/manual.pdf", IsDirectory = false };
        var vm = new FileTreeItemViewModel( tree );

        // Act
        var result = vm.IsHighlightedFile;

        // Assert
        Assert.True( result );
    }

    [Fact(DisplayName = "その他のファイルは強調対象にならない" )]
    public void IsHighlightedFile_ReturnsFalseForOther() {
        // Arrange
        var tree = new FileTree { Name = "readme.txt", AbsolutePath = "path/readme.txt", IsDirectory = false };
        var vm = new FileTreeItemViewModel( tree );

        // Act
        var result = vm.IsHighlightedFile;

        // Assert
        Assert.False( result );
    }
}
