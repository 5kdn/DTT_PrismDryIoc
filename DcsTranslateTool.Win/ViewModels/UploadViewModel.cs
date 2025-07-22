using System.Collections.ObjectModel;
using System.IO;

using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Win.Constants;
using DcsTranslateTool.Win.Contracts.Services;
using DcsTranslateTool.Win.Models;

namespace DcsTranslateTool.Win.ViewModels;

public class UploadViewModel(
    IAppSettingsService appSettingsService,
    IRegionManager regionManager,
    IDialogService dialogService,
    IFileService fileService
    ) : BindableBase, INavigationAware {
    #region Fields

    private ObservableCollection<UploadTabItem> _tabs = [];
    private int _selectedTabIndex = 0;
    private bool _isCreatePullRequestDialogButtonEnabled = true;

    private DelegateCommand? _openSettingsCommand;
    private DelegateCommand? _openCreatePullRequestDialogCommand;
    private DelegateCommand<FileTreeItemViewModel>? _loadLocalTreeCommand;

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
    public ObservableCollection<UploadTabItem> Tabs {
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
    public DelegateCommand<FileTreeItemViewModel> LoadLocalTreeCommand =>
        _loadLocalTreeCommand ??= new DelegateCommand<FileTreeItemViewModel>( OnLoadLocalTree );

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

    private void OnLoadLocalTree( FileTreeItemViewModel node ) {
        if(node == null) return;
        node.UpdateChildren();
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
        var aircraftTree = fileService.GetFileTree(
            Path.Join( appSettingsService.TranslateFileDir, "DCSWorld", "Mods", "aircraft" )
        );
        var dlcCampaignsTree = fileService.GetFileTree(
            Path.Join( appSettingsService.TranslateFileDir, "DCSWorld", "Mods", "campaigns" )
        );
        Tabs = [
            new UploadTabItem("Aircraft"     , new FileTreeItemViewModel(aircraftTree)),
            new UploadTabItem("DLC Campaigns", new FileTreeItemViewModel(dlcCampaignsTree))
        ];
    }

    #endregion
}
