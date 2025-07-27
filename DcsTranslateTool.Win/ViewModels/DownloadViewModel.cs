using System.Collections.ObjectModel;
using System.IO;

using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.Constants;
using DcsTranslateTool.Win.Contracts.Services;

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
    #region Fields

    private ObservableCollection<DownloadTabItemViewModel> _tabs = [];
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
        IReadOnlyList<RepoEntry> entries = await repositoryService.GetRepositoryEntryAsync();
        RepoEntryViewModel rootVm = new( new RepoEntry( "", "", true ) );
        foreach(var entry in entries) AddRepoEntryToRepoEntryViewModel( rootVm, entry );
        var modVM = rootVm
            .Children.FirstOrDefault(c => c.Name == "DCSWorld")?
            .Children.FirstOrDefault(c => c.Name == "Mods");

        ResetTabs();
        var aircraftVM = modVM?.Children.FirstOrDefault( c => c.Name == "aircraft" );
        var dlcCampaignsVM = modVM?.Children.FirstOrDefault( c => c.Name == "campaigns" );
        if(aircraftVM is not null) Tabs.FindFirst( t => t.Title == "Aircraft" )!.Root = aircraftVM;
        if(dlcCampaignsVM is not null) Tabs.FindFirst( t => t.Title == "DLC Campaigns" )!.Root = dlcCampaignsVM;
    }

    /// <summary>
    /// チェック状態のファイルをダウンロードする
    /// </summary>
    private async void OnDownload() {
        var targetEntries = _tabs[SelectedTabIndex].GetCheckedEntries();
        foreach(var entry in targetEntries) {
            if(entry.IsDirectory) continue;
            byte[] data = await repositoryService.GetFileAsync( entry.AbsolutePath );
            var savePath = Path.Join( appSettingsService.TranslateFileDir, entry.AbsolutePath );
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
    /// Tabsを初期化
    /// </summary>
    private void ResetTabs() {
        Tabs = [
            new DownloadTabItemViewModel("Aircraft"     , new RepoEntryViewModel(new RepoEntry("", "", true) ) ),
            new DownloadTabItemViewModel("DLC Campaigns", new RepoEntryViewModel(new RepoEntry("", "", true) ) )
        ];
    }

    /// <summary>
    /// <see cref="RepoEntry"/>を<see cref="RepoEntryViewModel"/>に変換し、ツリー構造に追加する"/>
    /// </summary>
    /// <param name="root">ルートViewModel</param>
    /// <param name="entry">追加するエントリー</param>
    private static void AddRepoEntryToRepoEntryViewModel( RepoEntryViewModel root, RepoEntry entry ) {
        string[] parts = entry.AbsolutePath.Split( "/", StringSplitOptions.RemoveEmptyEntries );
        if(parts.IsNullOrEmpty()) return;
        RepoEntryViewModel current = root;
        string absolutePath = "";
        // ディレクトリが確定している場所までディレクトリを作成していく
        foreach(string part in parts[..^1]) {
            absolutePath += absolutePath.Length == 0 ? part : "/" + part;
            var next = current.Children.FirstOrDefault(c => c.Name == part && c.IsDirectory);
            if(next is null) {
                next = new RepoEntryViewModel( new RepoEntry( part, absolutePath, true ) );
                current.Children.Add( next );
            }
            current = next;
        }

        var last = parts[^1];
        if(!current.Children.Any( c => c.Name == last )) {
            current.Children.Add( new RepoEntryViewModel( new RepoEntry( last, entry.AbsolutePath, entry.IsDirectory ) ) );
        }
    }
    #endregion
}
