using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Services;
using DcsTranslateTool.Win.Contracts.Services;
using DcsTranslateTool.Win.Services;
using DcsTranslateTool.Win.ViewModels;

using MaterialDesignThemes.Wpf;

using Moq;

using Xunit;

namespace DcsTranslateTool.Tests.Win.ViewModels;

public class MainViewModelTests {
    private readonly Container _container;

    public MainViewModelTests() {
        _container = new Container();
        _container.Register<IRegionManager, RegionManager>( Reuse.Singleton );
        _container.Register<IAppSettingsService, AppSettingsService>( Reuse.Singleton );
        _container.Register<IFileService, FileService>( Reuse.Transient );
        _container.RegisterDelegate( () => Mock.Of<IRepositoryService>() );
        _container.RegisterInstance<ISnackbarMessageQueue>( Mock.Of<ISnackbarMessageQueue>() );

        // ViewModels
        _container.Register<MainViewModel>( Reuse.Singleton );
    }

    [Fact( DisplayName = "MainViewModelが正常に生成できる" )]
    public void TestMainViewModelCreation() {
        // Arrange & Act
        var vm = _container.Resolve<MainViewModel>();

        // Assert
        Assert.NotNull( vm );
    }
}