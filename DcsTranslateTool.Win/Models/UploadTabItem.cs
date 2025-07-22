using DcsTranslateTool.Win.ViewModels;

namespace DcsTranslateTool.Win.Models;
public class UploadTabItem( string title, FileTreeItemViewModel localTree ) : BindableBase {
    public string Title { get; } = title;
    public FileTreeItemViewModel LocalTree {
        get => localTree;
        set => SetProperty( ref localTree, value );
    }
}
