using System.IO;

using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Share.Contracts.Services;
using DcsTranslateTool.Share.Models;
using DcsTranslateTool.Win.Constants;
using DcsTranslateTool.Win.Contracts.Services;

namespace DcsTranslateTool.Win.ViewModels;

/// <summary>
/// DownloadPageの表示ロジックを保持する ViewModel
/// </summary>
/// <param name="appSettingsService">アプリ設定サービス</param>
/// <param name="regionManager">ナビゲーション管理用の IRegionManager</param>
/// <param name="repositoryService">リポジトリサービス</param>
/// <param name="fileService">ファイルサービス</param>
public class DownloadViewModel(
    IAppSettingsService appSettingsService,
    IRegionManager regionManager,
    IRepositoryService repositoryService,
    IFileService fileService
    ) : BindableBase, INavigationAware {
    #region Fields

    private RepoTreeItemViewModel? _repoAircraftTree;
    private RepoTreeItemViewModel? _repoDlcCampaignsTree;
    private int _selectedTabIndex;

    private DelegateCommand? _openSettingsCommand;
    private DelegateCommand? _fetchCommand;
    private DelegateCommand? _downloadCommand;
    private DelegateCommand<FileTreeItemViewModel>? _loadLocalTreeCommand;
    private DelegateCommand? _resetCheckCommand;

    #endregion

    #region Properties

    /// <summary>
    /// リポジトリの機体フォルダツリー
    /// </summary>
    public RepoTreeItemViewModel? RepoAircraftTree {
        get => _repoAircraftTree;
        set => SetProperty( ref _repoAircraftTree, value );
    }

    /// <summary>
    /// リポジトリのDLCキャンペーンフォルダツリー
    /// </summary>
    public RepoTreeItemViewModel? RepoDlcCampaignsTree {
        get => _repoDlcCampaignsTree;
        set => SetProperty( ref _repoDlcCampaignsTree, value );
    }

    /// <summary>
    /// 選択中のタブインデックス
    /// </summary>
    public int SelectedTabIndex {
        get => _selectedTabIndex;
        set => SetProperty( ref _selectedTabIndex, value );
    }

    #endregion

    #region Commands

    /// <summary>
    /// 設定画面を開くコマンド
    /// </summary>
    public DelegateCommand OpenSettingsCommand => _openSettingsCommand ??= new DelegateCommand( OnOpenSettings );

    /// <summary>
    /// リポジトリツリーを取得するコマンド
    /// </summary>
    public DelegateCommand FetchCommand => _fetchCommand ??= new DelegateCommand( OnFetch );

    /// <summary>
    /// ファイルをダウンロードするコマンド
    /// </summary>
    public DelegateCommand DownloadCommand => _downloadCommand ??= new DelegateCommand( OnDownload );

    /// <summary>
    /// ローカルツリーを読み込むコマンド
    /// </summary>
    public DelegateCommand<FileTreeItemViewModel> LoadLocalTreeCommand =>
        _loadLocalTreeCommand ??= new DelegateCommand<FileTreeItemViewModel>( OnLoadLocalTree );

    /// <summary>
    /// チェック状態をリセットするコマンド
    /// </summary>
    public DelegateCommand ResetCheckCommand => _resetCheckCommand ??= new DelegateCommand( OnResetCheck );

    #endregion

    #region Methods

    /// <summary>
    /// 設定ページに遷移する
    /// </summary>
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
    /// ナビゲーションターゲットかどうかを示す
    /// </summary>
    /// <param name="navigationContext">ナビゲーションコンテキスト</param>
    /// <returns>常に true</returns>
    public bool IsNavigationTarget( NavigationContext navigationContext )
        => true;

    private void OnOpenSettings() => regionManager.RequestNavigate( Regions.Main, PageKeys.Settings );

    private async void OnFetch() {
        var trees = await repositoryService.GetRepositoryTreeAsync();
        var root = new RepoTree
        {
            Name = string.Empty,
            AbsolutePath = string.Empty,
            IsDirectory = true,
            Children = trees
        };
        var aircraft = FindRepoTree( root, "DCSWorld/Mods/aircraft" )
            ?? new RepoTree { Name = string.Empty, AbsolutePath = string.Empty, IsDirectory = true };
        var dlcCampaigns = FindRepoTree( root, "DCSWorld/Mods/campaigns" )
            ?? new RepoTree { Name = string.Empty, AbsolutePath = string.Empty, IsDirectory = true };
        RepoAircraftTree = new RepoTreeItemViewModel( aircraft );
        RepoDlcCampaignsTree = new RepoTreeItemViewModel( dlcCampaigns );
    }

    private async void OnDownload() {
        RepoTreeItemViewModel? root;
        switch(SelectedTabIndex) {
            // Aircraft tab
            case 0:
                root = RepoAircraftTree;
                break;
            // DLC Campaign tab
            case 1:
                root = RepoDlcCampaignsTree;
                break;
            default:
                return;
        }
        if(root == null) return;
        foreach(var item in root.GetCheckedFiles()) {
            byte[] data = await repositoryService.GetFileAsync( item.AbsolutePath );
            var savePath = Path.Combine( appSettingsService.TranslateFileDir, item.AbsolutePath );
            await fileService.SaveAsync( savePath, data );
        }
    }

    private void OnLoadLocalTree( FileTreeItemViewModel node ) {
        if(node == null) return;
        node.UpdateChildren( fileService );
    }

    private void OnResetCheck() {
        RepoAircraftTree?.SetCheckedRecursive( false );
        RepoDlcCampaignsTree?.SetCheckedRecursive( false );
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

    #endregion
}
