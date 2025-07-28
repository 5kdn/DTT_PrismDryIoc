using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.Enums;
using DcsTranslateTool.Win.ViewModels;

namespace DcsTranslateTool.Win.Contracts.ViewModels.Factories;
public interface IFileEntryViewModelFactory {
    FileEntryViewModel Create( FileEntry model, CheckState checkState = CheckState.Unchecked );
    FileEntryViewModel Create( string absolutePath, bool isDirectory, CheckState checkState = CheckState.Unchecked );
}
