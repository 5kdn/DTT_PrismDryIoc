using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Services;
using DcsTranslateTool.Share.Contracts.Services;
using DcsTranslateTool.Share.Services;
using DcsTranslateTool.Win.Constants;
using DcsTranslateTool.Win.Contracts.Providers;
using DcsTranslateTool.Win.Contracts.Services;
using DcsTranslateTool.Win.Models;
using DcsTranslateTool.Win.Providers;
using DcsTranslateTool.Win.Services;
using DcsTranslateTool.Win.ViewModels;
using DcsTranslateTool.Win.Views;

using Microsoft.Extensions.Configuration;

namespace DcsTranslateTool.Win;

// For more information about application lifecycle events see https://docs.microsoft.com/dotnet/framework/wpf/app-development/application-management-overview
// For docs about using Prism in WPF see https://prismlibrary.com/docs/wpf/introduction.html

// WPF UI elements use language en-US by default.
// If you need to support other cultures make sure you add converters and review dates and numbers in your UI to ensure everything adapts correctly.
// Tracking issue for improving this is https://github.com/dotnet/wpf/issues/1946
public partial class App : PrismApplication {
    private string[] _startUpArgs;

    public App() { }

    protected override Window CreateShell() => Container.Resolve<ShellWindow>();

    protected override async void OnInitialized() {
        var persistAndRestoreService = Container.Resolve<IPersistAndRestoreService>();
        persistAndRestoreService.RestoreData();

        var themeSelectorService = Container.Resolve<IThemeSelectorService>();
        themeSelectorService.InitializeTheme();

        base.OnInitialized();
        await Task.CompletedTask;
    }

    protected override void OnStartup( StartupEventArgs e ) {
        _startUpArgs = e.Args;
        base.OnStartup( e );
    }

    protected override void RegisterTypes( IContainerRegistry containerRegistry ) {
        // Core Services
        containerRegistry.Register<IFileService, FileService>();

        // Share Service
        containerRegistry.Register<IRepositoryService, RepositoryService>();

        // App Services
        containerRegistry.Register<IApplicationInfoService, ApplicationInfoService>();
        containerRegistry.Register<ISystemService, SystemService>();
        containerRegistry.Register<IPersistAndRestoreService, PersistAndRestoreService>();
        containerRegistry.Register<IThemeSelectorService, ThemeSelectorService>();
        containerRegistry.Register<IDialogProvider, DialogProvider>();
        containerRegistry.Register<IEnvironmentProvider, EnvironmentProvider>();
        containerRegistry.Register<IAppSettingsService, AppSettingsService>();

        // Views
        containerRegistry.RegisterForNavigation<SettingsPage, SettingsViewModel>( PageKeys.Settings );
        containerRegistry.RegisterForNavigation<DownloadPage, DownloadViewModel>( PageKeys.Download );
        containerRegistry.RegisterForNavigation<ShellWindow, ShellViewModel>();

        // Configuration
        var configuration = BuildConfiguration();
        var appConfig = configuration
            .GetSection(nameof(AppConfig))
            .Get<AppConfig>();

        // Register configurations to IoC
        containerRegistry.RegisterInstance<IConfiguration>( configuration );
        containerRegistry.RegisterInstance<AppConfig>( appConfig );

        // カスタム引数
        var dryIoc = containerRegistry.GetContainer();
        dryIoc.RegisterDelegate<IRepositoryService>(
            r => new RepositoryService( new GitHubApiClient( "5kdn", "test_DCS", "DCSTranslateTool", 1510695, 74330212 ) ),
            DryIoc.Reuse.Transient );
    }

    private IConfiguration BuildConfiguration() {
        var appLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        return new ConfigurationBuilder()
            .SetBasePath( appLocation )
            .AddJsonFile( "appsettings.json" )
            .AddCommandLine( _startUpArgs )
            .Build();
    }

    private void OnExit( object sender, ExitEventArgs e ) {
        var persistAndRestoreService = Container.Resolve<IPersistAndRestoreService>();
        persistAndRestoreService.PersistData();
    }

    private void OnDispatcherUnhandledException( object sender, DispatcherUnhandledExceptionEventArgs e ) {
        // TODO: Please log and handle the exception as appropriate to your scenario
        // For more info see https://docs.microsoft.com/dotnet/api/system.windows.application.dispatcherunhandledexception?view=netcore-3.0
    }
}
