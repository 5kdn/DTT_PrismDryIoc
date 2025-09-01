using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Core.Services;
using DcsTranslateTool.Win.Contracts.Services;
using DcsTranslateTool.Win.Services;
using DcsTranslateTool.Win.ViewModels;

using FluentResults;

using Moq;

using Xunit;


namespace DcsTranslateTool.Tests.Win.ViewModels;

public class UploadViewModelTests {
    private readonly Container _container;

    public UploadViewModelTests() {

        var mockedRepositoryService = new Mock<IRepositoryService>();
        _container = new Container();
        _container.Register<IAppSettingsService, AppSettingsService>( Reuse.Singleton );
        _container.Register<IRegionManager, RegionManager>( Reuse.Singleton );
        _container.RegisterInstance<IDialogService>( Mock.Of<IDialogService>() );
        _container.RegisterInstance<IRepositoryService>( mockedRepositoryService.Object );
        _container.Register<IFileService, FileService>( Reuse.Transient );
        _container.Register<IFileEntryService, DummyFileEntryService>( Reuse.Singleton );
        // ViewModels
        _container.Register<UploadViewModel>( Reuse.Singleton );
    }
    private class DummyFileEntryService : IFileEntryService {
        public void Dispose() { }

#pragma warning disable CS0067
        public event Func<IReadOnlyList<FileEntry>, Task>? EntriesChanged;
#pragma warning restore CS0067
        public Task<IReadOnlyList<FileEntry>> GetEntriesAsync() => Task.FromResult<IReadOnlyList<FileEntry>>( [] );
        public void Watch( string path ) { }
        public Result<IEnumerable<FileEntry>> GetChildrenRecursive( string path ) => Result.Ok<IEnumerable<FileEntry>>( [] );
    }

    [Fact( DisplayName = "UploadViewModelが正常に生成できる" )]
    public void TestUploadViewModelCreation() {
        // Arrange & Act
        var vm = _container.Resolve<UploadViewModel>();

        // Assert
        Assert.NotNull( vm );
    }
}