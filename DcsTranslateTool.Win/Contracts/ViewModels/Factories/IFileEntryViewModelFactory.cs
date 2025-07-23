using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.ViewModels;

namespace DcsTranslateTool.Win.Contracts.ViewModels.Factories;
public interface IFileEntryViewModelFactory {
    FileEntryViewModel Create( FileEntry model );
    FileEntryViewModel Create( string absolutePath, bool isDirectory );
}
