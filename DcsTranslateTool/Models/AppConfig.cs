namespace DcsTranslateTool.Models;

/// <summary>
/// アプリケーション設定を表すモデル
/// </summary>
public class AppConfig
{
    public string ConfigurationsFolder { get; set; }

    public string AppPropertiesFileName { get; set; }

    public string PrivacyStatement { get; set; }
}
