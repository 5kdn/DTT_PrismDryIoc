using DcsTranslateTool.Core.Helpers;

using Xunit;

namespace DcsTranslateTool.Core.Tests.Helpers;

public class GitBlobSha1HelperTests {
    #region Calculate

    [Fact]
    public void Calculateはファイルが読み取り可能なときSHA1を返す() {
        // Arrange
        var temp = Path.GetTempFileName();
        File.WriteAllText( temp, "test" );

        // Act
        var sha = GitBlobSha1Helper.Calculate( temp );

        // Assert
        Assert.Equal( "30d74d258442c7c65512eafab474568dd706c430", sha );

        File.Delete( temp );
    }

    [Fact]
    public void Calculateはファイルがロックされているときnullを返す() {
        // Arrange
        var temp = Path.GetTempFileName();
        using var _ = new FileStream( temp, FileMode.Open, FileAccess.Read, FileShare.None );

        // Act
        var sha = GitBlobSha1Helper.Calculate( temp );

        // Assert
        Assert.Null( sha );

        File.Delete( temp );
    }

    #endregion
}
