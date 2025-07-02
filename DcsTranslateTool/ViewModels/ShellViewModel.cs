using System.Windows.Input;

using DcsTranslateTool.Constants;

namespace DcsTranslateTool.ViewModels;

public class ShellViewModel : BindableBase
{
    private readonly IRegionManager _regionManager;
    private IRegionNavigationService _navigationService;
    private DelegateCommand _goBackCommand;
    private ICommand _loadedCommand;
    private ICommand _unloadedCommand;

    public DelegateCommand GoBackCommand => _goBackCommand ?? (_goBackCommand = new DelegateCommand( OnGoBack, CanGoBack ));

    public ICommand LoadedCommand => _loadedCommand ?? (_loadedCommand = new DelegateCommand( OnLoaded ));

    public ICommand UnloadedCommand => _unloadedCommand ?? (_unloadedCommand = new DelegateCommand( OnUnloaded ));

    public ShellViewModel( IRegionManager regionManager )
    {
        _regionManager = regionManager;
    }

    private void OnLoaded()
    {
        _navigationService = _regionManager.Regions[Regions.Main].NavigationService;
        _navigationService.RequestNavigate( PageKeys.Main );
    }

    private void OnUnloaded()
    {
        _regionManager.Regions.Remove( Regions.Main );
    }

    private bool CanGoBack()
        => _navigationService != null && _navigationService.Journal.CanGoBack;

    private void OnGoBack()
        => _navigationService.Journal.GoBack();
}
