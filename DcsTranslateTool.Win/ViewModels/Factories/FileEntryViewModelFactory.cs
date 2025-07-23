using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.Contracts.ViewModels.Factories;

namespace DcsTranslateTool.Win.ViewModels.Factories;

public class FileEntryViewModelFactory( IResolverContext resolver ) : IFileEntryViewModelFactory {
    /// <inheritdoc />
    public FileEntryViewModel Create( FileEntry model ) {
        var service = resolver.Resolve<IFileEntryService>();
        return new FileEntryViewModel( this, service, model );
    }

    /// <inheritdoc />
    public FileEntryViewModel Create( string absolutePath, bool isDirectory ) {
        return Create( new FileEntry( absolutePath, isDirectory ) );
    }
}
