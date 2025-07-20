using System.IO;

using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.Constants;
using DcsTranslateTool.Win.Contracts.Services;

namespace DcsTranslateTool.Win.ViewModels;

public class UploadViewModel : BindableBase, INavigationAware {
    private readonly IAppSettingsService _appSettingsService;
    private readonly IRegionManager _regionManager;
    private readonly IDialogService _dialogService;
    private readonly IFileService _fileService;

    private FileTreeItemViewModel _localAircraftRoot;
    private FileTreeItemViewModel _localDlcCampaignsRoot;
    private int _selectedTabIndex;
    private bool _isCreatePullRequestDialogButtonEnabled = true;

    private DelegateCommand _openSettingsCommand;
    private DelegateCommand _openCreatePullRequestDialogCommand;
    private DelegateCommand<FileTreeItemViewModel> _loadLocalTreeCommand;

    /// <summary>
    /// ローカルのAircraftsフォルダツリー
    /// </summary>
    public FileTreeItemViewModel LocalAircraftRoot {
        get => _localAircraftRoot;
        set => SetProperty( ref _localAircraftRoot, value );
    }

    /// <summary>
    /// ローカルのDLC Campaignsフォルダツリー
    /// </summary>
    public FileTreeItemViewModel LocalDlcCampaignsRoot {
        get => _localDlcCampaignsRoot;
        set => SetProperty( ref _localDlcCampaignsRoot, value );
    }

    /// <summary>
    /// 選択中のタブインデックス
    /// </summary>
    public int SelectedTabIndex {
        get => _selectedTabIndex;
        set => SetProperty( ref _selectedTabIndex, value );
    }

    /// <summary>
    /// Pull Request 作成ダイアログを開くボタンが有効かどうかを示す
    /// </summary>
    public bool IsCreatePullRequestDialogButtonEnabled {
        get => _isCreatePullRequestDialogButtonEnabled;
        set => SetProperty( ref _isCreatePullRequestDialogButtonEnabled, value );
    }



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

    /// <summary>
    /// ナビゲーションターゲットかどうかを示す
    /// </summary>
    /// <param name="navigationContext">ナビゲーションコンテキスト</param>
    /// <returns>常に true</returns>
    public bool IsNavigationTarget( NavigationContext navigationContext ) => true;

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
    /// LocalAircraftRootをTranslateFileDirから初期化
    /// </summary>
    private void RefreshLocalAircraftTree() {
        var path = Path.Join(
            _appSettingsService.TranslateFileDir,
            "DCSWorld",
            "Mods",
            "aircraft"
        );
        if(string.IsNullOrEmpty( path ) || !Directory.Exists( path )) {
            var emptyRoot = new FileTree { Name = path, AbsolutePath = path, IsDirectory = true };
            LocalAircraftRoot = new FileTreeItemViewModel( emptyRoot );
            return;
        }
        FileTree tree = _fileService.GetFileTree( path );
        var root = new FileTreeItemViewModel( tree );
        root.UpdateChildren( _fileService );
        LocalAircraftRoot = root;
    }

    /// <summary>
    /// LocalDlcCampaignRootをTranslateFileDirから初期化
    /// </summary>
    private void RefreshLocalDlcCampaignTree() {
        var path = Path.Join(
            _appSettingsService.TranslateFileDir,
            "DCSWorld",
            "Mods",
            "campaigns"
        );
        if(string.IsNullOrEmpty( path ) || !Directory.Exists( path )) {
            var emptyRoot = new FileTree { Name = path, AbsolutePath = path, IsDirectory = true };
            LocalDlcCampaignsRoot = new FileTreeItemViewModel( emptyRoot );
            return;
        }
        FileTree tree = _fileService.GetFileTree( path );
        var root = new FileTreeItemViewModel( tree );
        root.UpdateChildren( _fileService );
        LocalDlcCampaignsRoot = root;
    }

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
        // TODO: Remove RefreshLocalRoot();
        RefreshLocalAircraftTree();
        RefreshLocalDlcCampaignTree();
    }

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
