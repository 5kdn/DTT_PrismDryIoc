using System.Collections.ObjectModel;
using System.IO;

using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Share.Contracts.Services;
using DcsTranslateTool.Share.Models;
using DcsTranslateTool.Win.Constants;
using DcsTranslateTool.Win.Contracts.Services;
using DcsTranslateTool.Win.Models;

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

    private ObservableCollection<DownloadTabItem> _tabs = [];
    private int _selectedTabIndex;

    private DelegateCommand? _openSettingsCommand;
    private DelegateCommand? _fetchCommand;
    private DelegateCommand? _downloadCommand;
    private DelegateCommand? _applyCommand;
    private DelegateCommand? _resetCheckCommand;
    private DelegateCommand? _openDirectoryCommand;

    #endregion

    #region Properties

    /// <summary>
    /// 選択中のタブインデックス
    /// </summary>
    public int SelectedTabIndex {
        get => _selectedTabIndex;
        set => SetProperty( ref _selectedTabIndex, value );
    }

    ///<summary>
    ///全てのタブ情報を取得する
    /// </summary>
    public ObservableCollection<DownloadTabItem> Tabs {
        get => _tabs;
        set => SetProperty( ref _tabs, value );
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
    /// リポジトリ上のファイルをmizファイルに適用するコマンド
    /// </summary>
    public DelegateCommand ApplyCommand => _applyCommand ??= new DelegateCommand( OnApply );

    /// <summary>
    /// チェック状態をリセットするコマンド
    /// </summary>
    public DelegateCommand ResetCheckCommand => _resetCheckCommand ??= new DelegateCommand( OnResetCheck );

    /// <summary>
    /// 翻訳ファイルを管理するディレクトリを開くコマンド
    /// </summary>
    public DelegateCommand OpenDirectoryCommand => _openDirectoryCommand ??= new DelegateCommand( OnOpenDirectory );

    #endregion

    #region Methods

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
        RefleshTabs();
    }

    /// <summary>
    /// ナビゲーションターゲットかどうかを示す
    /// </summary>
    /// <param name="navigationContext">ナビゲーションコンテキスト</param>
    /// <returns>常に true</returns>
    public bool IsNavigationTarget( NavigationContext navigationContext )
        => true;

    /// <summary>
    /// 設定ページに遷移する
    /// </summary>
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
        //RepoAircraftTree = new RepoTreeItemViewModel( aircraft );
        //RepoDlcCampaignsTree = new RepoTreeItemViewModel( dlcCampaigns );
        //_tabs[0].RepoTree = RepoAircraftTree;
        //_tabs[1].RepoTree = RepoDlcCampaignsTree;
        Tabs[0].RepoTree = new RepoTreeItemViewModel( aircraft );
        Tabs[1].RepoTree = new RepoTreeItemViewModel( dlcCampaigns );
    }

    private async void OnDownload() {
        var root = SelectedTabIndex >=0 && SelectedTabIndex < _tabs.Count
            ? _tabs[SelectedTabIndex].RepoTree
            : null;
        if(root == null) return;
        foreach(var item in root.GetCheckedFiles()) {
            byte[] data = await repositoryService.GetFileAsync( item.AbsolutePath );
            var savePath = Path.Combine( appSettingsService.TranslateFileDir, item.AbsolutePath );
            await fileService.SaveAsync( savePath, data );
        }
    }

    private void OnApply() {
        // TODO: 選択されたファイルをmizファイルに適用する処理を実装
    }

    private void OnResetCheck() {
        foreach(var tab in _tabs) {
            tab.RepoTree?.SetCheckedRecursive( false );
        }
    }

    private void OnOpenDirectory() {
        // TODO: TranslateFileDirを開く処理を実装
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
    /// Tabsを初期化
    /// </summary>
    private void RefleshTabs() {
        Tabs = [
            new DownloadTabItem("Aircraft"     , new RepoTreeItemViewModel() ),
            new DownloadTabItem("DLC Campaigns", new RepoTreeItemViewModel() )
        ];
    }

    #endregion
}
