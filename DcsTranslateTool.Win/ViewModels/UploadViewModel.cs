using System.Collections.ObjectModel;
using System.Diagnostics;

using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Helpers;
using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.Constants;
using DcsTranslateTool.Win.Contracts.Services;
using DcsTranslateTool.Win.Contracts.ViewModels;
using DcsTranslateTool.Win.Enums;
using DcsTranslateTool.Win.Extensions;

using DryIoc.ImTools;

namespace DcsTranslateTool.Win.ViewModels;

public class UploadViewModel : BindableBase, INavigationAware {
    #region Fields

    private readonly IAppSettingsService _appSettingsService;
    private readonly IRegionManager _regionManager;
    private readonly IDialogService _dialogService;
    private readonly IRepositoryService _repositoryService;
    private readonly IFileEntryService _fileEntryService;

    private ObservableCollection<UploadTabItemViewModel> _tabs = [];
    private int _selectedTabIndex = 0;
    private bool _isCreatePullRequestDialogButtonEnabled = false;

    private DelegateCommand? _openSettingsCommand;
    private DelegateCommand? _openCreatePullRequestDialogCommand;

    #endregion

    public UploadViewModel(
        IAppSettingsService appSettingsService,
        IRegionManager regionManager,
        IDialogService dialogService,
        IRepositoryService repositoryService,
        IFileEntryService fileEntryService
    ) {
        _appSettingsService = appSettingsService;
        _regionManager = regionManager;
        _dialogService = dialogService;
        _repositoryService = repositoryService;
        _fileEntryService = fileEntryService;

        Filter.FiltersChanged += ( _, _ ) => ApplyFilter();
    }

    #region Properties

    /// <summary>
    /// 選択中のタブインデックス
    /// </summary>
    public int SelectedTabIndex {
        get => _selectedTabIndex;
        set {
            if(SetProperty( ref _selectedTabIndex, value )) UpdateCreatePullRequestDialogButton();
        }
    }

    ///<summary>
    ///全てのタブ情報を取得する
    /// </summary>
    public ObservableCollection<UploadTabItemViewModel> Tabs {
        get => _tabs;
        set => SetProperty( ref _tabs, value );
    }

    /// <summary>
    /// ファイルのフィルタ状態を取得するプロパティである。
    /// </summary>
    public FilterViewModel Filter { get; } = new();

    /// <summary>
    /// Pull Request 作成ダイアログを開くボタンが有効かどうかを示す
    /// </summary>
    public bool IsCreatePullRequestDialogButtonEnabled {
        get => _isCreatePullRequestDialogButtonEnabled;
        set => SetProperty( ref _isCreatePullRequestDialogButtonEnabled, value );
    }

    #endregion

    #region Commands

    /// <summary>
    /// 設定画面を開くコマンド
    /// </summary>
    public DelegateCommand OpenSettingsCommand => _openSettingsCommand ??= new DelegateCommand( OnOpenSettings );

    /// <summary>
    /// Pull Request 作成ダイアログを開くコマンド
    /// </summary>
    public DelegateCommand OpenCreatePullRequestDialogCommand =>
        _openCreatePullRequestDialogCommand ??= new DelegateCommand( OnOpenCreatePullRequestDialog );

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
        Debug.WriteLine( "UploadViewModel.OnNavigatedTo called" );
        _ = RefleshTabs();
    }

    /// <summary>
    /// ナビゲーションターゲットかどうかを示す
    /// </summary>
    /// <param name="navigationContext">ナビゲーションコンテキスト</param>
    /// <returns>常に true</returns>
    public bool IsNavigationTarget( NavigationContext navigationContext ) => true;

    #endregion

    #region Private Methods

    /// <summary>
    /// 設定ページに遷移する
    /// </summary>
    private void OnOpenSettings() => _regionManager.RequestNavigate( Regions.Main, PageKeys.Settings );

    /// <summary>
    /// Pull Request作成ダイアログを開く
    /// </summary>
    private void OnOpenCreatePullRequestDialog() {
        IsCreatePullRequestDialogButtonEnabled = false;

        var checkedEntries = Tabs[SelectedTabIndex]
            .Root
            .GetCheckedModelRecursive();

        var parameters = new DialogParameters {
            { "files", checkedEntries }
        };

        _dialogService.ShowDialog( PageKeys.CreatePullRequestDialog, parameters, r => {
            // ダイアログの処理
            if(r.Result == ButtonResult.OK) {
                // OKボタンが押された場合の処理
            }
            else if(r.Result == ButtonResult.Cancel) {
                // キャンセルボタンが押された場合の処理
            }
            // ダイアログ終了後処理
            UpdateCreatePullRequestDialogButton();
        } );
    }

    /// <summary>
    /// TabsをTranslateFileDirから初期化
    /// </summary>
    private async Task RefleshTabs() {
        Debug.WriteLine( "UploadViewModel.RefleshTabs called" );
        var tabIndex = SelectedTabIndex;

        // リポジトリとローカルの FileEntry を取得する
        var repoResult = await _repositoryService.GetFileEntriesAsync();
        if(repoResult.IsFailed) {
            foreach(var error in repoResult.Errors) Console.WriteLine( $"DownloadViewModel.OnFetch:: {error.Message}" );
            return;
        }
        var localResult = _fileEntryService.GetChildrenRecursive(_appSettingsService.TranslateFileDir);
        if(localResult.IsFailed) {
            foreach(var error in localResult.Errors) Console.WriteLine( $"DownloadViewModel.OnFetch:: {error.Message}" );
            return;
        }

        // リポジトリとローカルの FileEntry をマージする
        var entries = FileEntryComparisonHelper.Merge(localResult.Value, repoResult.Value);

        IFileEntryViewModel rootVm = new FileEntryViewModel( new FileEntry( "", "", true ) );
        foreach(var entry in entries) AddFileEntryToFileEntryViewModel( rootVm, entry );

        var tabs = Enum.GetValues<RootTabType>().Select(tabType => {
            IFileEntryViewModel? target = rootVm;
            foreach(var name in tabType.GetRepoDirRoot()) {
                target = target?.Children.FirstOrDefault( c => c?.Name == name );
                if(target is null) break;
            }

            return new UploadTabItemViewModel(tabType, target ?? new FileEntryViewModel(new FileEntry("null","",false)));
        });

        Tabs.Clear();
        Tabs = [.. tabs];
        SelectedTabIndex = tabIndex;
        ApplyFilter();
        UpdateCreatePullRequestDialogButton();
        Debug.WriteLine( $"UploadViewModel.RefleshTabs: {Tabs.Count} tabs loaded." );
    }

    /// <summary>
    /// Pull Request 作成ボタンの有効状態を更新する
    /// </summary>
    private void UpdateCreatePullRequestDialogButton() {
        if(Tabs.Count == 0) {
            IsCreatePullRequestDialogButtonEnabled = false;
            return;
        }

        IsCreatePullRequestDialogButtonEnabled = Tabs[SelectedTabIndex].Root.CheckState != false;
    }

    /// <summary>
    /// 現在のフィルタ条件を適用するメソッドである。
    /// </summary>
    private void ApplyFilter() {
        var types = this.Filter.GetActiveTypes().ToHashSet();
        foreach(var tab in _tabs) {
            ApplyFilterRecursive( tab.Root, types );
        }
    }

    /// <summary>
    /// 指定したノードへフィルタを再帰的に適用するメソッドである。
    /// </summary>
    /// <param name="node">対象のノード</param>
    /// <param name="types">有効な変更種別のセット</param>
    /// <returns>ノードを表示するべきとき true</returns>
    private static bool ApplyFilterRecursive( IFileEntryViewModel node, HashSet<FileChangeType?> types ) {
        bool visible = types.Contains( node.ChangeType );
        if(node.IsDirectory) {
            bool childVisible = false;
            foreach(var child in node.Children) {
                if(ApplyFilterRecursive( child, types )) childVisible = true;
            }
            visible |= childVisible;
        }
        node.IsVisible = visible;
        return visible;
    }


    /// <summary>
    /// <see cref="FileEntry"/>を<see cref="FileEntryViewModel"/>に変換し、ツリー構造に追加する。
    /// </summary>
    /// <param name="root">ルートViewModel</param>
    /// <param name="entry">追加するエントリー</param>
    private static void AddFileEntryToFileEntryViewModel( IFileEntryViewModel root, FileEntry entry ) {
        string[] parts = entry.Path.Split( "/", StringSplitOptions.RemoveEmptyEntries );
        if(parts.IsNullOrEmpty()) return;
        IFileEntryViewModel current = root;
        string absolutePath = "";
        // ディレクトリが確定している場所までディレクトリを作成していく
        foreach(string part in parts[..^1]) {
            absolutePath += absolutePath.Length == 0 ? part : "/" + part;
            var next = current.Children.FirstOrDefault(c => c?.Name == part && c.IsDirectory);
            if(next is null) {
                next = new FileEntryViewModel( new FileEntry( part, absolutePath, true ) );
                current.Children.Add( next );
            }
            current = next;
        }

        var last = parts[^1];
        if(!current.Children.Any( c => c?.Name == last )) {
            current.Children.Add( new FileEntryViewModel( new FileEntry( last, entry.Path, entry.IsDirectory, entry.LocalSha, entry.RepoSha ) ) );
        }
    }

    #endregion
}