using System.Windows;

using DcsTranslateTool.Win.Constants;

namespace DcsTranslateTool.Win.Views;

/// <summary>
/// シェルウィンドウである。
/// </summary>
public partial class ShellWindow : Window {
    /// <summary>
    /// 新しいインスタンスを生成する。
    /// </summary>
    /// <param name="regionManager">リージョンマネージャー</param>
    public ShellWindow( IRegionManager regionManager ) {
        InitializeComponent();
        RegionManager.SetRegionName( shellContentControl, Regions.Main );
        RegionManager.SetRegionManager( shellContentControl, regionManager );
    }
}