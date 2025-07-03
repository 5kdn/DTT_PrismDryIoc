using DcsTranslateTool.Services;
using DcsTranslateTool.Contracts.Services;
using DcsTranslateTool.Properties;
using Xunit;

namespace DcsTranslateTool.Tests.Services;

/// <summary>
/// AppSettingsService のテスト
/// </summary>
public class AppSettingsServiceTests
{
    [Fact(DisplayName = "設定値の保存ができる" )]
    [Trait("Category", "WindowsOnly")]
    public void SettingValues_ShouldPersist()
    {
        // Arrange
        Settings.Default.Reset();
        IAppSettingsService service = new AppSettingsService();

        // Act
        service.SourceAircraftDir = "A";
        service.SourceDlcCampaignDir = "B";
        service.SourceUserDir = "C";
        service.TranslateFileDir = "D";

        // Assert
        Assert.Equal("A", Settings.Default.SourceAircraftDir);
        Assert.Equal("B", Settings.Default.SourceDlcCampaignDir);
        Assert.Equal("C", Settings.Default.SourceUserDir);
        Assert.Equal("D", Settings.Default.TranslateFileDir);

        Settings.Default.Reset();
    }

    [Fact(DisplayName = "リセットで既定値に戻る" )]
    [Trait("Category", "WindowsOnly")]
    public void Reset_ShouldRestoreDefaults()
    {
        // Arrange
        Settings.Default.Reset();
        IAppSettingsService service = new AppSettingsService();
        service.SourceAircraftDir = "A";
        service.SourceDlcCampaignDir = "B";
        service.SourceUserDir = "C";
        service.TranslateFileDir = "D";

        // Act
        service.Reset();

        // Assert
        Assert.Equal(string.Empty, service.SourceAircraftDir);
        Assert.Equal(string.Empty, service.SourceDlcCampaignDir);
        Assert.Equal(string.Empty, service.SourceUserDir);
        Assert.Equal(string.Empty, service.TranslateFileDir);

        Settings.Default.Reset();
    }
}
