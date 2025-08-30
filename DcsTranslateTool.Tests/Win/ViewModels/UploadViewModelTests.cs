using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Services;
using DcsTranslateTool.Win.Contracts.Services;
using DcsTranslateTool.Win.Services;
using DcsTranslateTool.Win.ViewModels;

using Moq;

using Xunit;


namespace DcsTranslateTool.Tests.Win.ViewModels;

public class UploadViewModelTests {
    private readonly Container _container;

    public UploadViewModelTests() {

        var mockedRepositoryService = new Mock<IRepositoryService>();
        _container = new Container();
        _container.Register<IAppSettingsService, AppSettingsService>( Reuse.Singleton );
        _container.Register<IRegionManager, RegionManager>( Reuse.Singleton );
        _container.RegisterInstance<IDialogService>( Mock.Of<IDialogService>() );
        _container.RegisterInstance<IRepositoryService>( mockedRepositoryService.Object );
        _container.Register<IFileEntryService, FileEntryService>();
        _container.Register<IFileService, FileService>( Reuse.Transient );
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