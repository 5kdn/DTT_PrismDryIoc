using DcsTranslateTool.Win.Constants;
using DcsTranslateTool.Win.Contracts.Services;

namespace DcsTranslateTool.Win.ViewModels;

/// <summary>
/// MainPage のロジックを制御する ViewModel である。
/// </summary>
/// <param name="regionManager">リージョン管理用サービス</param>
/// <param name="_snackbarService">Snackbar サービス</param>
public class MainViewModel(
    IRegionManager regionManager,
    IAppSettingsService _appSettingsService,
    ISnackbarService _snackbarService
) : BindableBase, INavigationAware {

    private DelegateCommand? _openSettingsCommand;
    private DelegateCommand? _openDownloadCommand;
    private DelegateCommand? _openUploadCommand;

    /// <summary>
    /// 設定画面を開くコマンド
    /// </summary>
    public DelegateCommand OpenSettingsCommand => _openSettingsCommand ??= new DelegateCommand( OnOpenSettings );

    /// <summary>
    /// ダウンロード画面を開くコマンド
    /// </summary>
    public DelegateCommand OpenDownloadCommand => _openDownloadCommand ??= new DelegateCommand( OnOpenDownload );

    /// <summary>
    /// アップロード画面を開くコマンド
    /// </summary>
    public DelegateCommand OpenUploadCommand => _openUploadCommand ??= new DelegateCommand( OnOpenUpload );

    /// <summary>
    /// ナビゲーションターゲットかどうかを示す
    /// </summary>
    /// <param name="navigationContext">ナビゲーションコンテキスト</param>
    /// <returns>常に true</returns>
    public bool IsNavigationTarget( NavigationContext navigationContext ) => true;

    /// <summary>
    /// ナビゲーション前の処理を行う
    /// </summary>
    /// <param name="navigationContext">ナビゲーションコンテキスト</param>
    public void OnNavigatedFrom( NavigationContext navigationContext ) {
        _snackbarService.Clear();
    }

    /// <summary>
    /// ナビゲーション後の処理を行う
    /// </summary>
    /// <param name="navigationContext">ナビゲーションコンテキスト</param>
    public void OnNavigatedTo( NavigationContext navigationContext ) {
        ValidateAppSettingsAndNotify();
    }

    private void OnOpenSettings() => regionManager.RequestNavigate( Regions.Main, PageKeys.Settings );

    private void OnOpenDownload() => regionManager.RequestNavigate( Regions.Main, PageKeys.Download );

    private void OnOpenUpload() => regionManager.RequestNavigate( Regions.Main, PageKeys.Upload );

    private void ValidateAppSettingsAndNotify() {
        if(string.IsNullOrEmpty( _appSettingsService.TranslateFileDir ) ||
           string.IsNullOrEmpty( _appSettingsService.SourceAircraftDir ) ||
           string.IsNullOrEmpty( _appSettingsService.SourceDlcCampaignDir )
        ) {
            _snackbarService.Show( "設定が不足しています。", "設定", OnOpenSettings, null, TimeSpan.FromDays( 1 ) );
        }
    }
}