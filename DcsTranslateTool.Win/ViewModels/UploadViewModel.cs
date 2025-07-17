using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Win.Constants;
using DcsTranslateTool.Win.Contracts.Services;

namespace DcsTranslateTool.Win.ViewModels;

public class UploadViewModel : BindableBase, INavigationAware {
    private readonly IAppSettingsService _appSettingsService;
    private readonly IRegionManager _regionManager;
    private readonly IDialogService _dialogService;
    private readonly IFileService _fileService;

    private DelegateCommand _openSettingsCommand;
    private DelegateCommand _openCreatePullRequestDialogCommand;
    private DelegateCommand<FileTreeItemViewModel> _loadLocalTreeCommand;

    private FileTreeItemViewModel _localRoot;
    //private FileTreeItemViewModel _localAircraftRoot;
    //private FileTreeItemViewModel _localDlcCampaignsRoot;
    //private int _selectedTabIndex;

    private bool _isCreatePullRequestDialogButtonEnabled = true;


    /// <summary>
    /// ローカルツリーを読み込むコマンド
    /// </summary>
    public DelegateCommand<FileTreeItemViewModel> LoadLocalTreeCommand =>
        _loadLocalTreeCommand ??= new DelegateCommand<FileTreeItemViewModel>( OnLoadLocalTree );


    /// <summary>
    /// 設定画面を開くコマンド
    /// </summary>
    public DelegateCommand OpenSettingsCommand => _openSettingsCommand ??= new DelegateCommand( OnOpenSettings );

    public DelegateCommand OpenCreatePullRequestDialogCommand =>
        _openCreatePullRequestDialogCommand ??= new DelegateCommand( OnOpenCreatePullRequestDialog );

    public UploadViewModel(
        IAppSettingsService appSettingsService,
        IRegionManager regionManager,
        IDialogService dialogService,
        IFileService fileService
    ) {
        _appSettingsService = appSettingsService;
        _regionManager = regionManager;
        _dialogService = dialogService;
        _fileService = fileService;
    }

    private void OnLoadLocalTree( FileTreeItemViewModel node ) {
        if(node == null) return;
        node.UpdateChildren( _fileService );
    }

    /// <summary>
    /// ローカルのフォルダツリー
    /// </summary>
    public FileTreeItemViewModel LocalRoot {
        get => _localRoot;
        set => SetProperty( ref _localRoot, value );
    }

    /// <summary>
    /// Pull Request 作成ダイアログを開くボタンが有効かどうかを示す
    /// </summary>
    public bool IsCreatePullRequestDialogButtonEnabled {
        get => _isCreatePullRequestDialogButtonEnabled;
        set => SetProperty( ref _isCreatePullRequestDialogButtonEnabled, value );
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

    /// <summary>
    /// 設定ページに遷移する
    /// </summary>
    private void OnOpenSettings() => _regionManager.RequestNavigate( Regions.Main, PageKeys.Settings );

    /// <summary>
    /// Pull Request作成ダイアログを開く
    /// </summary>
    private void OnOpenCreatePullRequestDialog() {
        IsCreatePullRequestDialogButtonEnabled = false;
        var parameters = new DialogParameters(){
            {"files", new string[]{"files1", "files2", "files3" } }
        };
        _dialogService.ShowDialog( PageKeys.CreatePullRequestDialog, parameters, r => {
            // ダイアログの処理
            if(r.Result == ButtonResult.OK) {
                // OKボタンが押された場合の処理
            }
            else if(r.Result == ButtonResult.Cancel) {
                // キャンセルボタンが押された場合の処理
            }
            //    // ダイアログ終了後処理
            IsCreatePullRequestDialogButtonEnabled = true;
        } );
    }
}
