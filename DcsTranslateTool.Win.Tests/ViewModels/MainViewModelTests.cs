using DcsTranslateTool.Win.ViewModels;

using Xunit;

namespace DcsTranslateTool.Tests.ViewModels;

public class MainViewModelTests {
    private readonly Container _container;

    public MainViewModelTests() {
        _container = new Container();
        _container.Register<IRegionManager, RegionManager>( Reuse.Singleton );
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
