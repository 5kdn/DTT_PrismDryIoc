using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Core.Services;
using DcsTranslateTool.Win.Constants;
using DcsTranslateTool.Win.Contracts.Services;
using DcsTranslateTool.Win.Contracts.ViewModels.Factories;
using DcsTranslateTool.Win.Enums;
using DcsTranslateTool.Win.Services;
using DcsTranslateTool.Win.ViewModels;
using DcsTranslateTool.Win.ViewModels.Factories;

using Moq;

using Xunit;


namespace DcsTranslateTool.Win.Tests.ViewModels;

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
        _container.Register<IFileEntryViewModelFactory, FileEntryViewModelFactory>( Reuse.Singleton );
    }

    [Fact( DisplayName = "UploadViewModelが正常に生成できる" )]
    public void TestUploadViewModelCreation() {
        // Arrange & Act
        var vm = _container.Resolve<UploadViewModel>();

        // Assert
        Assert.NotNull( vm );
    }

    [Fact]
    [Trait("Category", "WindowsOnly")]
    public void OpenCreatePullRequestDialogCommandはチェックされたFileEntryを渡す() {
        // Arrange
        var dialogServiceMock = new Mock<IDialogService>();
        var appSettingsServiceMock = new Mock<IAppSettingsService>();
        var regionManager = new RegionManager();
        var factoryMock = new Mock<IFileEntryViewModelFactory>();
        var fileEntryServiceMock = new Mock<IFileEntryService>();

        var vm = new UploadViewModel(
            appSettingsServiceMock.Object,
            regionManager,
            dialogServiceMock.Object,
            factoryMock.Object );

        var rootModel = new FileEntry("Root", "/Root", true);
        var childModel = new FileEntry("Child.txt", "/Root/Child.txt", false);
        var rootVm = new FileEntryViewModel(factoryMock.Object, fileEntryServiceMock.Object, rootModel);
        rootVm.Children.Clear();
        rootVm.Children.Add(new FileEntryViewModel(factoryMock.Object, fileEntryServiceMock.Object, childModel) { CheckState = CheckState.Checked });

        vm.Tabs.Add(new UploadTabItemViewModel(RootTabType.Aircraft, rootVm));
        vm.SelectedTabIndex = 0;

        Assert.True( vm.IsCreatePullRequestDialogButtonEnabled );

        IDialogParameters? captured = null;
        dialogServiceMock
            .Setup(d => d.ShowDialog(PageKeys.CreatePullRequestDialog, It.IsAny<IDialogParameters>(), It.IsAny<Action<IDialogResult>>()))
            .Callback<string, IDialogParameters, Action<IDialogResult>>((_, p, _) => captured = p);

        // Act
        vm.OpenCreatePullRequestDialogCommand.Execute();

        // Assert
        Assert.NotNull( captured );
        var files = captured!.GetValue<IEnumerable<FileEntry>>( "files" ).ToList();
        Assert.Single( files );
        Assert.Equal( childModel, files[0] );
        Assert.True( vm.IsCreatePullRequestDialogButtonEnabled );
    }

    [Fact]
    [Trait("Category", "WindowsOnly")]
    public void IsCreatePullRequestDialogButtonEnabledは選択状態で変化する() {
        // Arrange
        var appSettingsServiceMock = new Mock<IAppSettingsService>();
        var regionManager = new RegionManager();
        var dialogServiceMock = new Mock<IDialogService>();
        var factoryMock = new Mock<IFileEntryViewModelFactory>();
        var fileEntryServiceMock = new Mock<IFileEntryService>();

        var childModel = new FileEntry("Child", "/Root/Child", false);
        var rootModel = new FileEntry("Root", "/Root", true);
        var rootVm = new FileEntryViewModel(factoryMock.Object, fileEntryServiceMock.Object, rootModel);
        rootVm.Children.Clear();
        var childVm = new FileEntryViewModel(factoryMock.Object, fileEntryServiceMock.Object, childModel);
        rootVm.Children.Add( childVm );

        var vm = new UploadViewModel(
            appSettingsServiceMock.Object,
            regionManager,
            dialogServiceMock.Object,
            factoryMock.Object );

        vm.Tabs.Add(new UploadTabItemViewModel(RootTabType.Aircraft, rootVm));
        vm.SelectedTabIndex = 0;

        // Assert initial state
        Assert.False( vm.IsCreatePullRequestDialogButtonEnabled );

        // Act
        childVm.CheckState = CheckState.Checked;

        // Assert
        Assert.True( vm.IsCreatePullRequestDialogButtonEnabled );
    }
}
