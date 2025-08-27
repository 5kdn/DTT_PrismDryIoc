using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Core.Services;
using DcsTranslateTool.Win.Contracts.ViewModels;
using DcsTranslateTool.Win.Contracts.ViewModels.Factories;
using DcsTranslateTool.Win.ViewModels;
using DcsTranslateTool.Win.ViewModels.Factories;

using Xunit;

namespace DcsTranslateTool.Win.Tests.ViewModels.Factories;
public class FileEntryViewModelFactoryTests {
    #region Create_FileEntry

    [Fact]
    [Trait( "Category", "WindowsOnly" )]
    public void CreateはFileEntryを指定したときFileEntryViewModelを生成する() {
        // Arrange
        var container = new Container();
        container.Register<IFileEntryViewModelFactory, FileEntryViewModelFactory>( Reuse.Transient );
        container.Register<IFileEntryViewModel, FileEntryViewModel>( Reuse.Transient );
        container.Register<IFileEntryService, FileEntryService>( Reuse.Transient );

        var factory = container.Resolve<IFileEntryViewModelFactory>();
        var fileEntry = new FileEntry("test.txt", @"C:\Test\test.txt", false);

        // Act
        var viewModel = factory.Create(fileEntry);

        // Assert
        Assert.NotNull( viewModel );
        Assert.Equal( fileEntry, viewModel.Model );
    }

    #endregion

    #region Create_path

    [Fact]
    [Trait( "Category", "WindowsOnly" )]
    public void CreateはAbsolutePathとIsDirectoryを指定してFileEntryViewModelを生成する() {
        // Arrange
        var container = new Container();
        container.Register<IFileEntryViewModelFactory, FileEntryViewModelFactory>( Reuse.Transient );
        container.Register<IFileEntryViewModel, FileEntryViewModel>( Reuse.Transient );
        container.Register<IFileEntryService, FileEntryService>( Reuse.Transient );
        var factory = container.Resolve<IFileEntryViewModelFactory>();
        const string absolutePath = @"C:\Test\folder";
        const bool isDirectory = true;

        // Act
        var viewModel = factory.Create(absolutePath, isDirectory);

        // Assert
        var actualModel = viewModel.Model;
        Assert.NotNull( viewModel );
        Assert.Equal( "folder", viewModel.Name );
        Assert.Equal( absolutePath, viewModel.AbsolutePath );
        Assert.True( viewModel.IsDirectory );
        Assert.NotNull( actualModel );
        Assert.Equal( "folder", actualModel.Name );
        Assert.Equal( absolutePath, actualModel.AbsolutePath );
        Assert.True( actualModel.IsDirectory );
    }

    #endregion
}
