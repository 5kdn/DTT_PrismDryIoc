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

/// <summary>
/// <see cref="FileEntryViewModelFactory"/> のテストである。
/// </summary>
public class FileEntryViewModelFactoryTests : IDisposable {
    private readonly string _tempDir;

    /// <summary>
    /// コンストラクターで一時ディレクトリを作成する。
    /// </summary>
    public FileEntryViewModelFactoryTests() {
        _tempDir = Path.Join( Path.GetTempPath(), Guid.NewGuid().ToString() );
        Directory.CreateDirectory( _tempDir );
    }

    /// <summary>
    /// 一時ディレクトリを削除する。
    /// </summary>
    public void Dispose() {
        if(Directory.Exists( _tempDir )) Directory.Delete( _tempDir, true );
        GC.SuppressFinalize( this );
    }

    #region Create_FileEntry

    /// <summary>
    /// Entryを指定したときファイルエントリービューモデルを生成する。
    /// </summary>
    [Fact]
    [Trait( "Category", "WindowsOnly" )]
    public void CreateはEntryを指定したときFileEntryViewModelを生成する() {
        // Arrange
        var container = new Container();
        container.Register<IFileEntryViewModelFactory, FileEntryViewModelFactory>( Reuse.Transient );
        container.Register<IFileEntryViewModel, FileEntryViewModel>( Reuse.Transient );
        var appSettingsMock = new Mock<IAppSettingsService>();
        appSettingsMock.SetupGet( a => a.TranslateFileDir ).Returns( _tempDir );
        container.RegisterInstance<IAppSettingsService>( appSettingsMock.Object );
        container.RegisterDelegate<IFileEntryService>(
            c => new FileEntryService( c.Resolve<IAppSettingsService>().TranslateFileDir ),
            Reuse.Transient );

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

    /// <summary>
    /// Pathとディレクトリ指定からファイルエントリービューモデルを生成する。
    /// </summary>
    [Fact]
    [Trait( "Category", "WindowsOnly" )]
    public void CreateはPathとIsDirectoryを指定してEntryViewModelを生成する() {
        // Arrange
        var container = new Container();
        container.Register<IFileEntryViewModelFactory, FileEntryViewModelFactory>( Reuse.Transient );
        container.Register<IFileEntryViewModel, FileEntryViewModel>( Reuse.Transient );
        var appSettingsMock = new Mock<IAppSettingsService>();
        appSettingsMock.SetupGet( a => a.TranslateFileDir ).Returns( _tempDir );
        container.RegisterInstance<IAppSettingsService>( appSettingsMock.Object );
        container.RegisterDelegate<IFileEntryService>(
            c => new FileEntryService( c.Resolve<IAppSettingsService>().TranslateFileDir ),
            Reuse.Transient );
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