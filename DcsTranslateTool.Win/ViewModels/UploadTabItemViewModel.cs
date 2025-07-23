namespace DcsTranslateTool.Win.ViewModels;

public class UploadTabItemViewModel( string title, FileEntryViewModel rootEntry ) : BindableBase {
    #region Properties

    public string Title { get; } = title;

    public FileEntryViewModel Root {
        get => rootEntry;
        set => SetProperty( ref rootEntry, value );
    }

    #endregion
}
