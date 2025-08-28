using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Core.Services;
using DcsTranslateTool.Win.Contracts.Services;
using DcsTranslateTool.Win.Contracts.ViewModels;
using DcsTranslateTool.Win.Contracts.ViewModels.Factories;
using DcsTranslateTool.Win.ViewModels;
using DcsTranslateTool.Win.ViewModels.Factories;

using DryIoc;
using Moq;

using Xunit;

namespace DcsTranslateTool.Win.Tests.ViewModels.Factories;

public class FileEntryViewModelFactoryTests : IDisposable {
    private readonly string _tempDir;

    public FileEntryViewModelFactoryTests() {
        _tempDir = Path.Join( Path.GetTempPath(), Guid.NewGuid().ToString() );
        Directory.CreateDirectory( _tempDir );
    }

    public void Dispose() {
        if(Directory.Exists( _tempDir )) Directory.Delete( _tempDir, true );
        GC.SuppressFinalize( this );
    }

    #region Create_FileEntry

    [Fact]
    [Trait( "Category", "WindowsOnly" )]
    public void CreateはEntryを指定したときFileEntryViewModelを生成する() {
        // Arrange
        var container = new Container();
        container.Register<IFileEntryViewModelFactory, FileEntryViewModelFactory>( Reuse.Transient );
        container.Register<IFileEntryViewModel, FileEntryViewModel>( Reuse.Transient );
        container.Register<IFileEntryService, FileEntryService>( Reuse.Transient );

        var factory = container.Resolve<IFileEntryViewModelFactory>();
        var entry = new Entry( "test.txt", "test.txt", false );

        // Act
        var viewModel = factory.Create( entry );

        // Assert
        Assert.NotNull( viewModel );
        Assert.Equal( entry, viewModel.Model );
    }

    #endregion

    #region Create_path

    [Fact]
    [Trait( "Category", "WindowsOnly" )]
    public void CreateはPathとIsDirectoryを指定してEntryViewModelを生成する() {
        // Arrange
        var container = new Container();
        container.Register<IFileEntryViewModelFactory, FileEntryViewModelFactory>( Reuse.Transient );
        container.Register<IFileEntryViewModel, FileEntryViewModel>( Reuse.Transient );
        container.Register<IFileEntryService, FileEntryService>( Reuse.Transient );
        var appSettingsMock = new Mock<IAppSettingsService>();
        appSettingsMock.SetupGet( a => a.TranslateFileDir ).Returns( _tempDir );
        container.RegisterInstance<IAppSettingsService>( appSettingsMock.Object );
        var factory = container.Resolve<IFileEntryViewModelFactory>();
        string path = Path.Combine( _tempDir, "folder" );
        const bool isDirectory = true;

        // Act
        var viewModel = factory.Create( path, isDirectory );

        // Assert
        var actualModel = viewModel.Model;
        Assert.NotNull( viewModel );
        Assert.Equal( "folder", viewModel.Name );
        Assert.Equal( "folder", viewModel.Path );
        Assert.True( viewModel.IsDirectory );
        Assert.NotNull( actualModel );
        Assert.Equal( "folder", actualModel.Name );
        Assert.Equal( "folder", actualModel.Path );
        Assert.True( actualModel.IsDirectory );
    }

    #endregion
}