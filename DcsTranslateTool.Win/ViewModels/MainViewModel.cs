using DcsTranslateTool.Win.Constants;
using DcsTranslateTool.Win.Contracts.Services;

using MaterialDesignThemes.Wpf;

namespace DcsTranslateTool.Win.ViewModels;

public class MainViewModel(
    IRegionManager regionManager,
    IAppSettingsService _appSettingsService,
    ISnackbarMessageQueue _snackbarMessageQueue
) : BindableBase, INavigationAware {

    private DelegateCommand? _openSettingsCommand;
    private DelegateCommand? _openDownloadCommand;
    private DelegateCommand? _openUploadCommand;

    /// <summary>
    /// スナックバーのメッセージキューを取得する
    /// </summary>
    public ISnackbarMessageQueue MessageQueue { get; } = _snackbarMessageQueue;

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
    public void OnNavigatedFrom( NavigationContext navigationContext ) { }

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
        if(string.IsNullOrEmpty( _appSettingsService.TranslateFileDir )
            || string.IsNullOrEmpty( _appSettingsService.SourceAircraftDir )
            || string.IsNullOrEmpty( _appSettingsService.SourceDlcCampaignDir )) {
            MessageQueue.Enqueue(
                content: "設定が不足しています。",
                actionContent: "設定",
                actionHandler: ( _ ) => OnOpenSettings(),
                actionArgument: null,
                promote: false,
                neverConsiderToBeDuplicate: true,
                durationOverride: TimeSpan.FromDays( 1 )
            );
        }
    }
}