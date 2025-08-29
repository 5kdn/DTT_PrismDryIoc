using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Enums;
using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.Constants;
using DcsTranslateTool.Win.Contracts.Services;
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
    IFileService fileService
    ) : BindableBase, INavigationAware {

    Filters = [
        new FilterOptionViewModel("全て", null),
        new FilterOptionViewModel("DL済", FileChangeType.Unchanged),
        new FilterOptionViewModel("未DL", FileChangeType.Deleted),
        new FilterOptionViewModel("追加", FileChangeType.Added),
        new FilterOptionViewModel("変更/更新", FileChangeType.Modified),
    ];
    foreach(var f in Filters) f.CheckedChanged += OnFilterChanged;
    Filters[0].IsChecked = true;

    #region Fields

    private ObservableCollection<DownloadTabItemViewModel> _tabs = [];
    private int _selectedTabIndex;
    private bool _isUpdatingFilters;

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
    public ObservableCollection<DownloadTabItemViewModel> Tabs {
        get => _tabs;
        set => SetProperty( ref _tabs, value );
    }

    /// <summary>
    /// フィルタ項目を取得する。
    /// </summary>
    public ObservableCollection<FilterOptionViewModel> Filters { get; }

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
        Debug.WriteLine( "DownloadViewModel.OnNavigatedTo called" );
        OnFetch();
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

    /// <summary>
    /// リポジトリからツリーを取得する
    /// </summary>
    private async void OnFetch() {
        Debug.WriteLine( "DownloadViewModel.OnFetch called" );
        var result = await repositoryService.GetRepositoryEntryAsync();
        if(result.IsFailed) {
            // TODO: エラーハンドリング
            foreach(var error in result.Errors) Console.WriteLine( $"DownloadViewModel.OnFetch:: {error.Message}" );
            return;
        }
        IEnumerable<Entry> entries = result.Value;
        RepoEntryViewModel rootVm = new( new Entry( "", "", true ) );
        foreach(var entry in entries) AddRepoEntryToRepoEntryViewModel( rootVm, entry );

        ResetTabs();
        Enum.GetValues<RootTabType>().ForEach( tabType => {
            RepoEntryViewModel? target = tabType
                .GetRepoDirRoot()
                .Aggregate<string, RepoEntryViewModel?>(rootVm, (node, part) =>
                    node?.Children.FirstOrDefault( c => c.Name == part ) );
            if( target != null ) Tabs.FindFirst( t => t.TabType == tabType ).UpdateRoot( target );
        } );
        ApplyFilters();
    }

    /// <summary>
    /// チェック状態のファイルをダウンロードする
    /// </summary>
    private async void OnDownload() {
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

    private void OnApply() {
        // TODO: 選択されたファイルをmizファイルに適用する処理を実装
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
    /// フィルタ変更時に呼び出される。
    /// </summary>
    /// <param name="changed">変更されたフィルタ</param>
    private void OnFilterChanged( FilterOptionViewModel changed ) {
        if( _isUpdatingFilters ) return;
        _isUpdatingFilters = true;

        if( changed.ChangeType is null && changed.IsChecked ) {
            foreach( var f in Filters.Where( f => f.ChangeType is not null ) ) f.IsChecked = false;
        } else if( changed.ChangeType is not null && changed.IsChecked ) {
            var all = Filters.First( f => f.ChangeType is null );
            all.IsChecked = false;
        }

        if( Filters.All( f => !f.IsChecked ) ) {
            Filters.First( f => f.ChangeType is null ).IsChecked = true;
        }

        _isUpdatingFilters = false;
        ApplyFilters();
    }

    /// <summary>
    /// フィルタを各タブに適用する。
    /// </summary>
    private void ApplyFilters() {
        List<FileChangeType> active = Filters
            .Where( f => f.ChangeType is not null && f.IsChecked )
            .Select( f => f.ChangeType!.Value )
            .ToList();
        foreach( var tab in Tabs ) {
            tab.ApplyFilter( active );
        }
    }

    /// <summary>
    /// Tabsを初期化
    /// </summary>
    private void ResetTabs() {
        Tabs = [..Enum.GetValues<RootTabType>().Select(tabType=>
            new DownloadTabItemViewModel(tabType, new RepoEntryViewModel(new Entry("", "", true) ))
        )];
    }

    /// <summary>
    /// <see cref="Entry"/>を<see cref="RepoEntryViewModel"/>に変換し、ツリー構造に追加する。
    /// </summary>
    /// <param name="root">ルートViewModel</param>
    /// <param name="entry">追加するエントリー</param>
    private static void AddRepoEntryToRepoEntryViewModel( RepoEntryViewModel root, Entry entry ) {
        string[] parts = entry.Path.Split( "/", StringSplitOptions.RemoveEmptyEntries );
        if(parts.IsNullOrEmpty()) return;
        RepoEntryViewModel current = root;
        string absolutePath = "";
        // ディレクトリが確定している場所までディレクトリを作成していく
        foreach(string part in parts[..^1]) {
            absolutePath += absolutePath.Length == 0 ? part : "/" + part;
            var next = current.Children.FirstOrDefault(c => c.Name == part && c.IsDirectory);
            if(next is null) {
                next = new RepoEntryViewModel( new Entry( part, absolutePath, true ) );
                current.Children.Add( next );
            }
            current = next;
        }

        var last = parts[^1];
        if(!current.Children.Any( c => c.Name == last )) {
            current.Children.Add( new RepoEntryViewModel( new Entry( last, entry.Path, entry.IsDirectory, null, entry.RepoSha ) ) );
        }
    }
    #endregion
}