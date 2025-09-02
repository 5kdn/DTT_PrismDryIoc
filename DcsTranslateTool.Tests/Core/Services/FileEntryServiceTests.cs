using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Core.Services;

using Xunit;

namespace DcsTranslateTool.Tests.Core.Services;
public class FileEntryServiceTests : IDisposable {
    private readonly string _tempDir;

    public FileEntryServiceTests() {
        _tempDir = Path.Join( Path.GetTempPath(), Guid.NewGuid().ToString() );
        Directory.CreateDirectory( _tempDir );
    }

    public void Dispose() {
        if(Directory.Exists( _tempDir )) Directory.Delete( _tempDir, true );
        GC.SuppressFinalize( this );
    }

    #region GetChildrenRecursive
    [Fact]
    public void GetChildrenRecursiveがファイルを含むディレクトリを指定した場合正しい結果が返る() {
        // Arrange
        var targetDir = Path.Combine(_tempDir, "DirWithFile");
        var targetSubdir = Path.Combine(targetDir, "subdir");
        Directory.CreateDirectory( targetSubdir );

        var filePath = Path.Combine(targetSubdir, "test.txt");
        File.WriteAllText( filePath, "Hello World" );
        var service = new FileEntryService();

        // Act
        var result = service.GetChildrenRecursive(targetDir);

        // Assert
        Assert.True( result.IsSuccess );
        var actualFileEntries = result.Value;
        Assert.Single( actualFileEntries );

        var actualEntry = actualFileEntries.First();
        Assert.NotNull( actualEntry );
        Assert.Equal( "test.txt", actualEntry.Name );
        Assert.Equal( "subdir/test.txt", actualEntry.Path );
        Assert.False( actualEntry.IsDirectory );
        Assert.Equal( "5e1c309dae7f45e0f39b1bf3ac3cd9db12e7d689", actualEntry.LocalSha );
        Assert.Null( actualEntry.RepoSha );
    }

    [Fact]
    public void GetChildrenRecursiveは空のディレクトリを指定した場合空の結果が返る() {
        // Arrange
        var targetDir = Path.Combine(_tempDir, "EmptyDir_" + Guid.NewGuid());
        Directory.CreateDirectory( targetDir );
        var service = new FileEntryService();

        // Act
        var result = service.GetChildrenRecursive(targetDir);

        // Assert
        Assert.True( result.IsSuccess );
        var actualFileEntries = result.Value;
        Assert.Empty( actualFileEntries );
    }

    [Fact]
    public void GetChildrenRecursiveは存在しないパスを指定した場合Failが返る() {
        // Arrange
        var invalidPath = Path.Combine(_tempDir, "NotExist_" + Guid.NewGuid());
        var service = new FileEntryService();

        // Act
        var result = service.GetChildrenRecursive(invalidPath);

        // Assert
        Assert.True( result.IsFailed );
        var actualMessage = result.Errors[0].Message;
        Assert.StartsWith( "指定されたパスが存在しません: ", actualMessage );
    }

    #endregion

    #region Watch

    [Fact]
    public async Task Watchはファイルを追加したときEntriesChangedが発火する() {
        // Arrange
        var targetDir = Path.Combine(_tempDir, "WatchDir");
        Directory.CreateDirectory( targetDir );
        using var service = new FileEntryService();
        var tcs = new TaskCompletionSource<IReadOnlyList<FileEntry>>(TaskCreationOptions.RunContinuationsAsynchronously);
        service.EntriesChanged += entries => {
            if(entries is { Count: > 0 }) tcs.TrySetResult( entries );
            return Task.CompletedTask;
        };

        // Act
        service.Watch( targetDir );
        var filePath = Path.Combine(targetDir, "new.txt");
        await File.WriteAllTextAsync( filePath, "data" );
        await Task.Delay( 500 );    // GitHub Actionsでの実行結果を安定化させる

        // Assert
        var entries = await tcs.Task;
        Assert.NotNull( entries );
        Assert.Single( entries );

        var entry = entries[0];
        Assert.Equal( "new.txt", entry.Name );
        Assert.Equal( "6320cd248dd8aeaab759d5871f8781b5c0505172", entry.LocalSha );
    }

    #endregion
}