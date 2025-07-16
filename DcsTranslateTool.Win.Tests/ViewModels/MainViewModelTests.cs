using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Services;
using DcsTranslateTool.Share.Contracts.Services;
using DcsTranslateTool.Win.Contracts.Services;
using DcsTranslateTool.Win.Services;
using DcsTranslateTool.Win.ViewModels;

using Moq;

using Xunit;

namespace DcsTranslateTool.Win.Tests.ViewModels;

public class UploadViewModelTests {
    private readonly Container _container;

    public UploadViewModelTests() {
        _container = new Container();
        _container.Register<IRegionManager, RegionManager>( Reuse.Singleton );
        _container.Register<IAppSettingsService, AppSettingsService>( Reuse.Singleton );
        _container.Register<IFileService, FileService>( Reuse.Transient );
        _container.RegisterDelegate<IRepositoryService>( s => Mock.Of<IRepositoryService>() );
        // ViewModels
        _container.Register<UploadViewModel>( Reuse.Singleton );
    }

    [Fact( DisplayName = "UploadViewModelが正常に生成できる" )]
    public void TestUploadViewModelCreation() {
        // Arrange & Act
        var vm = _container.Resolve<UploadViewModel>();

        // Assert
        Assert.NotNull( vm );
    }
}
