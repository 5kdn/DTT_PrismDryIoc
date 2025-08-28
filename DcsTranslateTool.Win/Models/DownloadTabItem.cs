using DcsTranslateTool.Win.ViewModels;

namespace DcsTranslateTool.Win.Models;
public class DownloadTabItem( string title, RepoEntryViewModel root ) {
    public string Title { get; } = title;

    public RepoEntryViewModel Root { get; set; } = root;
}