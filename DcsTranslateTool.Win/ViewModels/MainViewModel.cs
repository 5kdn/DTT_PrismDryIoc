using DcsTranslateTool.Win.Constants;

namespace DcsTranslateTool.Win.ViewModels;

/// <summary>
/// メイン画面の表示ロジックを保持する ViewModel
/// </summary>
public class MainViewModel : BindableBase {
    private readonly IRegionManager _regionManager;
    private DelegateCommand _openSettingsCommand;

    /// <summary>
    /// 設定画面を開くコマンド
    /// </summary>
    public DelegateCommand OpenSettingsCommand =>
        _openSettingsCommand ??= new DelegateCommand( OnOpenSettings );

    /// <summary>
    /// ViewModel の新しいインスタンスを生成する
    /// </summary>
    /// <param name="regionManager">ナビゲーション管理用の IRegionManager</param>
    public MainViewModel( IRegionManager regionManager ) {
        _regionManager = regionManager;
    }

    private void OnOpenSettings()
        => _regionManager.RequestNavigate( Regions.Main, PageKeys.Settings );
}
