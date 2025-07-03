using DcsTranslateTool.Models;

namespace DcsTranslateTool.Contracts.Services;

/// <summary>
/// テーマ選択を制御するサービスの契約
/// </summary>
public interface IThemeSelectorService
{
    /// <summary>
    /// 保存されたテーマを適用する
    /// </summary>
    void InitializeTheme();

    /// <summary>
    /// 指定されたテーマを適用する
    /// </summary>
    /// <param name="theme">テーマ種別</param>
    void SetTheme( AppTheme theme );

    /// <summary>
    /// 現在のテーマを取得する
    /// </summary>
    /// <returns>テーマ種別</returns>
    AppTheme GetCurrentTheme();
}
