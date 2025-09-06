using System.Windows.Input;

using DcsTranslateTool.Win.Constants;
using DcsTranslateTool.Win.Contracts.Services;

using MaterialDesignThemes.Wpf;

namespace DcsTranslateTool.Win.ViewModels;

/// <summary>
/// アプリケーションシェルを制御する ViewModel
/// </summary>
/// <param name="_regionManager">リージョン管理用サービス</param>
/// <param name="_snackbarService">Snackbar サービス</param>
public class ShellViewModel( IRegionManager _regionManager, IAppSettingsService _appSettingsService, ISnackbarService _snackbarService ) : BindableBase {
    private IRegionNavigationService? _navigationService;
    private DelegateCommand? _goBackCommand;
    private DelegateCommand? _openSettingsCommand;
    private ICommand? _loadedCommand;
    private ICommand? _unloadedCommand;

    /// <summary>
    /// Snackbar のメッセージキューを取得するプロパティである。
    /// </summary>
    public ISnackbarMessageQueue MessageQueue => _snackbarService.MessageQueue;

    /// <summary>
    /// 戻るボタン用のコマンド
    /// </summary>
    public DelegateCommand GoBackCommand => _goBackCommand ??= new DelegateCommand( OnGoBack, CanGoBack );

    /// <summary>
    /// 設定画面を開くコマンド
    /// </summary>
    public DelegateCommand OpenSettingsCommand => _openSettingsCommand ??= new DelegateCommand( OnOpenSettings );


    /// <summary>
    /// ウィンドウロード時に呼び出されるコマンド
    /// </summary>
    public ICommand LoadedCommand => _loadedCommand ??= new DelegateCommand( OnLoaded );

    /// <summary>
    /// ウィンドウアンロード時に呼び出されるコマンド
    /// </summary>
    public ICommand UnloadedCommand => _unloadedCommand ??= new DelegateCommand( OnUnloaded );

    private void OnLoaded() {
        _navigationService = _regionManager.Regions[Regions.Main].NavigationService;
        _navigationService.Navigated += OnNavigated;
        _navigationService.RequestNavigate( PageKeys.Main );
        _goBackCommand?.RaiseCanExecuteChanged();
    }

    private void OnUnloaded() {
        if(_navigationService != null) {
            _navigationService.Navigated -= OnNavigated;
        }
        _regionManager.Regions.Remove( Regions.Main );
    }

    private bool CanGoBack() => _navigationService?.Journal.CanGoBack == true;

    private void OnGoBack() => _navigationService?.Journal.GoBack();

    /// <summary>
    /// 設定ページに遷移する
    /// </summary>
    private void OnOpenSettings() => _regionManager.RequestNavigate( Regions.Main, PageKeys.Settings );

    private void OnNavigated( object? sender, RegionNavigationEventArgs e ) {
        _goBackCommand?.RaiseCanExecuteChanged();
        ValidateAppSettingsAndNotify();
    }

    private void ValidateAppSettingsAndNotify() {
        if(string.IsNullOrEmpty( _appSettingsService.TranslateFileDir ) ||
           string.IsNullOrEmpty( _appSettingsService.SourceAircraftDir ) ||
           string.IsNullOrEmpty( _appSettingsService.SourceDlcCampaignDir )
        ) {
            _snackbarService.Show( "設定が不足しています。", "設定", OnOpenSettings, null );
        }
    }
}