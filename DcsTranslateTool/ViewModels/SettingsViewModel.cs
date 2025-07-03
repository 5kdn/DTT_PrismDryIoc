using System.Windows.Input;
using System.IO;

using DcsTranslateTool.Contracts.Providers;
using DcsTranslateTool.Models;

namespace DcsTranslateTool.ViewModels;

// TODO: Change the URL for your privacy policy in the appsettings.json file, currently set to https://YourPrivacyUrlGoesHere
public class SettingsViewModel : BindableBase, INavigationAware
{
    private readonly AppConfig _appConfig;
    private readonly IThemeSelectorService _themeSelectorService;
    private readonly ISystemService _systemService;
    private readonly IApplicationInfoService _applicationInfoService;
    private readonly IDialogProvider _dialogProvider;
    private readonly IEnvironmentProvider _environmentProvider;
    private AppTheme _theme;
    private string _versionDescription;
    private ICommand _setThemeCommand;
    private ICommand _privacyStatementCommand;
    private string _sourceAircraftDir = string.Empty;
    private string _sourceDlcCampaignDir = string.Empty;
    private string _sourceUserDir = string.Empty;
    private ICommand _selectAircraftDirCommand;
    private ICommand _selectDlcCampaignDirCommand;
    private ICommand _selectUserDirCommand;

    public AppTheme Theme
    {
        get { return _theme; }
        set { SetProperty( ref _theme, value ); }
    }

    public string VersionDescription
    {
        get { return _versionDescription; }
        set { SetProperty( ref _versionDescription, value ); }
    }

    public ICommand SetThemeCommand =>
        _setThemeCommand
        ?? (_setThemeCommand = new DelegateCommand<string>( OnSetTheme ));

    public ICommand PrivacyStatementCommand =>
        _privacyStatementCommand
        ?? (_privacyStatementCommand = new DelegateCommand( OnPrivacyStatement ));

    /// <summary>
    /// DCSのAircraftディレクトリ
    /// </summary>
    public string SourceAircraftDir
    {
        get { return _sourceAircraftDir; }
        set { SetProperty( ref _sourceAircraftDir, value ); }
    }

    /// <summary>
    /// DCSのCampaignディレクトリ
    /// </summary>
    public string SourceDlcCampaignDir
    {
        get { return _sourceDlcCampaignDir; }
        set { SetProperty( ref _sourceDlcCampaignDir, value ); }
    }

    /// <summary>
    /// ユーザーディレクトリのルート
    /// </summary>
    public string SourceUserDir
    {
        get { return _sourceUserDir; }
        set { SetProperty( ref _sourceUserDir, value ); }
    }

    public ICommand SelectAircraftDirCommand =>
        _selectAircraftDirCommand ??= new DelegateCommand( OnSelectAircraftDir );

    public ICommand SelectDlcCampaignDirCommand =>
        _selectDlcCampaignDirCommand ??= new DelegateCommand( OnSelectDlcCampaignDir );

    public ICommand SelectUserDirCommand =>
        _selectUserDirCommand ??= new DelegateCommand( OnSelectUserDir );

    public SettingsViewModel(
        AppConfig appConfig,
        IThemeSelectorService themeSelectorService,
        ISystemService systemService,
        IApplicationInfoService applicationInfoService,
        IDialogProvider dialogProvider,
        IEnvironmentProvider environmentProvider
    )
    {
        _appConfig = appConfig;
        _themeSelectorService = themeSelectorService;
        _systemService = systemService;
        _applicationInfoService = applicationInfoService;
        _dialogProvider = dialogProvider;
        _environmentProvider = environmentProvider;
        _sourceUserDir = GetDefaultUserDir();
    }

    public void OnNavigatedTo( NavigationContext navigationContext )
    {
        VersionDescription = $"{Properties.Resources.AppDisplayName} - {_applicationInfoService.GetVersion()}";
        Theme = _themeSelectorService.GetCurrentTheme();
    }

    public void OnNavigatedFrom( NavigationContext navigationContext ) { }

    private void OnSetTheme( string themeName )
    {
        var theme = (AppTheme)Enum.Parse(typeof(AppTheme), themeName);
        _themeSelectorService.SetTheme( theme );
    }

    private void OnPrivacyStatement()
        => _systemService.OpenInWebBrowser( _appConfig.PrivacyStatement );

    public bool IsNavigationTarget( NavigationContext navigationContext )
        => true;

    private void OnSelectAircraftDir()
    {
        if(_dialogProvider.ShowFolderPicker( SourceAircraftDir, out var path ) && path is not null)
        {
            SourceAircraftDir = path;
        }
    }

    private void OnSelectDlcCampaignDir()
    {
        if(_dialogProvider.ShowFolderPicker( SourceDlcCampaignDir, out var path ) && path is not null)
        {
            SourceDlcCampaignDir = path;
        }
    }

    private void OnSelectUserDir()
    {
        if(_dialogProvider.ShowFolderPicker( SourceUserDir, out var path ) && path is not null)
        {
            SourceUserDir = path;
        }
    }

    private string GetDefaultUserDir()
    {
        var userProfile = _environmentProvider.GetUserProfile();
        var openBeta = Path.Combine( userProfile, "DCS.openbeta" );
        if( Directory.Exists( openBeta ) )
        {
            return openBeta;
        }

        var stable = Path.Combine( userProfile, "DCS" );
        return Directory.Exists( stable ) ? stable : string.Empty;
    }
}
