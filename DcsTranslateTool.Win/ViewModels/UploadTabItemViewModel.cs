using DcsTranslateTool.Win.Enums;
using DcsTranslateTool.Win.Extensions;

namespace DcsTranslateTool.Win.ViewModels;

public class UploadTabItemViewModel( RootTabType tabType, FileEntryViewModel rootEntry ) : BindableBase {
    public string Title { get; } = tabType.GetTabTitle();

    public FileEntryViewModel Root {
        get => rootEntry;
        set => SetProperty( ref rootEntry, value );
    }
}