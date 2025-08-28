using DcsTranslateTool.Win.Contracts.Services;
using DcsTranslateTool.Win.Properties;

namespace DcsTranslateTool.Win.Services;

/// <summary>
/// Properties.Settings を操作するサービス
/// </summary>
public class AppSettingsService : IAppSettingsService {
    /// <inheritdoc/>
    public string SourceAircraftDir {
        get => Settings.Default.SourceAircraftDir;
        set {
            Settings.Default.SourceAircraftDir = value;
            Settings.Default.Save();
        }
    }

    /// <inheritdoc/>
    public string SourceDlcCampaignDir {
        get => Settings.Default.SourceDlcCampaignDir;
        set {
            Settings.Default.SourceDlcCampaignDir = value;
            Settings.Default.Save();
        }
    }

    /// <inheritdoc/>
    public string SourceUserDir {
        get => Settings.Default.SourceUserDir;
        set {
            Settings.Default.SourceUserDir = value;
            Settings.Default.Save();
        }
    }

    /// <inheritdoc/>
    public string TranslateFileDir {
        get => Settings.Default.TranslateFileDir;
        set {
            Settings.Default.TranslateFileDir = value;
            Settings.Default.Save();
        }
    }

    /// <inheritdoc/>
    public void Reset()
        => Settings.Default.Reset();
}