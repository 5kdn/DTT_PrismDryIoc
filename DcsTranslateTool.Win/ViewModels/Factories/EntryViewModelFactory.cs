using System.IO;

using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Helpers;
using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.Contracts.Services;
using DcsTranslateTool.Win.Contracts.ViewModels.Factories;
using DcsTranslateTool.Win.Enums;
using DcsTranslateTool.Win.ViewModels;

using DryIoc;

namespace DcsTranslateTool.Win.ViewModels.Factories;

/// <summary>
/// <see cref="EntryViewModel"/> を生成するファクトリーである。
/// </summary>
/// <param name="resolver">依存解決コンテナ</param>
public class EntryViewModelFactory( IResolverContext resolver ) : IEntryViewModelFactory {
    /// <inheritdoc />
    public EntryViewModel Create(
        Entry model,
        EntryViewModel? parent = null,
        CheckState checkState = CheckState.Unchecked
    ) {
        var service = resolver.Resolve<IFileEntryService>();
        return new EntryViewModel( this, service, model )
        {
            Parent = parent,
            CheckState = checkState
        };
    }

    /// <inheritdoc />
    public EntryViewModel Create(
        string absolutePath,
        bool isDirectory,
        EntryViewModel? parent = null,
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