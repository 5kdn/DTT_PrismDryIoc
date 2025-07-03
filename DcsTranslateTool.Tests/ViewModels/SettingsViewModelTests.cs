using System.Reflection;
using System.IO;

using Microsoft.Extensions.Configuration;

using Moq;

using DcsTranslateTool.Contracts.Services;
using DcsTranslateTool.Contracts.Providers;
using DcsTranslateTool.Models;
using DcsTranslateTool.Services;
using DcsTranslateTool.ViewModels;
using DcsTranslateTool.Providers;
using DryIoc;

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
        _container.Register<IAppSettingsService, AppSettingsService>( Reuse.Singleton );

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
        var mockEnvironmentProvider = new Mock<IEnvironmentProvider>();
        mockEnvironmentProvider
            .Setup( p => p.GetUserProfilePath() )
            .Returns( "Path/To/UserProfile" );
        var appSettingsService = new AppSettingsService();
        var settingsVm = new SettingsViewModel(
            mockAppConfig.Object,
            mockThemeSelectorService.Object,
            mockSystemService.Object,
            mockApplicationInfoService.Object,
            mockDialogProvider.Object,
            mockEnvironmentProvider.Object,
            appSettingsService
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
        var mockEnvironmentProvider = new Mock<IEnvironmentProvider>();
        mockEnvironmentProvider
            .Setup( p => p.GetUserProfilePath() )
            .Returns( "Path/To/UserProfile" );
        var appSettingsService = new AppSettingsService();

        var settingsVm = new SettingsViewModel(
            mockAppConfig.Object,
            mockThemeSelectorService.Object,
            mockSystemService.Object,
            mockApplicationInfoService.Object,
            mockDialogProvider.Object,
            mockEnvironmentProvider.Object,
            appSettingsService
        );
        settingsVm.OnNavigatedTo( null );

        // Act
        var actual = settingsVm.VersionDescription;

        // Assert
        Assert.Equal( expected, actual );
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
        var mockEnvironmentProvider = new Mock<IEnvironmentProvider>();
        var appSettingsService = new AppSettingsService();

        var settingsVm = new SettingsViewModel(
            mockAppConfig.Object,
            mockThemeSelectorService.Object,
            mockSystemService.Object,
            mockApplicationInfoService.Object,
            mockDialogProvider.Object,
            mockEnvironmentProvider.Object,
            appSettingsService
        );

        // Act
        settingsVm.SetThemeCommand.Execute( expected.ToString() );

        // Assert
        mockThemeSelectorService.Verify( mock => mock.SetTheme( expected ) );
    }

    [Fact( DisplayName = "プロパティ変更で設定に保存される" )]
    public void PropertyChange_ShouldSaveSettings()
    {
        // Arrange
        Properties.Settings.Default.Reset();
        var mockThemeSelectorService = new Mock<IThemeSelectorService>();
        var mockAppConfig = new Mock<AppConfig>();
        var mockSystemService = new Mock<ISystemService>();
        var mockApplicationInfoService = new Mock<IApplicationInfoService>();
        var mockDialogProvider = new Mock<IDialogProvider>();
        var mockEnvironmentProvider = new Mock<IEnvironmentProvider>();
        var appSettingsService = new AppSettingsService();

        var vm = new SettingsViewModel(
            mockAppConfig.Object,
            mockThemeSelectorService.Object,
            mockSystemService.Object,
            mockApplicationInfoService.Object,
            mockDialogProvider.Object,
            mockEnvironmentProvider.Object,
            appSettingsService
        );

        // Act
        vm.SourceAircraftDir = "A";
        vm.SourceDlcCampaignDir = "B";
        vm.SourceUserDir = "C";
        vm.TranslateFileDir = "D";

        // Assert
        Assert.Equal( "A", Properties.Settings.Default.SourceAircraftDir );
        Assert.Equal( "B", Properties.Settings.Default.SourceDlcCampaignDir );
        Assert.Equal( "C", Properties.Settings.Default.SourceUserDir );
        Assert.Equal( "D", Properties.Settings.Default.TranslateFileDir );

        Properties.Settings.Default.Reset();
    }

    [Fact( DisplayName = "リセットコマンドで初期値に戻る" )]
    public void ResetCommand_ShouldRestoreDefaults()
    {
        // Arrange
        Properties.Settings.Default.Reset();
        var mockThemeSelectorService = new Mock<IThemeSelectorService>();
        var mockAppConfig = new Mock<AppConfig>();
        var mockSystemService = new Mock<ISystemService>();
        var mockApplicationInfoService = new Mock<IApplicationInfoService>();
        var mockDialogProvider = new Mock<IDialogProvider>();
        var mockEnvironmentProvider = new Mock<IEnvironmentProvider>();
        mockEnvironmentProvider
            .Setup( p => p.GetUserProfilePath() )
            .Returns( "NonExist" );
        var appSettingsService = new AppSettingsService();

        var vm = new SettingsViewModel(
            mockAppConfig.Object,
            mockThemeSelectorService.Object,
            mockSystemService.Object,
            mockApplicationInfoService.Object,
            mockDialogProvider.Object,
            mockEnvironmentProvider.Object,
            appSettingsService
        );

        vm.SourceAircraftDir = "A";
        vm.SourceDlcCampaignDir = "B";
        vm.SourceUserDir = "C";
        vm.TranslateFileDir = "D";

        // Act
        vm.ResetSettingsCommand.Execute( null );

        // Assert
        Assert.Equal( string.Empty, vm.SourceAircraftDir );
        Assert.Equal( string.Empty, vm.SourceDlcCampaignDir );
        Assert.Equal( string.Empty, vm.SourceUserDir );
        var exeDir = Path.GetDirectoryName( Assembly.GetEntryAssembly()?.Location );
        var expected = Path.Combine( exeDir!, "TranslateFiles" );
        Assert.Equal( expected, vm.TranslateFileDir );

        Properties.Settings.Default.Reset();
    }
}
