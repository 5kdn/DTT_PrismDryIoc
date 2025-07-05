using DcsTranslateTool.Win.Constants;

using MahApps.Metro.Controls;

namespace DcsTranslateTool.Win.Views;

public partial class ShellWindow : MetroWindow {
    public ShellWindow( IRegionManager regionManager ) {
        InitializeComponent();
        RegionManager.SetRegionName( shellContentControl, Regions.Main );
        RegionManager.SetRegionManager( shellContentControl, regionManager );
    }
}
