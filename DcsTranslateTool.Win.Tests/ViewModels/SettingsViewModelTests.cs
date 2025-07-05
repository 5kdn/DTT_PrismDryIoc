using DcsTranslateTool.Win.Contracts.Providers;
using DcsTranslateTool.Win.Contracts.Services;
using DcsTranslateTool.Win.Models;
using DcsTranslateTool.Win.Providers;
using DcsTranslateTool.Win.Services;
using DcsTranslateTool.Win.ViewModels;

using Moq;

using Xunit;

namespace DcsTranslateTool.Tests.ViewModels;

public class SettingsViewModelTests {
    [Fact( DisplayName = "SettingsViewModelが正常に生成できる" )]
    public void TestSettingsViewModelCreation() {
        // Arrange & Act
        var vm = new SettingsViewModel(
            Mock.Of<AppConfig>(),
            Mock.Of<IThemeSelectorService>(),
            Mock.Of<ISystemService>(),
            Mock.Of<IApplicationInfoService>(),
            Mock.Of<IDialogProvider>(),
            Mock.Of<IEnvironmentProvider>(),
            Mock.Of<AppSettingsService>()
        );

        // Assert
        Assert.NotNull( vm );
    }

    [Theory( DisplayName = "テーマの取得が正しく動作する" )]
    [InlineData( AppTheme.Default )]
    [InlineData( AppTheme.Light )]
    [InlineData( AppTheme.Dark )]
    public void TestSettingsViewModel_SetCurrentTheme( AppTheme expected ) {
        // Arrange
        var mockThemeSelectorService = new Mock<IThemeSelectorService>();
        mockThemeSelectorService
            .Setup( mock => mock.GetCurrentTheme() )
            .Returns( expected );
        var mockEnvironmentProvider = new Mock<IEnvironmentProvider>();
        mockEnvironmentProvider
            .Setup( p => p.GetUserProfilePath() )
            .Returns( "Path/To/UserProfile" );
        var settingsVm = new SettingsViewModel(
            new AppConfig(),
            mockThemeSelectorService.Object,
            Mock.Of<SystemService>(),
            Mock.Of<ApplicationInfoService>(),
            Mock.Of<DialogProvider>(),
            mockEnvironmentProvider.Object,
            Mock.Of<AppSettingsService>()
        );
        settingsVm.OnNavigatedTo( null );

        // Act
        var actual = settingsVm.Theme;

        // Assert
        Assert.Equal( expected, actual );
    }

    [Fact( DisplayName = "バージョン情報が正しく取得できる" )]
    public void TestSettingsViewModel_SetCurrentVersion() {
        // Arrange
        var testVersion = new Version(1, 2, 3, 4);
        var expected = $"DcsTranslateTool - {testVersion}";

        var mockThemeSelectorService = new Mock<IThemeSelectorService>();
        var mockApplicationInfoService = new Mock<IApplicationInfoService>();
        mockApplicationInfoService.Setup( mock => mock.GetVersion() ).Returns( testVersion );
        var mockEnvironmentProvider = new Mock<IEnvironmentProvider>();
        mockEnvironmentProvider.Setup( p => p.GetUserProfilePath() ).Returns( "Path/To/UserProfile" );

        var settingsVm = new SettingsViewModel(
            new AppConfig(),
            mockThemeSelectorService.Object,
            Mock.Of<SystemService>(),
            mockApplicationInfoService.Object,
            Mock.Of<DialogProvider>(),
            mockEnvironmentProvider.Object,
            Mock.Of<AppSettingsService>()
        );
        settingsVm.OnNavigatedTo( null );

        // Act
        var actual = settingsVm.VersionDescription;

        // Assert
        Assert.Equal( expected, actual );
    }

    [Fact( DisplayName = "テーマ変更コマンドが正しく動作する" )]
    public void TestSettingsViewModel_SetThemeCommand() {
        // Arrange
        var expected = AppTheme.Light;
        var mockThemeSelectorService = new Mock<IThemeSelectorService>();
        var settingsVm = new SettingsViewModel(
            new AppConfig(),
            mockThemeSelectorService.Object,
            Mock.Of<SystemService>(),
            Mock.Of<ApplicationInfoService>(),
            Mock.Of<DialogProvider>(),
            Mock.Of<EnvironmentProvider>(),
            new AppSettingsService()
        );

        // Act
        settingsVm.SetThemeCommand.Execute( expected.ToString() );

        // Assert
        mockThemeSelectorService.Verify( mock => mock.SetTheme( expected ) );
    }

    [Fact( DisplayName = "プロパティの初期値が正しく設定される" )]
    public void PropertyInitialValues_ShouldBeSetCorrectly() {
        // Arrange
        string expectedAircraftDir = "Path/To/SourceAirCraft";
        string expectedDlcCampaignDir = "Path/To/SourceDlcCampaign";
        string expectedUserDir = "Path/To/SourceUser";
        string expectedTranslateFileDir = "Path/To/TranslateFile";

        var mockAppSettingsService = new Mock<IAppSettingsService>();
        mockAppSettingsService.SetupGet( mock => mock.SourceAircraftDir ).Returns( expectedAircraftDir );
        mockAppSettingsService.SetupGet( mock => mock.SourceDlcCampaignDir ).Returns( expectedDlcCampaignDir );
        mockAppSettingsService.SetupGet( mock => mock.SourceUserDir ).Returns( expectedUserDir );
        mockAppSettingsService.SetupGet( mock => mock.TranslateFileDir ).Returns( expectedTranslateFileDir );

        var mockThemeSelectorService = new Mock<IThemeSelectorService>();
        var mockApplicationInfoService = new Mock<IApplicationInfoService>();
        mockThemeSelectorService.Setup( s => s.GetCurrentTheme() ).Returns( AppTheme.Default );
        mockApplicationInfoService.Setup( s => s.GetVersion() ).Returns( new Version( 1, 0, 0, 0 ) );
        var vm = new SettingsViewModel(
            new AppConfig(),
            mockThemeSelectorService.Object,
            Mock.Of<SystemService>(),
            mockApplicationInfoService.Object,
            Mock.Of<DialogProvider>(),
            Mock.Of<EnvironmentProvider>(),
            mockAppSettingsService.Object
        );

        // Act
        vm.OnNavigatedTo( null );

        var actualAircraftDir = vm.SourceAircraftDir;
        var actualDlcCampaignDir = vm.SourceDlcCampaignDir;
        var actualUserDir = vm.SourceUserDir;
        var actualTranslateFileDir = vm.TranslateFileDir;

        // Assert
        Assert.Equal( expectedAircraftDir, actualAircraftDir );
        Assert.Equal( expectedDlcCampaignDir, actualDlcCampaignDir );
        Assert.Equal( expectedUserDir, actualUserDir );
        Assert.Equal( expectedTranslateFileDir, actualTranslateFileDir );
    }

    [Fact( DisplayName = "プロパティ変更で設定に保存される" )]
    public void PropertyChange_ShouldSaveSettings() {
        // Arrange
        var mockAppSettingsService = new Mock<IAppSettingsService>();
        mockAppSettingsService.CallBase = true;
        var mockThemeSelectorService = new Mock<IThemeSelectorService>();
        mockThemeSelectorService.Setup( s => s.GetCurrentTheme() ).Returns( AppTheme.Default );

        var vm = new SettingsViewModel(
            new AppConfig(),
            mockThemeSelectorService.Object,
            Mock.Of<SystemService>(),
            Mock.Of<ApplicationInfoService>(),
            Mock.Of<DialogProvider>(),
            Mock.Of<EnvironmentProvider>(),
            mockAppSettingsService.Object
        );

        // Act
        vm.OnNavigatedTo( null );
        vm.SourceAircraftDir = "new Aircraft Dir";
        vm.SourceDlcCampaignDir = "new DLC Campaign Dir";
        vm.SourceUserDir = "new User Dir";
        vm.TranslateFileDir = "new Translate File Dir";

        // Assert
        mockAppSettingsService.VerifySet( s => s.SourceAircraftDir = "new Aircraft Dir", Times.Once() );
        mockAppSettingsService.VerifySet( s => s.SourceDlcCampaignDir = "new DLC Campaign Dir", Times.Once() );
        mockAppSettingsService.VerifySet( s => s.SourceUserDir = "new User Dir", Times.Once() );
        mockAppSettingsService.VerifySet( s => s.TranslateFileDir = "new Translate File Dir", Times.Once() );
    }

    [Fact( DisplayName = "リセットコマンドで初期値に戻る" )]
    public void ResetCommand_ShouldRestoreDefaults() {
        // Arrange
        var mockAppSettingsService = new Mock<IAppSettingsService>();
        mockAppSettingsService.CallBase = true;
        var mockThemeSelectorService = new Mock<IThemeSelectorService>();
        mockThemeSelectorService.Setup( s => s.GetCurrentTheme() ).Returns( AppTheme.Default );

        var vm = new SettingsViewModel(
            new AppConfig(),
            mockThemeSelectorService.Object,
            Mock.Of<SystemService>(),
            Mock.Of<ApplicationInfoService>(),
            Mock.Of<DialogProvider>(),
            Mock.Of<EnvironmentProvider>(),
            mockAppSettingsService.Object
        );
        vm.SourceAircraftDir = "A";
        vm.SourceDlcCampaignDir = "B";
        vm.SourceUserDir = "C";
        vm.TranslateFileDir = "D";

        // Act
        vm.ResetSettingsCommand.Execute( null );

        // Assert
        mockAppSettingsService.Verify( s => s.Reset(), Times.Once() );
        Assert.Equal( string.Empty, vm.SourceAircraftDir );
        Assert.Equal( string.Empty, vm.SourceDlcCampaignDir );
        Assert.Equal( string.Empty, vm.SourceUserDir );
        Assert.Equal(
            Path.Combine(
                Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location ), "TranslateFiles" ),
            vm.TranslateFileDir );
    }
}
