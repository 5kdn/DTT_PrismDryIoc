using System.IO;
using System.Reflection;
using System.Windows.Input;

using DcsTranslateTool.Contracts.Providers;
using DcsTranslateTool.Contracts.Services;
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
    private string _sourceAircraftDir;
    private string _sourceDlcCampaignDir;
    private string _sourceUserDir;
    private string _translateFileDir;
    private ICommand _setThemeCommand;
    private ICommand _privacyStatementCommand;
    private ICommand _selectAircraftDirCommand;
    private ICommand _selectDlcCampaignDirCommand;
    private ICommand _selectUserDirCommand;
    private ICommand _selectTranslateFileDirCommand;
    private ICommand _resetSettingsCommand;

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

    public string SourceAircraftDir
    {
        get { return _sourceAircraftDir; }
        set
        {
            if(SetProperty( ref _sourceAircraftDir, value ))
            {
                _appConfig.SourceAircraftDir = value;
            }
        }
    }

    public string SourceDlcCampaignDir
    {
        get { return _sourceDlcCampaignDir; }
        set
        {
            if(SetProperty( ref _sourceDlcCampaignDir, value ))
            {
                _appConfig.SourceDlcCampaignDir = value;
            }
        }
    }

    public string SourceUserDir
    {
        get { return _sourceUserDir; }
        set
        {
            if(SetProperty( ref _sourceUserDir, value ))
            {
                _appConfig.SourceUserDir = value;
            }
        }
    }

    public string TranslateFileDir
    {
        get { return _translateFileDir; }
        set
        {
            if(SetProperty( ref _translateFileDir, value ))
            {
                _appConfig.TranslateFileDir = value;
            }
        }
    }

    public ICommand SetThemeCommand =>
        _setThemeCommand
        ?? (_setThemeCommand = new DelegateCommand<string>( OnSetTheme ));

    public ICommand PrivacyStatementCommand =>
        _privacyStatementCommand
        ?? (_privacyStatementCommand = new DelegateCommand( OnPrivacyStatement ));

    public ICommand SelectAircraftDirCommand =>
        _selectAircraftDirCommand
        ?? (_selectAircraftDirCommand = new DelegateCommand( OnSelectAircraftDir ));

    public ICommand SelectDlcCampaignDirCommand =>
        _selectDlcCampaignDirCommand
        ?? (_selectDlcCampaignDirCommand = new DelegateCommand( OnSelectDlcCampaignDir ));

    public ICommand SelectUserDirCommand =>
        _selectUserDirCommand
        ?? (_selectUserDirCommand = new DelegateCommand( OnSelectUserDir ));

    public ICommand SelectTranslateFileDirCommand =>
        _selectTranslateFileDirCommand
        ?? (_selectTranslateFileDirCommand = new DelegateCommand( OnSelectTranslateFileDir ));

    /// <summary>
    /// 設定を初期状態へ戻すコマンド
    /// </summary>
    public ICommand ResetSettingsCommand =>
        _resetSettingsCommand
        ?? (_resetSettingsCommand = new DelegateCommand( OnResetSettings ));

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
    }

    public void OnNavigatedTo( NavigationContext navigationContext )
    {
        VersionDescription = $"{Properties.Resources.AppDisplayName} - {_applicationInfoService.GetVersion()}";
        Theme = _themeSelectorService.GetCurrentTheme();
        if(!string.IsNullOrEmpty( _appConfig.SourceAircraftDir ))
        {
            SourceAircraftDir = _appConfig.SourceAircraftDir;
        }
        else
        {
            SourceAircraftDir = string.Empty;
        }

        if(!string.IsNullOrEmpty( _appConfig.SourceDlcCampaignDir ))
        {
            SourceDlcCampaignDir = _appConfig.SourceDlcCampaignDir;
        }
        else
        {
            SourceDlcCampaignDir = string.Empty;
        }

        if(!string.IsNullOrEmpty( _appConfig.SourceUserDir ))
        {
            SourceUserDir = _appConfig.SourceUserDir;
        }
        else
        {
            var userProfile = _environmentProvider.GetUserProfilePath();
            var openBeta = Path.Combine( userProfile, "DCS.openbeta" );
            var release = Path.Combine( userProfile, "DCS" );
            if( Directory.Exists( openBeta ) )
            {
                SourceUserDir = openBeta;
            }
            else if( Directory.Exists( release ) )
            {
                SourceUserDir = release;
            }
            else
            {
                SourceUserDir = string.Empty;
            }
        }

        if(!string.IsNullOrEmpty( _appConfig.TranslateFileDir ))
        {
            TranslateFileDir = _appConfig.TranslateFileDir;
        }
        else
        {
            var exeDir = Path.GetDirectoryName( Assembly.GetEntryAssembly()?.Location );
            TranslateFileDir = Path.Combine( exeDir!, "TranslateFiles" );
        }
    }

    public void OnNavigatedFrom( NavigationContext navigationContext ) { }

    private void OnSetTheme( string themeName )
    {
        var theme = (AppTheme)Enum.Parse(typeof(AppTheme), themeName);
        _themeSelectorService.SetTheme( theme );
    }

    private void OnPrivacyStatement()
        => _systemService.OpenInWebBrowser( _appConfig.PrivacyStatement );

    private void OnSelectAircraftDir()
    {
        if( _dialogProvider.ShowFolderPicker( SourceAircraftDir, out var path ) )
        {
            SourceAircraftDir = path;
        }
    }

    private void OnSelectDlcCampaignDir()
    {
        if( _dialogProvider.ShowFolderPicker( SourceDlcCampaignDir, out var path ) )
        {
            SourceDlcCampaignDir = path;
        }
    }

    private void OnSelectUserDir()
    {
        if( _dialogProvider.ShowFolderPicker( SourceUserDir, out var path ) )
        {
            SourceUserDir = path;
        }
    }

    private void OnSelectTranslateFileDir()
    {
        if( _dialogProvider.ShowFolderPicker( TranslateFileDir, out var path ) )
        {
            TranslateFileDir = path;
        }
    }

    /// <summary>
    /// フォルダ設定を初期化する
    /// </summary>
    private void OnResetSettings()
    {
        SourceAircraftDir = string.Empty;
        SourceDlcCampaignDir = string.Empty;

        var userProfile = _environmentProvider.GetUserProfilePath();
        var openBeta = Path.Combine( userProfile, "DCS.openbeta" );
        var release = Path.Combine( userProfile, "DCS" );
        if( Directory.Exists( openBeta ) )
        {
            SourceUserDir = openBeta;
        }
        else if( Directory.Exists( release ) )
        {
            SourceUserDir = release;
        }
        else
        {
            SourceUserDir = string.Empty;
        }

        var exeDir = Path.GetDirectoryName( Assembly.GetEntryAssembly()?.Location );
        TranslateFileDir = Path.Combine( exeDir!, "TranslateFiles" );
    }

    public bool IsNavigationTarget( NavigationContext navigationContext )
        => true;
}
