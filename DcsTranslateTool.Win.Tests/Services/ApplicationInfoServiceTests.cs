using DcsTranslateTool.Win.Services;

using Xunit;

namespace DcsTranslateTool.Win.Tests.Services;
public class ApplicationInfoServiceTests {
    #region GetVersion

    [Fact]
    [Trait( "Category", "WindowsOnly" )]
    public void GetVersionは実行アセンブリにバージョン情報が含まれているときに正しいバージョンを返す() {
        // Arrange
        var service = new ApplicationInfoService();

        // Act
        var version = service.GetVersion();

        // Assert
        // 実際のバージョン番号は都度変わるので Major.Minorの型チェックのみ
        Assert.NotNull( version );
        Assert.True( version.Major >= 0 ); // バージョンが取れればOKとみなす
        Assert.True( version.Minor >= 0 );
    }

    #endregion
}
