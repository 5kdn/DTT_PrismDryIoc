using DcsTranslateTool.Win.Constants;
using DcsTranslateTool.Win.Contracts.Services;
using System.IO;

using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Models;

namespace DcsTranslateTool.Win.ViewModels;

public class UploadViewModel : BindableBase, INavigationAware {
    private readonly IAppSettingsService _appSettingsService;
    private readonly IRegionManager _regionManager;
    private readonly IFileService _fileService;
    private readonly IUploadDialogService _uploadDialogService;

    private DelegateCommand _openSettingsCommand;
    private DelegateCommand _uploadCommand;
    private DelegateCommand<UploadTreeItemViewModel> _loadTreeCommand;
    private DelegateCommand _uploadCommand;
    private DelegateCommand<UploadTreeItemViewModel> _loadTreeCommand;

    private UploadTreeItemViewModel _aircraftRoot;
    private UploadTreeItemViewModel _campaignRoot;
    private int _selectedTabIndex;
    private bool _isUploadEnabled = true;

    /// <summary>
    /// 設定画面を開くコマンド
    /// </summary>
    public DelegateCommand OpenSettingsCommand => _openSettingsCommand ??= new DelegateCommand( OnOpenSettings );

    /// <summary>
    /// ファイルツリーを展開するコマンドである。
    /// </summary>
    public DelegateCommand<UploadTreeItemViewModel> LoadTreeCommand =>
        _loadTreeCommand ??= new DelegateCommand<UploadTreeItemViewModel>( OnLoadTree );

    /// <summary>
    /// アップロード処理を実行するコマンドである。
    /// </summary>
    public DelegateCommand UploadCommand => _uploadCommand ??= new DelegateCommand( OnUpload, () => IsUploadEnabled ).ObservesProperty( () => IsUploadEnabled );

    /// <summary>
    /// 機体フォルダのルートを取得または設定する。
    /// </summary>
    public UploadTreeItemViewModel AircraftRoot {
        get => _aircraftRoot;
        set => SetProperty( ref _aircraftRoot, value );
    }

    /// <summary>
    /// DLC キャンペーンフォルダのルートを取得または設定する。
    /// </summary>
    public UploadTreeItemViewModel CampaignRoot {
        get => _campaignRoot;
        set => SetProperty( ref _campaignRoot, value );
    }

    /// <summary>
    /// 選択中のタブインデックスを取得または設定する。
    /// </summary>
    public int SelectedTabIndex {
        get => _selectedTabIndex;
        set => SetProperty( ref _selectedTabIndex, value );
    }

    /// <summary>
    /// アップロードボタンの有効状態を取得または設定する。
    /// </summary>
    public bool IsUploadEnabled {
        get => _isUploadEnabled;
        set => SetProperty( ref _isUploadEnabled, value );
    }


    public UploadViewModel(
        IAppSettingsService appSettingsService,
        IRegionManager regionManager,
        IFileService fileService,
        IUploadDialogService uploadDialogService
    ) {
        _appSettingsService = appSettingsService;
        _regionManager = regionManager;
        _fileService = fileService;
        _uploadDialogService = uploadDialogService;
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
    public void OnNavigatedTo( NavigationContext navigationContext ) {
        RefreshTrees();
    }

    /// <summary>
    /// 設定ページに遷移する
    /// </summary>
    private void OnOpenSettings() => _regionManager.RequestNavigate( Regions.Main, PageKeys.Settings );

    private void OnLoadTree( UploadTreeItemViewModel node ) {
        if(node == null) return;
        node.UpdateChildren( _fileService );
    }

    private void RefreshTrees() {
        RefreshAircraftTree();
        RefreshCampaignTree();
    }

    private void RefreshAircraftTree() {
        var path = Path.Combine( _appSettingsService.TranslateFileDir, "aircraft" );
        if(string.IsNullOrEmpty( path ) || !Directory.Exists( path )) {
            var empty = new FileTree { Name = path, AbsolutePath = path, IsDirectory = true };
            AircraftRoot = new UploadTreeItemViewModel( empty );
            return;
        }
        FileTree tree = _fileService.GetFileTree( path );
        var root = new UploadTreeItemViewModel( tree );
        root.UpdateChildren( _fileService );
        AircraftRoot = root;
    }

    private void RefreshCampaignTree() {
        var path = Path.Combine( _appSettingsService.TranslateFileDir, "campaigns" );
        if(string.IsNullOrEmpty( path ) || !Directory.Exists( path )) {
            var empty = new FileTree { Name = path, AbsolutePath = path, IsDirectory = true };
            CampaignRoot = new UploadTreeItemViewModel( empty );
            return;
        }
        FileTree tree = _fileService.GetFileTree( path );
        var root = new UploadTreeItemViewModel( tree );
        root.UpdateChildren( _fileService );
        CampaignRoot = root;
    }

    private void OnUpload() {
        UploadTreeItemViewModel root = SelectedTabIndex == 0 ? AircraftRoot : CampaignRoot;
        if(root == null) return;
        var files = root.GetCheckedFiles().Select( f => f.AbsolutePath ).ToList();
        if(files.Count == 0) return;
        IsUploadEnabled = false;
        _uploadDialogService.ShowDialog( files );
        IsUploadEnabled = true;
    }
}
