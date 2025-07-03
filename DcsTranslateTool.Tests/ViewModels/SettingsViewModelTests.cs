using System.Reflection;

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
        var config = _container.Resolve<AppConfig>();

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
        var settingsVm = new SettingsViewModel(
            mockAppConfig.Object,
            mockThemeSelectorService.Object,
            mockSystemService.Object,
            mockApplicationInfoService.Object,
            mockDialogProvider.Object,
            mockEnvironmentProvider.Object
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

        var settingsVm = new SettingsViewModel(
            mockAppConfig.Object,
            mockThemeSelectorService.Object,
            mockSystemService.Object,
            mockApplicationInfoService.Object,
            mockDialogProvider.Object,
            mockEnvironmentProvider.Object
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

        var settingsVm = new SettingsViewModel(
            mockAppConfig.Object,
            mockThemeSelectorService.Object,
            mockSystemService.Object,
            mockApplicationInfoService.Object,
            mockDialogProvider.Object,
            mockEnvironmentProvider.Object
        );

        // Act
        settingsVm.SetThemeCommand.Execute( expected.ToString() );

        // Assert
        mockThemeSelectorService.Verify( mock => mock.SetTheme( expected ) );
    }

    [Fact( DisplayName = "ディレクトリ設定変更時にプロパティへ保存される" )]
    public void TestSettingsViewModel_PersistProperties()
    {
        // Arrange
        var vm = _container.Resolve<SettingsViewModel>();
        var config = _container.Resolve<AppConfig>();
        
        // Act
        vm.SourceAircraftDir = "A";
        vm.SourceDlcCampaignDir = "B";
        vm.SourceUserDir = "C";
        vm.TranslateFileDir = "D";

        // Assert
        Assert.Equal( "A", config.SourceAircraftDir );
        Assert.Equal( "B", config.SourceDlcCampaignDir );
        Assert.Equal( "C", config.SourceUserDir );
        Assert.Equal( "D", config.TranslateFileDir );
    }

    [Fact( DisplayName = "ナビゲーション時に保存済みプロパティを読み込む" )]
    public void TestSettingsViewModel_RestoreProperties()
    {
        // Arrange
        var config2 = _container.Resolve<AppConfig>();
        config2.SourceAircraftDir = "A";
        config2.SourceDlcCampaignDir = "B";
        config2.SourceUserDir = "C";
        config2.TranslateFileDir = "D";

        var vm = _container.Resolve<SettingsViewModel>();

        // Act
        vm.OnNavigatedTo( null );

        // Assert
        Assert.Equal( "A", vm.SourceAircraftDir );
        Assert.Equal( "B", vm.SourceDlcCampaignDir );
        Assert.Equal( "C", vm.SourceUserDir );
        Assert.Equal( "D", vm.TranslateFileDir );
    }

    [Fact( DisplayName = "リセットコマンドで設定が初期化される" )]
    public void TestSettingsViewModel_ResetCommand()
    {
        // Arrange
        var vm = _container.Resolve<SettingsViewModel>();
        var config3 = _container.Resolve<AppConfig>();
        vm.OnNavigatedTo( null );
        config3.SourceAircraftDir = "tmp";
        config3.SourceDlcCampaignDir = "tmp";
        config3.SourceUserDir = "tmp";
        config3.TranslateFileDir = "tmp";

        // Act
        vm.ResetSettingsCommand.Execute( null );

        // Assert
        Assert.Equal( string.Empty, vm.SourceAircraftDir );
        Assert.Equal( string.Empty, vm.SourceDlcCampaignDir );
        Assert.NotEqual( "tmp", vm.SourceUserDir );
        Assert.NotEqual( "tmp", vm.TranslateFileDir );
    }
}
