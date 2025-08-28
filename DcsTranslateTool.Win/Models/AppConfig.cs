namespace DcsTranslateTool.Win.Models;

/// <summary>
/// アプリケーション設定を表すモデル
/// </summary>
public class AppConfig {
    public string ConfigurationsFolder { get; set; } = string.Empty;

    public string AppPropertiesFileName { get; set; } = string.Empty;

    public string PrivacyStatement { get; set; } = string.Empty;
}