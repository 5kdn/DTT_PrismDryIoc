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
public class SettingsViewModel(
    AppConfig appConfig,
    IThemeSelectorService themeSelectorService,
    ISystemService systemService,
    IApplicationInfoService applicationInfoService,
    IDialogProvider dialogProvider,
    IEnvironmentProvider environmentProvider,
    IAppSettingsService appSettingsService
) : BindableBase, INavigationAware {
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
                appSettingsService.SourceAircraftDir = value;
            }
        }
    }

    public string SourceDlcCampaignDir {
        get { return _sourceDlcCampaignDir; }
        set {
            if(SetProperty( ref _sourceDlcCampaignDir, value )) {
                appSettingsService.SourceDlcCampaignDir = value;
            }
        }
    }

    public string SourceUserDir {
        get { return _sourceUserDir; }
        set {
            if(SetProperty( ref _sourceUserDir, value )) {
                appSettingsService.SourceUserDir = value;
            }
        }
    }

    public string TranslateFileDir {
        get { return _translateFileDir; }
        set {
            if(SetProperty( ref _translateFileDir, value )) {
                appSettingsService.TranslateFileDir = value;
            }
        }
    }

    public ICommand SetThemeCommand => _setThemeCommand ??= new DelegateCommand<string>( OnSetTheme );

    public ICommand PrivacyStatementCommand => _privacyStatementCommand ??= new DelegateCommand( OnPrivacyStatement );

    public ICommand SelectAircraftDirCommand => _selectAircraftDirCommand ??= new DelegateCommand( OnSelectAircraftDir );

    public ICommand SelectDlcCampaignDirCommand => _selectDlcCampaignDirCommand ??= new DelegateCommand( OnSelectDlcCampaignDir );

    public ICommand SelectUserDirCommand => _selectUserDirCommand ??= new DelegateCommand( OnSelectUserDir );

    public ICommand SelectTranslateFileDirCommand => _selectTranslateFileDirCommand ??= new DelegateCommand( OnSelectTranslateFileDir );

    public ICommand ResetSettingsCommand => _resetSettingsCommand ??= new DelegateCommand( OnResetSettings );

    public void OnNavigatedTo( NavigationContext navigationContext ) {
        VersionDescription = $"{Properties.Resources.AppDisplayName} - {applicationInfoService.GetVersion()}";
        Theme = themeSelectorService.GetCurrentTheme();
        LoadSettings();
    }

    public void OnNavigatedFrom( NavigationContext navigationContext ) { }

    private void OnSetTheme( string themeName ) {
        var theme = (AppTheme)Enum.Parse(typeof(AppTheme), themeName);
        themeSelectorService.SetTheme( theme );
    }

    private void OnPrivacyStatement()
        => systemService.OpenInWebBrowser( appConfig.PrivacyStatement );

    private void OnSelectAircraftDir() {
        if(dialogProvider.ShowFolderPicker( SourceAircraftDir, out var path )) {
            SourceAircraftDir = path;
        }
    }

    private void OnSelectDlcCampaignDir() {
        if(dialogProvider.ShowFolderPicker( SourceDlcCampaignDir, out var path )) {
            SourceDlcCampaignDir = path;
        }
    }

    private void OnSelectUserDir() {
        if(dialogProvider.ShowFolderPicker( SourceUserDir, out var path )) {
            SourceUserDir = path;
        }
    }

    private void OnSelectTranslateFileDir() {
        if(dialogProvider.ShowFolderPicker( TranslateFileDir, out var path )) {
            TranslateFileDir = path;
        }
    }

    /// <summary>
    /// 設定を初期値に戻す
    /// </summary>
    private void OnResetSettings() => ResetToDefault();

    /// <summary>
    /// 保存された設定を読み込む
    /// </summary>
    private void LoadSettings() {
        SourceAircraftDir = appSettingsService.SourceAircraftDir;
        SourceDlcCampaignDir = appSettingsService.SourceDlcCampaignDir;
        SourceUserDir = string.IsNullOrEmpty( appSettingsService.SourceUserDir )
            ? GetDefaultUserDir()
            : appSettingsService.SourceUserDir;
        TranslateFileDir = string.IsNullOrEmpty( appSettingsService.TranslateFileDir )
            ? GetDefaultTranslateDir()
            : appSettingsService.TranslateFileDir;
    }

    /// <summary>
    /// 初期値を設定する
    /// </summary>
    private void ResetToDefault() {
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
        var userProfile = environmentProvider.GetUserProfilePath();
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
    private static string GetDefaultTranslateDir() {
        var exeDir = Path.GetDirectoryName( Assembly.GetEntryAssembly()?.Location );
        return Path.Combine( exeDir!, "TranslateFiles" );
    }

    public bool IsNavigationTarget( NavigationContext navigationContext ) => true;
}