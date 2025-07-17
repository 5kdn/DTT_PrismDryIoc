using DcsTranslateTool.Win.Constants;
using DcsTranslateTool.Win.Contracts.Services;

namespace DcsTranslateTool.Win.ViewModels;

public class MainViewModel : BindableBase, INavigationAware {
    private readonly IRegionManager _regionManager;
    private readonly IAppSettingsService _appSettingsService;

    private DelegateCommand _openSettingsCommand;
    private DelegateCommand _openDownloadCommand;
    private DelegateCommand _openUploadCommand;

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


    public MainViewModel(
        IRegionManager regionManager,
        IAppSettingsService appSettingsService
    ) {
        _regionManager = regionManager;
        _appSettingsService = appSettingsService;
    }

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
    public void OnNavigatedTo( NavigationContext navigationContext ) { }

    private void OnOpenSettings() => _regionManager.RequestNavigate( Regions.Main, PageKeys.Settings );

    private void OnOpenDownload() => _regionManager.RequestNavigate( Regions.Main, PageKeys.Download );

    private void OnOpenUpload() => _regionManager.RequestNavigate( Regions.Main, PageKeys.Upload );
}
