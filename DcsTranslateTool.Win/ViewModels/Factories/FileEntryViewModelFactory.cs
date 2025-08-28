using System.IO;

using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Helpers;
using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.Contracts.Services;
using DcsTranslateTool.Win.Contracts.ViewModels.Factories;
using DcsTranslateTool.Win.Enums;

using DryIoc;

namespace DcsTranslateTool.Win.ViewModels.Factories;

/// <summary>
/// <see cref="FileEntryViewModel"/> を生成するファクトリーである。
/// </summary>
/// <param name="resolver">依存解決コンテナ</param>
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
        var appSettings = resolver.Resolve<IAppSettingsService>();
        string relative = Path.GetRelativePath( appSettings.TranslateFileDir, absolutePath );
        string? sha = isDirectory ? null : GitBlobSha1Helper.Calculate( absolutePath );
        return Create(
            new Entry( Path.GetFileName( absolutePath ), relative, isDirectory, sha ),
            parent,
            checkState );
    }
}