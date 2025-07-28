using System.IO;

using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.Contracts.ViewModels.Factories;
using DcsTranslateTool.Win.Enums;

namespace DcsTranslateTool.Win.ViewModels.Factories;

public class FileEntryViewModelFactory( IResolverContext resolver ) : IFileEntryViewModelFactory {
    /// <inheritdoc />
    public FileEntryViewModel Create( FileEntry model, CheckState checkState = CheckState.Unchecked ) {
        var service = resolver.Resolve<IFileEntryService>();
        return new FileEntryViewModel( this, service, model )
        {
            CheckState = checkState
        };
    }

    /// <inheritdoc />
    public FileEntryViewModel Create( string absolutePath, bool isDirectory, CheckState checkState = CheckState.Unchecked ) {
        return Create( new FileEntry( Path.GetFileName( absolutePath ), absolutePath, isDirectory ), checkState );
    }
}
