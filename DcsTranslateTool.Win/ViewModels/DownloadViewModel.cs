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
/// <param name="appSettingsService">アプリ設定サービス</param>
/// <param name="regionManager">ナビゲーション管理用の IRegionManager</param>
/// <param name="repositoryService">リポジトリサービス</param>
/// <param name="fileService">ファイルサービス</param>
public class DownloadViewModel(
    IAppSettingsService appSettingsService,
    IRegionManager regionManager,
    IRepositoryService repositoryService,
    IFileService fileService,
    IFileEntryService fileEntryService
    ) : BindableBase, INavigationAware {
    #region Fields

    private ObservableCollection<DownloadTabItemViewModel> _tabs = [];
    private int _selectedTabIndex = 0;

    private DelegateCommand? _openSettingsCommand;
    private AsyncDelegateCommand? _fetchCommand;
    private AsyncDelegateCommand? _downloadCommand;
    private AsyncDelegateCommand? _applyCommand;
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
    public ObservableCollection<DownloadTabItemViewModel> Tabs {
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
        Debug.WriteLine( "DownloadViewModel.OnNavigatedTo called" );
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
    private void OnOpenSettings() => regionManager.RequestNavigate( Regions.Main, PageKeys.Settings );

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
    private async Task OnRefleshTabsAsync() {
        Debug.WriteLine( "DownloadViewModel.OnRefleshTabs called" );
        var tabIndex = SelectedTabIndex;

        // リポジトリとローカルの FileEntry を取得する
        var repoResult = await repositoryService.GetFileEntriesAsync();
        if(repoResult.IsFailed) {
            foreach(var error in repoResult.Errors) Console.WriteLine( $"DownloadViewModel.OnFetch:: {error.Message}" );
            return;
        }
        var localResult = fileEntryService.GetChildrenRecursive(appSettingsService.TranslateFileDir);
        if(localResult.IsFailed) {
            foreach(var error in localResult.Errors) Console.WriteLine( $"DownloadViewModel.OnFetch:: {error.Message}" );
            return;
        }

        // リポジトリとローカルの FileEntry をマージする
        IEnumerable<FileEntry> repoEntries = repoResult.Value;
        IEnumerable<FileEntry> localEntries = localResult.Value;
        var newEntries = FileEntryComparisonHelper.Merge(localEntries, repoEntries);

        IFileEntryViewModel rootVm = new FileEntryViewModel( new FileEntry( "", "", true ) );
        foreach(var entry in newEntries) AddFileEntryToFileEntryViewModel( rootVm, entry );

        var tabs = Enum.GetValues<RootTabType>().Select(tabType => {
            IFileEntryViewModel? target = rootVm;
            foreach(var name in tabType.GetRepoDirRoot()) {
                target = target?.Children.FirstOrDefault( c => c?.Name == name );
                if(target is null) break;
            }

            return new DownloadTabItemViewModel(tabType, target ?? new FileEntryViewModel(new FileEntry("null","",false)));
        });
        Tabs.Clear();
        Tabs = [.. tabs];
        SelectedTabIndex = tabIndex;
        Debug.WriteLine( "DownloadViewModel.OnRefleshTabs finished" );
    }

    /// <summary>
    /// リポジトリからツリーを取得する
    /// </summary>
    private async Task OnFetchAsync() {
        Debug.WriteLine( "DownloadViewModel.OnFetch called" );
        await OnRefleshTabsAsync();
        Debug.WriteLine( $"DownloadViewModel.RefleshTabs: {Tabs.Count} tabs loaded." );
    }

    /// <summary>
    /// チェック状態のファイルをダウンロードする
    /// </summary>
    private async Task OnDownloadAsync() {
        var targetEntries = _tabs[SelectedTabIndex].GetCheckedEntries();
        foreach(var entry in targetEntries) {
            if(entry.IsDirectory) continue;
            var result = await repositoryService.GetFileAsync( entry.Path );
            if(result.IsFailed) {
                // TODO: エラーハンドリング
                return;
            }
            byte[] data = result.Value;
            var savePath = Path.Join( appSettingsService.TranslateFileDir, entry.Path );
            await fileService.SaveAsync( savePath, data );
        }
    }

    private async Task OnApplyAsync() {
        // TODO: 選択されたファイルをmizファイルに適用する処理を実装
        await Task.Delay( 100 );
    }

    private void OnResetCheck() {
        foreach(var tab in _tabs) {
            tab.SetCheckRecursive( false );
        }
    }

    private void OnOpenDirectory() {
        // TODO: TranslateFileDirを開く処理を実装
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