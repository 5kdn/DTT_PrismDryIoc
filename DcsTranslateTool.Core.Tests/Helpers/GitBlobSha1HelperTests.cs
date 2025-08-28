using DcsTranslateTool.Core.Helpers;

using Xunit;

namespace DcsTranslateTool.Core.Tests.Helpers;

public class GitBlobSha1HelperTests : IDisposable {
    private readonly string _tempDir;

    public GitBlobSha1HelperTests() {
        _tempDir = Path.Join( Path.GetTempPath(), Guid.NewGuid().ToString() );
        Directory.CreateDirectory( _tempDir );
    }

    public void Dispose() {
        if(Directory.Exists( _tempDir )) Directory.Delete( _tempDir, true );
        GC.SuppressFinalize( this );
    }

    #region Calculate

    [Fact]
    public void Calculateはファイルが読み取り可能なときSHA1を返す() {
        // Arrange
        var temp = Path.Combine( _tempDir, "test.txt" );
        File.WriteAllText( temp, "test" );

        // Act
        var sha = GitBlobSha1Helper.Calculate( temp );

        // Assert
        Assert.Equal( "30d74d258442c7c65512eafab474568dd706c430", sha );
    }

    [Fact]
    public void Calculateはファイルがロックされているときnullを返す() {
        // Arrange
        var temp = Path.Combine( _tempDir, "test.txt" );
        File.WriteAllText( temp, "test" );
        using var _ = new FileStream( temp, FileMode.Open, FileAccess.Read, FileShare.None );

        // Act
        var sha = GitBlobSha1Helper.Calculate( temp );

        // Assert
        Assert.Null( sha );
    }

    #endregion
}