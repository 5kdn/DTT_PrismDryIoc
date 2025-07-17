using System.IO;
using System.Linq;

using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Share.Contracts.Services;
using DcsTranslateTool.Share.Models;
using DcsTranslateTool.Win.Constants;
using DcsTranslateTool.Win.Contracts.Services;

namespace DcsTranslateTool.Win.ViewModels;

public class UploadViewModel : BindableBase, INavigationAware {
    private readonly IRegionManager _regionManager;
    private readonly IAppSettingsService _appSettingsService;
    private readonly IRepositoryService _repositoryService;
    private readonly IFileService _fileService;

    private DelegateCommand _openSettingsCommand;
    private DelegateCommand<FileTreeItemViewModel> _loadLocalTreeCommand;
    private DelegateCommand _uploadCommand;
    private DelegateCommand _resetCheckCommand;

    private FileTreeItemViewModel _translateRoot;

    public UploadViewModel(
        IRegionManager regionManager,
        IAppSettingsService appSettingsService,
        IRepositoryService repositoryService,
        IFileService fileService
    ) {
        _regionManager = regionManager;
        _appSettingsService = appSettingsService;
        _repositoryService = repositoryService;
        _fileService = fileService;
    }

    /// <summary>
    /// 設定画面を開くコマンド
    /// </summary>
    public DelegateCommand OpenSettingsCommand =>
        _openSettingsCommand ??= new DelegateCommand( OnOpenSettings );

    /// <summary>
    /// ローカルツリーを読み込むコマンド
    /// </summary>
    public DelegateCommand<FileTreeItemViewModel> LoadLocalTreeCommand =>
        _loadLocalTreeCommand ??= new DelegateCommand<FileTreeItemViewModel>( OnLoadLocalTree );

    /// <summary>
    /// ファイルをアップロードするコマンド
    /// </summary>
    public DelegateCommand UploadCommand =>
        _uploadCommand ??= new DelegateCommand( OnUpload );

    /// <summary>
    /// チェック状態をリセットするコマンド
    /// </summary>
    public DelegateCommand ResetCheckCommand =>
        _resetCheckCommand ??= new DelegateCommand( OnResetCheck );

    /// <summary>
    /// ローカルの翻訳ファイルツリー
    /// </summary>
    public FileTreeItemViewModel TranslateRoot {
        get => _translateRoot;
        set => SetProperty( ref _translateRoot, value );
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
        RefreshLocalTree();
    }

    private void OnLoadLocalTree( FileTreeItemViewModel node ) {
        if(node == null) return;
        node.UpdateChildren( _fileService );
    }

    private void RefreshLocalTree() {
        var path = _appSettingsService.TranslateFileDir;
        if(string.IsNullOrEmpty( path ) || !Directory.Exists( path )) {
            var emptyRoot = new FileTree { Name = path, AbsolutePath = path, IsDirectory = true };
            TranslateRoot = new FileTreeItemViewModel( emptyRoot );
            return;
        }
        FileTree tree = _fileService.GetFileTree( path );
        var root = new FileTreeItemViewModel( tree );
        root.UpdateChildren( _fileService );
        TranslateRoot = root;
    }

    private async void OnUpload() {
        var files = TranslateRoot?.GetCheckedFiles().ToList();
        if(files == null || files.Count == 0) return;
        var commitFiles = files.Select( f => new CommitFile {
            LocalPath = f.AbsolutePath,
            RepoPath = f.AbsolutePath.Replace( Path.DirectorySeparatorChar, '/' ),
            Operation = CommitOperation.AddOrUpdate
        } ).ToList();
        var branch = $"upload-{DateTime.Now:yyyyMMddHHmmss}";
        await _repositoryService.CreateBranchAsync( branch );
        await _repositoryService.CommitAsync( branch, commitFiles, "Upload" );
        await _repositoryService.CreatePullRequestAsync( branch, "Upload", "Upload" );
    }

    private void OnResetCheck() {
        TranslateRoot?.SetCheckedRecursive( false );
    }

    private void OnOpenSettings() => _regionManager.RequestNavigate( Regions.Upload, PageKeys.Settings );
}
