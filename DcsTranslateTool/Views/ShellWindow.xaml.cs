using MahApps.Metro.Controls;

using DcsTranslateTool.Constants;

namespace DcsTranslateTool.Views;

public partial class ShellWindow : MetroWindow
{
    public ShellWindow( IRegionManager regionManager )
    {
        InitializeComponent();
        RegionManager.SetRegionName( shellContentControl, Regions.Main );
        RegionManager.SetRegionManager( shellContentControl, regionManager );
    }
}
