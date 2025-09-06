using DcsTranslateTool.Win.Contracts.Services;
using DcsTranslateTool.Win.Models;

using MaterialDesignThemes.Wpf;

namespace DcsTranslateTool.Win.Services;

/// <summary>
/// アプリのテーマを管理するサービス
/// </summary>
public class ThemeSelectorService() : IThemeSelectorService {

    private readonly PaletteHelper _paletteHelper = new();

    /// <inheritdoc/>
    public void InitializeTheme() {
        var theme = GetCurrentTheme();
        SetTheme( theme );
    }

    /// <inheritdoc/>
    public void SetTheme( AppTheme theme ) {
        var materialTheme = _paletteHelper.GetTheme();

        materialTheme.SetBaseTheme( theme == AppTheme.Dark ? BaseTheme.Dark : BaseTheme.Light );

        _paletteHelper.SetTheme( materialTheme );

        App.Current.Properties["Theme"] = theme.ToString();
    }

    /// <inheritdoc/>
    public AppTheme GetCurrentTheme() {
        if(App.Current.Properties.Contains( "Theme" )) {
            var themeName = App.Current.Properties["Theme"]?.ToString();
            if(Enum.TryParse( themeName, out AppTheme theme )) return theme;
        }

        return AppTheme.Default;
    }
}