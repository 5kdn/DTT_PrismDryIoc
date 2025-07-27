using System.IO;

using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.Contracts.ViewModels.Factories;

namespace DcsTranslateTool.Win.ViewModels.Factories;

public class FileEntryViewModelFactory( IResolverContext resolver ) : IFileEntryViewModelFactory {
    /// <inheritdoc />
    public FileEntryViewModel Create( FileEntry model, bool isSelected = false ) {
        var service = resolver.Resolve<IFileEntryService>();
        return new FileEntryViewModel( this, service, model )
        {
            IsSelected = isSelected
        };
    }

    /// <inheritdoc />
    public FileEntryViewModel Create( string absolutePath, bool isDirectory, bool isSelected = false ) {
        return Create( new FileEntry( Path.GetFileName( absolutePath ), absolutePath, isDirectory ), isSelected );
    }
}
