using DcsTranslateTool.Win.Contracts.ViewModels;
using DcsTranslateTool.Win.Enums;
using DcsTranslateTool.Win.Extensions;

namespace DcsTranslateTool.Win.ViewModels;

public class UploadTabItemViewModel( RootTabType tabType, IFileEntryViewModel rootEntry ) : BindableBase {
    public RootTabType TabType { get; } = tabType;
    public string Title { get; } = tabType.GetTabTitle();

    public IFileEntryViewModel Root {
        get => rootEntry;
        set => SetProperty( ref rootEntry, value );
    }
}