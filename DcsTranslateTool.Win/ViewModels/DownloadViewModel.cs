using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

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

/// <summary>
/// DownloadPageの表示ロジックを保持する ViewModel
/// </summary>
public class DownloadViewModel : BindableBase, INavigationAware {
    #region Fields

    private readonly IAppSettingsService _appSettingsService;
    private readonly IRegionManager _regionManager;
    private readonly IRepositoryService _repositoryService;
    private readonly IFileService _fileService;
    private readonly IFileEntryService _fileEntryService;
    private readonly IDispatcherService _dispatcherService;
    private readonly ISystemService _systemService;

    private IReadOnlyList<FileEntry> _localEntries = [];
    private IReadOnlyList<FileEntry> _repoEntries = [];

    private ObservableCollection<DownloadTabItemViewModel> _tabs = [];
    private int _selectedTabIndex = 0;

    private DelegateCommand? _openSettingsCommand;
    private AsyncDelegateCommand? _fetchCommand;
    private AsyncDelegateCommand? _downloadCommand;
    private AsyncDelegateCommand? _applyCommand;
    private DelegateCommand? _openDirectoryCommand;

    #endregion

    public DownloadViewModel(
        IAppSettingsService appSettingsService,
        IRegionManager regionManager,
        IRepositoryService repositoryService,
        IFileService fileService,
        IFileEntryService fileEntryService,
        IDispatcherService dispatcherService,
        ISystemService systemService
    ) {
        _appSettingsService = appSettingsService;
        _regionManager = regionManager;
        _repositoryService = repositoryService;
        _fileService = fileService;
        _fileEntryService = fileEntryService;
        _dispatcherService = dispatcherService;
        _systemService = systemService;

        _fileEntryService.EntriesChanged += entries =>
            _dispatcherService.InvokeAsync( () => {
                _localEntries = entries;
                return OnRefleshTabs();
            }
        );

        Filter.FiltersChanged += ( _, _ ) => ApplyFilter();
    }

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
    public ObservableCollection<DownloadTabItemViewModel> Tabs {
        get => _tabs;
        set => SetProperty( ref _tabs, value );
    }

    /// <summary>
    /// ファイルのフィルタ状態を取得するプロパティである。
    /// </summary>
    public FilterViewModel Filter { get; } = new();

    #endregion

    #region Commands

    /// <summary>
    /// 設定画面を開くコマンド
    /// </summary>
    public DelegateCommand OpenSettingsCommand => _openSettingsCommand ??= new DelegateCommand( OnOpenSettings );

    /// <summary>
    /// リポジトリツリーを取得するコマンド
    /// </summary>
    public AsyncDelegateCommand FetchCommand => _fetchCommand ??= new AsyncDelegateCommand( OnFetchAsync );

    /// <summary>
    /// ファイルをダウンロードするコマンド
    /// </summary>
    public AsyncDelegateCommand DownloadCommand => _downloadCommand ??= new AsyncDelegateCommand( OnDownloadAsync );

    /// <summary>
    /// リポジトリ上のファイルをmizファイルに適用するコマンド
    /// </summary>
    public AsyncDelegateCommand ApplyCommand => _applyCommand ??= new AsyncDelegateCommand( OnApplyAsync );

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
    public void OnNavigatedFrom( NavigationContext navigationContext ) {
        _fileEntryService.Dispose();
    }

    /// <summary>
    /// ナビゲーション後の処理を行う
    /// </summary>
    /// <param name="navigationContext">ナビゲーションコンテキスト</param>
    public void OnNavigatedTo( NavigationContext navigationContext ) {
        Debug.WriteLine( "DownloadViewModel.OnNavigatedTo called" );
        _fileEntryService.Watch( _appSettingsService.TranslateFileDir );
        _ = OnFetchAsync();
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
    /// リポジトリとローカルストレージのファイルエントリを同期し、
    /// それらをマージしてタブビューモデルを更新することで、タブをリフレッシュする。
    /// </summary>
    /// <remarks>
    /// <para>
    /// このメソッドは、リポジトリとローカルストレージの両方からファイルエントリを取得し、
    /// それらを統合された構造にマージし、現在の状態を反映するようにタブビューモデルを更新します。
    /// </para>
    /// <para>
    /// リポジトリまたはローカルファイルの取得のいずれかが失敗した場合、
    /// このメソッドはエラーをログに記録し、タブに変更を加えることなく終了します。
    /// </para>
    /// </remarks>
    /// <returns>A task that represents the asynchronous operation of refreshing the tabs.</returns>
    private Task OnRefleshTabs() {
        Debug.WriteLine( "DownloadViewModel.OnRefleshTabs called" );
        var tabIndex = SelectedTabIndex;

        // リポジトリとローカルの FileEntry をマージする
        var entries = FileEntryComparisonHelper.Merge(_localEntries, _repoEntries);

        IFileEntryViewModel rootVm = new FileEntryViewModel( new FileEntry( "", "", true ), ChangeTypeMode.Download );
        foreach(var entry in entries) AddFileEntryToFileEntryViewModel( rootVm, entry );

        var tabs = Enum.GetValues<RootTabType>().Select(tabType => {
            IFileEntryViewModel? target = rootVm;
            foreach(var name in tabType.GetRepoDirRoot()) {
                target = target?.Children.FirstOrDefault( c => c?.Name == name );
                if(target is null) break;
            }

            return new DownloadTabItemViewModel(tabType, target ?? new FileEntryViewModel(new FileEntry("null","",false), ChangeTypeMode.Download));
        });
        Tabs.Clear();
        Tabs = [.. tabs];
        SelectedTabIndex = tabIndex;
        ApplyFilter();
        Debug.WriteLine( "DownloadViewModel.OnRefleshTabs finished" );
        return Task.CompletedTask;
    }

    /// <summary>
    /// リポジトリからツリーを取得する
    /// </summary>
    private async Task OnFetchAsync() {
        Debug.WriteLine( "DownloadViewModel.OnFetchAsync called" );
        var repoResult = await _repositoryService.GetFileEntriesAsync();
        if(repoResult.IsFailed) {
            foreach(var err in repoResult.Errors) Console.WriteLine( $"DownloadViewModel.OnFetch:: {err.Message}" );
        }
        _repoEntries = [.. repoResult.Value];
        _localEntries = await _fileEntryService.GetEntriesAsync();
        await OnRefleshTabs();
        Debug.WriteLine( $"DownloadViewModel.RefleshTabs: {Tabs.Count} tabs loaded." );
        Debug.WriteLine( "DownloadViewModel.OnFetchAsync finished" );
    }

    /// <summary>
    /// チェック状態のファイルをダウンロードする
    /// </summary>
    private async Task OnDownloadAsync() {
        var targetEntries = _tabs[SelectedTabIndex].GetCheckedEntries();
        foreach(var entry in targetEntries) {
            if(entry.IsDirectory) continue;
            var result = await _repositoryService.GetFileAsync( entry.Path );
            if(result.IsFailed) {
                // TODO: エラーハンドリング
                return;
            }
            byte[] data = result.Value;
            var savePath = Path.Join( _appSettingsService.TranslateFileDir, entry.Path );
            await _fileService.SaveAsync( savePath, data );
        }
    }

    private async Task OnApplyAsync() {
        // TODO: 選択されたファイルをmizファイルに適用する処理を実装
        await Task.Delay( 100 );
    }

    /// <summary>
    /// 翻訳ファイルの管理ディレクトリをエクスプローラーで開く
    /// </summary>
    private void OnOpenDirectory() {
        _systemService.OpenDirectory( _appSettingsService.TranslateFileDir );
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
                next = new FileEntryViewModel( new FileEntry( part, absolutePath, true ), ChangeTypeMode.Download );
                current.Children.Add( next );
            }
            current = next;
        }

        var last = parts[^1];
        if(!current.Children.Any( c => c?.Name == last )) {
            current.Children.Add( new FileEntryViewModel( new FileEntry( last, entry.Path, entry.IsDirectory, entry.LocalSha, entry.RepoSha ), ChangeTypeMode.Download ) );
        }
    }

    #endregion
}