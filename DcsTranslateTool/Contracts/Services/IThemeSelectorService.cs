using DcsTranslateTool.Models;

namespace DcsTranslateTool.Contracts.Services;

public interface IThemeSelectorService
{
    void InitializeTheme();

    void SetTheme( AppTheme theme );

    AppTheme GetCurrentTheme();
}
