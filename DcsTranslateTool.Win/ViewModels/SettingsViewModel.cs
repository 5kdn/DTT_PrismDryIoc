using System.IO;
using System.Reflection;
using System.Windows.Input;

using DcsTranslateTool.Win.Contracts.Providers;
using DcsTranslateTool.Win.Contracts.Services;
using DcsTranslateTool.Win.Models;

namespace DcsTranslateTool.Win.ViewModels;

/// <summary>
/// 設定ページを制御する ViewModel
/// </summary>
public class SettingsViewModel : BindableBase, INavigationAware {
    private readonly AppConfig _appConfig;
    private readonly IThemeSelectorService _themeSelectorService;
    private readonly ISystemService _systemService;
    private readonly IApplicationInfoService _applicationInfoService;
    private readonly IDialogProvider _dialogProvider;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IAppSettingsService _appSettingsService;
    private AppTheme _theme;
    private string _versionDescription = string.Empty;
    private string _sourceAircraftDir = string.Empty;
    private string _sourceDlcCampaignDir = string.Empty;
    private string _sourceUserDir = string.Empty;
    private string _translateFileDir = string.Empty;
    private ICommand? _setThemeCommand;
    private ICommand? _privacyStatementCommand;
    private ICommand? _selectAircraftDirCommand;
    private ICommand? _selectDlcCampaignDirCommand;
    private ICommand? _selectUserDirCommand;
    private ICommand? _selectTranslateFileDirCommand;
    private ICommand? _resetSettingsCommand;

    public AppTheme Theme {
        get { return _theme; }
        set { SetProperty( ref _theme, value ); }
    }

    public string VersionDescription {
        get { return _versionDescription; }
        set { SetProperty( ref _versionDescription, value ); }
    }

    public string SourceAircraftDir {
        get { return _sourceAircraftDir; }
        set {
            if(SetProperty( ref _sourceAircraftDir, value )) {
                _appSettingsService.SourceAircraftDir = value;
            }
        }
    }

    public string SourceDlcCampaignDir {
        get { return _sourceDlcCampaignDir; }
        set {
            if(SetProperty( ref _sourceDlcCampaignDir, value )) {
                _appSettingsService.SourceDlcCampaignDir = value;
            }
        }
    }

    public string SourceUserDir {
        get { return _sourceUserDir; }
        set {
            if(SetProperty( ref _sourceUserDir, value )) {
                _appSettingsService.SourceUserDir = value;
            }
        }
    }

    public string TranslateFileDir {
        get { return _translateFileDir; }
        set {
            if(SetProperty( ref _translateFileDir, value )) {
                _appSettingsService.TranslateFileDir = value;
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

    public ICommand ResetSettingsCommand =>
        _resetSettingsCommand
        ?? (_resetSettingsCommand = new DelegateCommand( OnResetSettings ));

    public SettingsViewModel(
        AppConfig appConfig,
        IThemeSelectorService themeSelectorService,
        ISystemService systemService,
        IApplicationInfoService applicationInfoService,
        IDialogProvider dialogProvider,
        IEnvironmentProvider environmentProvider,
        IAppSettingsService appSettingsService
    ) {
        _appConfig = appConfig;
        _themeSelectorService = themeSelectorService;
        _systemService = systemService;
        _applicationInfoService = applicationInfoService;
        _dialogProvider = dialogProvider;
        _environmentProvider = environmentProvider;
        _appSettingsService = appSettingsService;
    }

    public void OnNavigatedTo( NavigationContext navigationContext ) {
        VersionDescription = $"{Properties.Resources.AppDisplayName} - {_applicationInfoService.GetVersion()}";
        Theme = _themeSelectorService.GetCurrentTheme();
        LoadSettings();
    }

    public void OnNavigatedFrom( NavigationContext navigationContext ) { }

    private void OnSetTheme( string themeName ) {
        var theme = (AppTheme)Enum.Parse(typeof(AppTheme), themeName);
        _themeSelectorService.SetTheme( theme );
    }

    private void OnPrivacyStatement()
        => _systemService.OpenInWebBrowser( _appConfig.PrivacyStatement );

    private void OnSelectAircraftDir() {
        if(_dialogProvider.ShowFolderPicker( SourceAircraftDir, out var path )) {
            SourceAircraftDir = path;
        }
    }

    private void OnSelectDlcCampaignDir() {
        if(_dialogProvider.ShowFolderPicker( SourceDlcCampaignDir, out var path )) {
            SourceDlcCampaignDir = path;
        }
    }

    private void OnSelectUserDir() {
        if(_dialogProvider.ShowFolderPicker( SourceUserDir, out var path )) {
            SourceUserDir = path;
        }
    }

    private void OnSelectTranslateFileDir() {
        if(_dialogProvider.ShowFolderPicker( TranslateFileDir, out var path )) {
            TranslateFileDir = path;
        }
    }

    /// <summary>
    /// 設定を初期値に戻す
    /// </summary>
    private void OnResetSettings()
        => ResetToDefault();

    /// <summary>
    /// 保存された設定を読み込む
    /// </summary>
    private void LoadSettings() {
        SourceAircraftDir = _appSettingsService.SourceAircraftDir;
        SourceDlcCampaignDir = _appSettingsService.SourceDlcCampaignDir;
        SourceUserDir = string.IsNullOrEmpty( _appSettingsService.SourceUserDir )
            ? GetDefaultUserDir()
            : _appSettingsService.SourceUserDir;
        TranslateFileDir = string.IsNullOrEmpty( _appSettingsService.TranslateFileDir )
            ? GetDefaultTranslateDir()
            : _appSettingsService.TranslateFileDir;
    }

    /// <summary>
    /// 初期値を設定する
    /// </summary>
    private void ResetToDefault() {
        _appSettingsService.Reset();
        SourceAircraftDir = string.Empty;
        SourceDlcCampaignDir = string.Empty;
        SourceUserDir = GetDefaultUserDir();
        TranslateFileDir = GetDefaultTranslateDir();
    }

    /// <summary>
    /// ユーザーディレクトリの初期値を取得する
    /// </summary>
    /// <returns>取得したパス</returns>
    private string GetDefaultUserDir() {
        var userProfile = _environmentProvider.GetUserProfilePath();
        var openBeta = Path.Combine( userProfile, "DCS.openbeta" );
        var release = Path.Combine( userProfile, "DCS" );
        if(Directory.Exists( openBeta )) {
            return openBeta;
        }
        if(Directory.Exists( release )) {
            return release;
        }
        return string.Empty;
    }

    /// <summary>
    /// 翻訳ファイルの保存先初期値を取得する
    /// </summary>
    /// <returns>取得したパス</returns>
    private string GetDefaultTranslateDir() {
        var exeDir = Path.GetDirectoryName( Assembly.GetEntryAssembly()?.Location );
        return Path.Combine( exeDir!, "TranslateFiles" );
    }

    public bool IsNavigationTarget( NavigationContext navigationContext ) => true;
}
