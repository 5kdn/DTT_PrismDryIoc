using System.Windows.Input;

using DcsTranslateTool.Win.Constants;

namespace DcsTranslateTool.Win.ViewModels;

/// <summary>
/// アプリケーションシェルを制御する ViewModel
/// </summary>
/// <param name="regionManager">リージョン管理用サービス</param>
public class ShellViewModel( IRegionManager regionManager ) : BindableBase {
    private IRegionNavigationService? _navigationService;
    private DelegateCommand? _goBackCommand;
    private ICommand? _loadedCommand;
    private ICommand? _unloadedCommand;

    /// <summary>
    /// 戻るボタン用のコマンド
    /// </summary>
    public DelegateCommand GoBackCommand => _goBackCommand ??= new DelegateCommand( OnGoBack, CanGoBack );

    /// <summary>
    /// ウィンドウロード時に呼び出されるコマンド
    /// </summary>
    public ICommand LoadedCommand => _loadedCommand ??= new DelegateCommand( OnLoaded );

    /// <summary>
    /// ウィンドウアンロード時に呼び出されるコマンド
    /// </summary>
    public ICommand UnloadedCommand => _unloadedCommand ??= new DelegateCommand( OnUnloaded );

    private void OnLoaded() {
        _navigationService = regionManager.Regions[Regions.Main].NavigationService;
        _navigationService.Navigated += OnNavigated;
        _navigationService.RequestNavigate( PageKeys.Main );
        _goBackCommand?.RaiseCanExecuteChanged();
    }

    private void OnUnloaded() {
        if(_navigationService != null) {
            _navigationService.Navigated -= OnNavigated;
        }
        regionManager.Regions.Remove( Regions.Main );
    }

    private bool CanGoBack() => _navigationService?.Journal.CanGoBack == true;

    private void OnGoBack() => _navigationService?.Journal.GoBack();

    private void OnNavigated( object? sender, RegionNavigationEventArgs e ) => _goBackCommand?.RaiseCanExecuteChanged();
}