using System.IO;

using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.Contracts.ViewModels.Factories;
using DcsTranslateTool.Win.Enums;

namespace DcsTranslateTool.Win.ViewModels.Factories;

public class FileEntryViewModelFactory( IResolverContext resolver ) : IFileEntryViewModelFactory {
    /// <inheritdoc />
    public FileEntryViewModel Create(
        Entry model,
        FileEntryViewModel? parent = null,
        CheckState checkState = CheckState.Unchecked
    ) {
        var service = resolver.Resolve<IFileEntryService>();
        return new FileEntryViewModel( this, service, model )
        {
            Parent = parent,
            CheckState = checkState
        };
    }

    /// <inheritdoc />
    public FileEntryViewModel Create(
        string absolutePath,
        bool isDirectory,
        FileEntryViewModel? parent = null,
        CheckState checkState = CheckState.Unchecked
    ) {
        return Create(
            new Entry( Path.GetFileName( absolutePath ), absolutePath, isDirectory ),
            parent,
            checkState );
    }
}