using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Services;
using DcsTranslateTool.Share.Contracts.Services;
using DcsTranslateTool.Win.Contracts.Services;
using DcsTranslateTool.Win.Services;
using DcsTranslateTool.Win.ViewModels;

using Moq;

using Xunit;

namespace DcsTranslateTool.Win.Tests.ViewModels;

public class DownloadViewModelTests {
    private readonly Container _container;

    public DownloadViewModelTests() {
        _container = new Container();
        _container.Register<IRegionManager, RegionManager>( Reuse.Singleton );
        _container.Register<IAppSettingsService, AppSettingsService>( Reuse.Singleton );
        _container.Register<IFileService, FileService>( Reuse.Transient );
        _container.RegisterDelegate<IRepositoryService>( s => Mock.Of<IRepositoryService>() );
        // ViewModels
        _container.Register<DownloadViewModel>( Reuse.Singleton );
    }

    [Fact( DisplayName = "DownloadViewModelが正常に生成できる" )]
    public void TestDownloadViewModelCreation() {
        // Arrange & Act
        var vm = _container.Resolve<DownloadViewModel>();

        // Assert
        Assert.NotNull( vm );
    }
}
