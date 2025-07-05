using System.Windows;

using ControlzEx.Theming;

using MahApps.Metro.Theming;

using DcsTranslateTool.Win.Contracts.Services;
using DcsTranslateTool.Win.Models;

namespace DcsTranslateTool.Win.Services;

/// <summary>
/// アプリのテーマを管理するサービス
/// </summary>
public class ThemeSelectorService : IThemeSelectorService
{
    private const string HcDarkTheme = "pack://application:,,,/Styles/Themes/HC.Dark.Blue.xaml";
    private const string HcLightTheme = "pack://application:,,,/Styles/Themes/HC.Light.Blue.xaml";

    /// <summary>
    /// 新しいインスタンスを生成する
    /// </summary>
    public ThemeSelectorService() { }

    /// <inheritdoc/>
    public void InitializeTheme()
    {
        // TODO: Mahapps.Metro supports syncronization with high contrast but you have to provide custom high contrast themes
        // We've added basic high contrast dictionaries for Dark and Light themes
        // Please complete these themes following the docs on https://mahapps.com/docs/themes/thememanager#creating-custom-themes
        ThemeManager.Current.AddLibraryTheme( new LibraryTheme( new Uri( HcDarkTheme ), MahAppsLibraryThemeProvider.DefaultInstance ) );
        ThemeManager.Current.AddLibraryTheme( new LibraryTheme( new Uri( HcLightTheme ), MahAppsLibraryThemeProvider.DefaultInstance ) );

        var theme = GetCurrentTheme();
        SetTheme( theme );
    }

    /// <inheritdoc/>
    public void SetTheme( AppTheme theme )
    {
        if(theme == AppTheme.Default)
        {
            ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncAll;
            ThemeManager.Current.SyncTheme();
        }
        else
        {
            ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncWithHighContrast;
            ThemeManager.Current.SyncTheme();
            ThemeManager.Current.ChangeTheme( Application.Current, $"{theme}.Blue", SystemParameters.HighContrast );
        }

        App.Current.Properties["Theme"] = theme.ToString();
    }

    /// <inheritdoc/>
    public AppTheme GetCurrentTheme()
    {
        if(App.Current.Properties.Contains( "Theme" ))
        {
            var themeName = App.Current.Properties["Theme"].ToString();
            Enum.TryParse( themeName, out AppTheme theme );
            return theme;
        }

        return AppTheme.Default;
    }
}
