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

public class UploadViewModel(
    IAppSettingsService appSettingsService,
    IRegionManager regionManager,
    IDialogService dialogService,
    IRepositoryService repositoryService,
    IFileEntryService fileEntryService
    ) : BindableBase, INavigationAware {
    #region Fields

    private ObservableCollection<UploadTabItemViewModel> _tabs = [];
    private int _selectedTabIndex = 0;
    private bool _isCreatePullRequestDialogButtonEnabled = false;

    private DelegateCommand? _openSettingsCommand;
    private DelegateCommand? _openCreatePullRequestDialogCommand;

    #endregion

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
    private void OnOpenSettings() => regionManager.RequestNavigate( Regions.Main, PageKeys.Settings );

    /// <summary>
    /// Pull Request作成ダイアログを開く
    /// </summary>
    private void OnOpenCreatePullRequestDialog() {
        IsCreatePullRequestDialogButtonEnabled = false;

        var checkedEntries = Tabs[SelectedTabIndex]
            .Root
            .GetCheckedModelRecursice();

        var parameters = new DialogParameters {
            { "files", checkedEntries }
        };

        dialogService.ShowDialog( PageKeys.CreatePullRequestDialog, parameters, r => {
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
        UpdateCreatePullRequestDialogButton();
        Debug.WriteLine( $"UploadViewModel.RefleshTabs: {Tabs.Count} tabs loaded." );
    }

    ///// <summary>
    ///// ファイルエントリの選択状態変更時に呼び出される
    ///// </summary>
    ///// <param name="sender">イベント送信元</param>
    ///// <param name="_">未使用</param>
    //private void OnFileEntrySelectedChanged( object? sender, CheckState _ ) => UpdateCreatePullRequestDialogButton();

    ///// <summary>
    ///// ファイルエントリの選択状態変更イベントを購読する
    ///// </summary>
    ///// <param name="node">対象のノード</param>
    //private void SubscribeSelectionChanged( IFileEntryViewModel node ) {
    //    if(node is FileEntryViewModel concrete) {
    //        concrete.CheckStateChanged += OnFileEntrySelectedChanged;
    //    }
    //    foreach(var child in node.Children) {
    //        if(child is not null) SubscribeSelectionChanged( child );
    //    }
    //}

    /// <summary>
    /// Pull Request 作成ボタンの有効状態を更新する
    /// </summary>
    private void UpdateCreatePullRequestDialogButton() {
        if(Tabs.Count == 0) {
            IsCreatePullRequestDialogButtonEnabled = false;
            return;
        }

        IsCreatePullRequestDialogButtonEnabled = Tabs[SelectedTabIndex].Root.CheckState.IsSelectedLike();
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