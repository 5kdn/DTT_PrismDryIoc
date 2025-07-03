using System.Reflection;

using Microsoft.Extensions.Configuration;

using Moq;
using DryIoc;

using DcsTranslateTool.Contracts.Providers;
using DcsTranslateTool.Models;
using DcsTranslateTool.Providers;
using DcsTranslateTool.ViewModels;

using Xunit;

namespace DcsTranslateTool.Tests.ViewModels;

public class SettingsViewModelTests
{
    private readonly Container _container;

    public SettingsViewModelTests()
    {
        _container = new Container();

        // App Services
        _container.Register<IThemeSelectorService, ThemeSelectorService>( Reuse.Singleton );
        _container.Register<ISystemService, SystemService>( Reuse.Singleton );
        _container.Register<IApplicationInfoService, ApplicationInfoService>( Reuse.Singleton );
        _container.Register<IDialogProvider, DialogProvider>( Reuse.Singleton );
        _container.Register<IEnvironmentProvider, EnvironmentProvider>( Reuse.Singleton );

        // ViewModels
        _container.Register<SettingsViewModel>( Reuse.Transient );

        // Configuration
        var appLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
        var configuration = new ConfigurationBuilder()
            .SetBasePath(appLocation)
            .AddJsonFile("appsettings.json")
            .Build();
        var appConfig = configuration.GetSection(nameof(AppConfig)).Get<AppConfig>();

        // Register configurations to IoC
        _container.RegisterInstance( configuration );
        _container.RegisterInstance( appConfig );
    }

    [Fact( DisplayName = "SettingsViewModelが正常に生成できる" )]
    public void TestSettingsViewModelCreation()
    {
        // Arrange & Act
        var vm = _container.Resolve<SettingsViewModel>();

        // Assert
        Assert.NotNull( vm );
    }

    [Theory( DisplayName = "テーマの取得が正しく動作する" )]
    [InlineData( AppTheme.Default )]
    [InlineData( AppTheme.Light )]
    [InlineData( AppTheme.Dark )]
    public void TestSettingsViewModel_SetCurrentTheme( AppTheme expected )
    {
        // Arrange
        var mockThemeSelectorService = new Mock<IThemeSelectorService>();
        mockThemeSelectorService
            .Setup( mock => mock.GetCurrentTheme() )
            .Returns( expected );
        var mockAppConfig = new Mock<AppConfig>();
        var mockSystemService = new Mock<ISystemService>();
        var mockApplicationInfoService = new Mock<IApplicationInfoService>();

        var mockDialogProvider = new Mock<IDialogProvider>();
        var mockEnvProvider = new Mock<IEnvironmentProvider>();
        mockEnvProvider.Setup( m => m.GetUserProfile() ).Returns( string.Empty );

        var settingsVm = new SettingsViewModel(
            mockAppConfig.Object,
            mockThemeSelectorService.Object,
            mockSystemService.Object,
            mockApplicationInfoService.Object,
            mockDialogProvider.Object,
            mockEnvProvider.Object
        );
        settingsVm.OnNavigatedTo( null );

        // Act
        var actual = settingsVm.Theme;

        // Assert
        Assert.Equal( expected, actual );
    }

    [Fact( DisplayName = "バージョン情報が正しく取得できる" )]
    public void TestSettingsViewModel_SetCurrentVersion()
    {
        // Arrange
        var testVersion = new Version(1, 2, 3, 4);
        var expected = $"DcsTranslateTool - {testVersion}";

        var mockThemeSelectorService = new Mock<IThemeSelectorService>();
        var mockAppConfig = new Mock<AppConfig>();
        var mockSystemService = new Mock<ISystemService>();
        var mockApplicationInfoService = new Mock<IApplicationInfoService>();
        mockApplicationInfoService
            .Setup( mock => mock.GetVersion() )
            .Returns( testVersion );

        var mockDialogProvider = new Mock<IDialogProvider>();
        var mockEnvProvider = new Mock<IEnvironmentProvider>();
        mockEnvProvider.Setup( m => m.GetUserProfile() ).Returns( string.Empty );

        var settingsVm = new SettingsViewModel(
            mockAppConfig.Object,
            mockThemeSelectorService.Object,
            mockSystemService.Object,
            mockApplicationInfoService.Object,
            mockDialogProvider.Object,
            mockEnvProvider.Object
        );
        settingsVm.OnNavigatedTo( null );

        // Act
        var version = settingsVm.VersionDescription;

        // Assert
        Assert.Equal( expected, version );
    }

    [Fact( DisplayName = "テーマ変更コマンドが正しく動作する" )]
    public void TestSettingsViewModel_SetThemeCommand()
    {
        // Arrange
        var expected = AppTheme.Light;

        var mockThemeSelectorService = new Mock<IThemeSelectorService>();
        var mockAppConfig = new Mock<AppConfig>();
        var mockSystemService = new Mock<ISystemService>();
        var mockApplicationInfoService = new Mock<IApplicationInfoService>();

        var mockDialogProvider = new Mock<IDialogProvider>();
        var mockEnvProvider = new Mock<IEnvironmentProvider>();
        mockEnvProvider.Setup( m => m.GetUserProfile() ).Returns( string.Empty );

        var settingsVm = new SettingsViewModel(
            mockAppConfig.Object,
            mockThemeSelectorService.Object,
            mockSystemService.Object,
            mockApplicationInfoService.Object,
            mockDialogProvider.Object,
            mockEnvProvider.Object
        );

        // Act
        settingsVm.SetThemeCommand.Execute( expected.ToString() );

        // Assert
        mockThemeSelectorService.Verify( mock => mock.SetTheme( expected ) );
    }
}
