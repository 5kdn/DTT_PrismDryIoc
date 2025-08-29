using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.Enums;
using DcsTranslateTool.Win.ViewModels;

namespace DcsTranslateTool.Win.Contracts.ViewModels.Factories;
public interface IFileEntryViewModelFactory {
    FileEntryViewModel Create(
        FileEntry model,
        FileEntryViewModel? parent = null,
        CheckState checkState = CheckState.Unchecked );

    FileEntryViewModel Create(
        string absolutePath,
        bool isDirectory,
        FileEntryViewModel? parent = null,
        CheckState checkState = CheckState.Unchecked );
}