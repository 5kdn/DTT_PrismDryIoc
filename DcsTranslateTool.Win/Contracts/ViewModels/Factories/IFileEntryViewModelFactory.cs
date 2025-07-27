using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.ViewModels;

namespace DcsTranslateTool.Win.Contracts.ViewModels.Factories;
public interface IFileEntryViewModelFactory {
    FileEntryViewModel Create( FileEntry model, bool isSelected = false );
    FileEntryViewModel Create( string absolutePath, bool isDirectory, bool isSelected = false );
}
