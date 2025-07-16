using System.Diagnostics;
using System.IO;

using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Share.Contracts.Services;
using DcsTranslateTool.Share.Models;
using DcsTranslateTool.Win.Constants;
using DcsTranslateTool.Win.Contracts.Services;

namespace DcsTranslateTool.Win.ViewModels;

/// <summary>
/// メイン画面の表示ロジックを保持する ViewModel
/// </summary>
public class DownloadViewModel : BindableBase, INavigationAware {
    private readonly IRegionManager _regionManager;
    private readonly IRepositoryService _repositoryService;
    private readonly IFileService _fileService;
    private readonly IAppSettingsService _appSettingsService;

    private DelegateCommand _openSettingsCommand;
    private DelegateCommand _fetchCommand;
    private DelegateCommand<FileTreeItemViewModel> _loadLocalTreeCommand;
    private DelegateCommand _downloadCommand;
    private DelegateCommand _resetCheckCommand;

    private RepoTreeItemViewModel _repoAircraftTree;
    private RepoTreeItemViewModel _repoDlcCampaignTree;
    private FileTreeItemViewModel _localAircraftRoot;
    private FileTreeItemViewModel _localDlcCampaignRoot;
    private int _selectedTabIndex;

    /// <summary>
    /// 設定画面を開くコマンド
    /// </summary>
    public DelegateCommand OpenSettingsCommand =>
        _openSettingsCommand ??= new DelegateCommand( OnOpenSettings );

    /// <summary>
    /// リポジトリツリーを取得するコマンド
    /// </summary>
    public DelegateCommand FetchCommand =>
        _fetchCommand ??= new DelegateCommand( OnFetch );

    /// <summary>
    /// ローカルツリーを読み込むコマンド
    /// </summary>
    public DelegateCommand<FileTreeItemViewModel> LoadLocalTreeCommand =>
        _loadLocalTreeCommand ??= new DelegateCommand<FileTreeItemViewModel>( OnLoadLocalTree );

    /// <summary>
    /// ファイルをダウンロードするコマンド
    /// </summary>
    public DelegateCommand DownloadCommand =>
        _downloadCommand ??= new DelegateCommand( OnDownload );

    /// <summary>
    /// チェック状態をリセットするコマンド
    /// </summary>
    public DelegateCommand ResetCheckCommand =>
        _resetCheckCommand ??= new DelegateCommand( OnResetCheck );

    /// <summary>
    /// リポジトリの機体フォルダツリー
    /// </summary>
    public RepoTreeItemViewModel RepoAircraftTree {
        get => _repoAircraftTree;
        set => SetProperty( ref _repoAircraftTree, value );
    }

    /// <summary>
    /// リポジトリのDLCキャンペーンフォルダツリー
    /// </summary>
    public RepoTreeItemViewModel RepoDlcCampaignTree {
        get => _repoDlcCampaignTree;
        set => SetProperty( ref _repoDlcCampaignTree, value );
    }

    /// <summary>
    /// ローカルの機体フォルダツリー
    /// </summary>
    public FileTreeItemViewModel LocalAircraftRoot {
        get => _localAircraftRoot;
        set => SetProperty( ref _localAircraftRoot, value );
    }

    /// <summary>
    /// ローカルのDLCキャンペーンフォルダツリー
    /// </summary>
    public FileTreeItemViewModel LocalDlcCampaignRoot {
        get => _localDlcCampaignRoot;
        set => SetProperty( ref _localDlcCampaignRoot, value );
    }

    /// <summary>
    /// 選択中のタブインデックス
    /// </summary>
    public int SelectedTabIndex {
        get => _selectedTabIndex;
        set => SetProperty( ref _selectedTabIndex, value );
    }

    /// <summary>
    /// ViewModel の新しいインスタンスを生成する
    /// </summary>
    /// <param name="regionManager">ナビゲーション管理用の IRegionManager</param>
    /// <param name="repositoryService">リポジトリサービス</param>
    /// <param name="fileService">ファイルサービス</param>
    /// <param name="appSettingsService">アプリ設定サービス</param>
    public DownloadViewModel(
        IRegionManager regionManager,
        IRepositoryService repositoryService,
        IFileService fileService,
        IAppSettingsService appSettingsService
    ) {
        _regionManager = regionManager;
        _repositoryService = repositoryService;
        _fileService = fileService;
        _appSettingsService = appSettingsService;
    }

    private async void OnFetch() {
        var trees = await _repositoryService.GetRepositoryTreeAsync();
        var root = new RepoTree
        {
            Name = string.Empty,
            AbsolutePath = string.Empty,
            IsDirectory = true,
            Children = trees
        };
        var aircraft = FindRepoTree( root, "DCSWorld/Mods/aircraft" )
            ?? new RepoTree { Name = string.Empty, AbsolutePath = string.Empty, IsDirectory = true };
        var dlc = FindRepoTree( root, "DCSWorld/Mods/campaigns" )
            ?? new RepoTree { Name = string.Empty, AbsolutePath = string.Empty, IsDirectory = true };
        RepoAircraftTree = new RepoTreeItemViewModel( aircraft );
        RepoDlcCampaignTree = new RepoTreeItemViewModel( dlc );
        RefreshLocalAircraftTree();
        RefreshLocalDlcCampaignTree();
    }

    private void OnLoadLocalTree( FileTreeItemViewModel node ) {
        if(node == null) return;
        node.UpdateChildren( _fileService );
    }

    private void RefreshLocalAircraftTree() {
        var path = _appSettingsService.SourceAircraftDir;
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

    private void RefreshLocalDlcCampaignTree() {
        var path = _appSettingsService.SourceDlcCampaignDir;
        if(string.IsNullOrEmpty( path ) || !Directory.Exists( path )) {
            var emptyRoot = new FileTree { Name = path, AbsolutePath = path, IsDirectory = true };
            LocalDlcCampaignRoot = new FileTreeItemViewModel( emptyRoot );
            return;
        }
        FileTree tree = _fileService.GetFileTree( path );
        var root = new FileTreeItemViewModel( tree );
        root.UpdateChildren( _fileService );
        LocalDlcCampaignRoot = root;
    }

    private static RepoTree? FindRepoTree( RepoTree root, string path ) {
        var parts = path.Split( '/' );
        var current = root;
        foreach(string part in parts) {
            RepoTree? next = null;
            foreach(var child in current.Children) {
                if(child.Name == part) {
                    next = child;
                    break;
                }
            }
            if(next == null) {
                return null;
            }
            current = next;
        }
        return current;
    }

    /// <summary>
    /// ナビゲーション後の処理を行う
    /// </summary>
    /// <param name="navigationContext">ナビゲーションコンテキスト</param>
    public void OnNavigatedTo( NavigationContext navigationContext ) {
        RefreshLocalAircraftTree();
        RefreshLocalDlcCampaignTree();
    }

    private async void OnDownload() {
        RepoTreeItemViewModel root;
        if(SelectedTabIndex == 0) {
            // Aircraft tab
            root = RepoAircraftTree;
        }
        else if(SelectedTabIndex == 1) {
            // DLC Campaign tab
            root = RepoDlcCampaignTree;
        }
        else {
            return;
        }

        if(root == null) {
            return;
        }

        var items = root.GetCheckedFiles();
        foreach(var item in root.GetCheckedFiles()) {
            byte[] data = await _repositoryService.GetFileAsync( item.AbsolutePath );
            var savePath = Path.Combine( _appSettingsService.TranslateFileDir, item.AbsolutePath );
            await _fileService.SaveAsync( savePath, data );
            Debug.WriteLine( $"書き込み完了: {savePath}" );
        }
    }

    private void OnResetCheck() {
        RepoAircraftTree?.SetCheckedRecursive( false );
        RepoDlcCampaignTree?.SetCheckedRecursive( false );
    }

    /// <summary>
    /// ナビゲーション前の処理を行う
    /// </summary>
    /// <param name="navigationContext">ナビゲーションコンテキスト</param>
    public void OnNavigatedFrom( NavigationContext navigationContext ) { }

    /// <summary>
    /// ナビゲーションターゲットかどうかを示す
    /// </summary>
    /// <param name="navigationContext">ナビゲーションコンテキスト</param>
    /// <returns>常に true</returns>
    public bool IsNavigationTarget( NavigationContext navigationContext )
        => true;

    private void OnOpenSettings()
        => _regionManager.RequestNavigate( Regions.Download, PageKeys.Settings );
}
