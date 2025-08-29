using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Services;
using DcsTranslateTool.Win.Contracts.Services;
using DcsTranslateTool.Win.Contracts.ViewModels.Factories;
using DcsTranslateTool.Win.Services;
using DcsTranslateTool.Win.ViewModels;
using DcsTranslateTool.Win.ViewModels.Factories;

using Moq;

using Xunit;


namespace DcsTranslateTool.Win.Tests.ViewModels;

/// <summary>
/// <see cref="UploadViewModel"/> のテストである。
/// </summary>
public class UploadViewModelTests {
    private readonly Container _container;

    public UploadViewModelTests() {
        _container = new Container();
        _container.Register<IAppSettingsService, AppSettingsService>( Reuse.Singleton );
        _container.Register<IRegionManager, RegionManager>( Reuse.Singleton );
        _container.RegisterInstance<IDialogService>( Mock.Of<IDialogService>() );
        _container.Register<IFileService, FileService>( Reuse.Transient );
        // ViewModels
        _container.Register<UploadViewModel>( Reuse.Singleton );
        _container.Register<IEntryViewModelFactory, EntryViewModelFactory>( Reuse.Singleton );
    }

    [Fact( DisplayName = "UploadViewModelが正常に生成できる" )]
    /// <summary>
    /// UploadViewModelが正常に生成できる。
    /// </summary>
    public void TestUploadViewModelCreation() {
        // Arrange & Act
        var vm = _container.Resolve<UploadViewModel>();

        // Assert
        Assert.NotNull( vm );
    }
}