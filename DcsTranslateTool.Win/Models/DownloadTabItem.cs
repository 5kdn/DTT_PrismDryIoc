using DcsTranslateTool.Win.ViewModels;

namespace DcsTranslateTool.Win.Models;
public class DownloadTabItem( string title, RepoTreeItemViewModel repoTree ) : BindableBase {
    public string Title { get; } = title;

    public RepoTreeItemViewModel RepoTree {
        get => repoTree;
        set => SetProperty( ref repoTree, value );
    }
}
