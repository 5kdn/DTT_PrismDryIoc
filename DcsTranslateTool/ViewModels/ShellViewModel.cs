using System.Windows.Input;

using DcsTranslateTool.Constants;

namespace DcsTranslateTool.ViewModels;

/// <summary>
/// アプリケーションシェルを制御する ViewModel である
/// </summary>
public class ShellViewModel : BindableBase
{
    private readonly IRegionManager _regionManager;
    private IRegionNavigationService _navigationService;
    private DelegateCommand _goBackCommand;
    private ICommand _loadedCommand;
    private ICommand _unloadedCommand;

    /// <summary>
    /// 戻るボタン用のコマンドである
    /// </summary>
    public DelegateCommand GoBackCommand =>
        _goBackCommand ?? (_goBackCommand = new DelegateCommand( OnGoBack, CanGoBack ));

    /// <summary>
    /// ウィンドウロード時に呼び出されるコマンドである
    /// </summary>
    public ICommand LoadedCommand =>
        _loadedCommand ?? (_loadedCommand = new DelegateCommand( OnLoaded ));

    /// <summary>
    /// ウィンドウアンロード時に呼び出されるコマンドである
    /// </summary>
    public ICommand UnloadedCommand =>
        _unloadedCommand ?? (_unloadedCommand = new DelegateCommand( OnUnloaded ));

    /// <summary>
    /// 新しいインスタンスを生成する
    /// </summary>
    /// <param name="regionManager">リージョン管理用サービス</param>
    public ShellViewModel( IRegionManager regionManager )
    {
        _regionManager = regionManager;
    }

    private void OnLoaded()
    {
        _navigationService = _regionManager.Regions[Regions.Main].NavigationService;
        _navigationService.Navigated += OnNavigated;
        _navigationService.RequestNavigate( PageKeys.Main );
        _goBackCommand?.RaiseCanExecuteChanged();
    }

    private void OnUnloaded()
    {
        if(_navigationService != null)
        {
            _navigationService.Navigated -= OnNavigated;
        }
        _regionManager.Regions.Remove( Regions.Main );
    }

    private bool CanGoBack()
        => _navigationService != null && _navigationService.Journal.CanGoBack;

    private void OnGoBack()
        => _navigationService.Journal.GoBack();

    private void OnNavigated( object sender, RegionNavigationEventArgs e )
        => _goBackCommand?.RaiseCanExecuteChanged();
}
