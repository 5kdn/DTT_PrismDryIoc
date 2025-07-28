using System.Collections.ObjectModel;
using System.IO;

using DcsTranslateTool.Win.Constants;
using DcsTranslateTool.Win.Contracts.Services;
using DcsTranslateTool.Win.Contracts.ViewModels.Factories;
using DcsTranslateTool.Win.Enums;
using DcsTranslateTool.Win.Extensions;

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
        SubscribeSelectionChanged( node );
        node.IsChildrenLoaded = true;
    }

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
    private void RefleshTabs() {
        var tabs = Enum.GetValues<RootTabType>().Select(tabType =>{
            var fileEntryVM = fileEntryViewModelFactory.Create(
                    Path.Join([appSettingsService.TranslateFileDir, ..tabType.GetRepoDirRoot()]),true);
            fileEntryVM.LoadChildren();
            SubscribeSelectionChanged( fileEntryVM );
            return new UploadTabItemViewModel(tabType, fileEntryVM);
        });
        Tabs.Clear();
        Tabs = [.. tabs];
        UpdateCreatePullRequestDialogButton();
    }

    private void OnFileEntrySelectedChanged( object? sender, bool _) => UpdateCreatePullRequestDialogButton();

    private void SubscribeSelectionChanged( FileEntryViewModel node ) {
        node.IsSelectedChanged += OnFileEntrySelectedChanged;
        foreach(var child in node.Children) {
            if(child is not null) SubscribeSelectionChanged( child );
        }
    }

    private void UpdateCreatePullRequestDialogButton() {
        if(Tabs.Count == 0) {
            IsCreatePullRequestDialogButtonEnabled = false;
            return;
        }

        IsCreatePullRequestDialogButtonEnabled = Tabs[SelectedTabIndex]
            .Root
            .GetCheckedModelRecursice()
            .Any();
    }

    #endregion
}
