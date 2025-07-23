using System.Collections.ObjectModel;
using System.IO;

using DcsTranslateTool.Win.Constants;
using DcsTranslateTool.Win.Contracts.Services;
using DcsTranslateTool.Win.Contracts.ViewModels.Factories;

namespace DcsTranslateTool.Win.ViewModels;

public class UploadViewModel(
    IAppSettingsService appSettingsService,
    IRegionManager regionManager,
    IDialogService dialogService,
    IFileEntryViewModelFactory fileEntryViewModelFactory
    ) : BindableBase, INavigationAware {
    #region Fields

    private ObservableCollection<UploadTabItemViewModel> _tabs = [];
    private int _selectedTabIndex = 0;
    private bool _isCreatePullRequestDialogButtonEnabled = false;

    private DelegateCommand? _openSettingsCommand;
    private DelegateCommand? _openCreatePullRequestDialogCommand;
    private DelegateCommand<object?>? _loadLocalTreeCommand;

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
    /// ローカルツリーを取得するコマンド
    /// </summary>
    public DelegateCommand<object?> LoadLocalTreeCommand =>
        _loadLocalTreeCommand ??= new DelegateCommand<object?>( OnLoadLocalTree );

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
        RefleshTabs();
    }

    /// <summary>
    /// ナビゲーションターゲットかどうかを示す
    /// </summary>
    /// <param name="navigationContext">ナビゲーションコンテキスト</param>
    /// <returns>常に true</returns>
    public bool IsNavigationTarget( NavigationContext navigationContext ) => true;

    /// <summary>
    /// 設定ページに遷移する
    /// </summary>
    private void OnOpenSettings() => regionManager.RequestNavigate( Regions.Main, PageKeys.Settings );

    private void OnLoadLocalTree( object? parameter ) {
        if(parameter is not FileEntryViewModel node) return;

        if(node.IsChildrenLoaded) return;
        node.LoadChildren();
        node.IsChildrenLoaded = true;
    }

    /// <summary>
    /// Pull Request作成ダイアログを開く
    /// </summary>
    private void OnOpenCreatePullRequestDialog() {
        IsCreatePullRequestDialogButtonEnabled = false;
        var parameters = new DialogParameters(){
            {"files", new string[]{"files1", "files2", "files3" } }
        };
        dialogService.ShowDialog( PageKeys.CreatePullRequestDialog, parameters, r => {
            // ダイアログの処理
            if(r.Result == ButtonResult.OK) {
                // OKボタンが押された場合の処理
            }
            else if(r.Result == ButtonResult.Cancel) {
                // キャンセルボタンが押された場合の処理
            }
            //    // ダイアログ終了後処理
            IsCreatePullRequestDialogButtonEnabled = true;
        } );
    }

    /// <summary>
    /// TabsをTranslateFileDirから初期化
    /// </summary>
    private void RefleshTabs() {
        var aircraftPath = Path.Join( appSettingsService.TranslateFileDir, "DCSWorld", "Mods", "aircraft");
        var dlcCampaignsPath = Path.Join( appSettingsService.TranslateFileDir, "DCSWorld", "Mods", "campaigns" );
        Tabs = [
            new UploadTabItemViewModel("Aircraft"     , fileEntryViewModelFactory.Create(aircraftPath, true)),
            new UploadTabItemViewModel("DLC Campaigns", fileEntryViewModelFactory.Create(dlcCampaignsPath, true))
        ];
        foreach(var tab in Tabs) {
            tab.Root.LoadChildren();
            tab.Root.IsChildrenLoaded = true;
        }
    }

    #endregion
}
