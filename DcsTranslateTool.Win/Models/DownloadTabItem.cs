using DcsTranslateTool.Win.Contracts.ViewModels;

namespace DcsTranslateTool.Win.Models;
public class DownloadTabItem( string title, IFileEntryViewModel root ) {
    public string Title { get; } = title;

    public IFileEntryViewModel Root { get; set; } = root;
}