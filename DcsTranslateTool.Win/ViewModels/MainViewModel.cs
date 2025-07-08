using System.IO;
using System.Linq;
using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Share.Contracts.Services;
using DcsTranslateTool.Share.Models;
using DcsTranslateTool.Win.Constants;

namespace DcsTranslateTool.Win.ViewModels;

/// <summary>
/// メイン画面の表示ロジックを保持する ViewModel
/// </summary>
public class MainViewModel : BindableBase {
    private readonly IRegionManager _regionManager;
    private readonly IRepositoryService _repositoryService;
    private readonly IFileService _fileService;
    private readonly IAppSettingsService _appSettingsService;

    private DelegateCommand _openSettingsCommand;
    private DelegateCommand _fetchCommand;
    private DelegateCommand<FileTreeItemViewModel> _loadLocalTreeCommand;

    private RepoTree _repoAircraftTree;
    private RepoTree _repoDlcCampaignTree;
    private FileTreeItemViewModel _localAircraftRoot;

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
    /// リポジトリの機体フォルダツリー
    /// </summary>
    public RepoTree RepoAircraftTree {
        get => _repoAircraftTree;
        set => SetProperty( ref _repoAircraftTree, value );
    }

    /// <summary>
    /// リポジトリのDLCキャンペーンフォルダツリー
    /// </summary>
    public RepoTree RepoDlcCampaignTree {
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
    /// ViewModel の新しいインスタンスを生成する
    /// </summary>
    /// <param name="regionManager">ナビゲーション管理用の IRegionManager</param>
    /// <param name="repositoryService">リポジトリサービス</param>
    /// <param name="fileService">ファイルサービス</param>
    /// <param name="appSettingsService">アプリ設定サービス</param>
    public MainViewModel(
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
        var root = new RepoTree {
            Name = string.Empty,
            AbsolutePath = string.Empty,
            IsDirectory = true,
            Children = trees
        };
        RepoAircraftTree = FindRepoTree( root, "DCSWorld/Mods/aircraft" ) ?? new RepoTree{ Name="", AbsolutePath="", IsDirectory=true };
        RepoDlcCampaignTree = FindRepoTree( root, "DCSWorld/Mods/campaigns" ) ?? new RepoTree{ Name="", AbsolutePath="", IsDirectory=true };
        RefreshLocalAircraftTree();
    }

    private void OnLoadLocalTree( FileTreeItemViewModel node ) {
        if(node == null) return;
        node.UpdateChildren( _fileService );
    }

    private void RefreshLocalAircraftTree() {
        var path = _appSettingsService.SourceAircraftDir;
        if(string.IsNullOrEmpty( path ) || !Directory.Exists( path )) {
            LocalAircraftRoot = new FileTreeItemViewModel( new FileTree{ Name=path, AbsolutePath=path, IsDirectory=true } );
            return;
        }
        FileTree tree = _fileService.GetFileTree( path );
        LocalAircraftRoot = new FileTreeItemViewModel( tree );
    }

    private static RepoTree? FindRepoTree( RepoTree root, string path ) {
        var parts = path.Split( '/' );
        var current = root;
        foreach(string part in parts) {
            current = current.Children.FirstOrDefault( c => c.Name == part );
            if(current == null) return null;
        }
        return current;
    }

    private void OnOpenSettings()
        => _regionManager.RequestNavigate( Regions.Main, PageKeys.Settings );
}
