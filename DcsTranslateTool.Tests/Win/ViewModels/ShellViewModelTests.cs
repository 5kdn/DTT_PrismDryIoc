using DcsTranslateTool.Win.Contracts.Services;
using DcsTranslateTool.Win.ViewModels;

using Moq;

using Xunit;

namespace DcsTranslateTool.Tests.Win.ViewModels;

public class ShellViewModelTests {
    private readonly Container _container;

    public ShellViewModelTests() {
        _container = new Container();
        _container.Register<IRegionManager, RegionManager>( Reuse.Singleton );

        // ViewModels
        _container.Register<ShellViewModel>( Reuse.Transient );
        _container.RegisterInstance<ISnackbarService>( Mock.Of<ISnackbarService>() );
    }

    [Fact( DisplayName = "ShellViewModelが正常に生成できる" )]
    public void TestShellViewModelCreation() {
        // Arrange & Act
        var vm = _container.Resolve<ShellViewModel>();

        // Assert
        Assert.NotNull( vm );
    }
}