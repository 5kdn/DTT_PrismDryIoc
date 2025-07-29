using DcsTranslateTool.Win.Enums;
using DcsTranslateTool.Win.Extensions;

namespace DcsTranslateTool.Win.ViewModels;

public class UploadTabItemViewModel( RootTabType tabType, FileEntryViewModel rootEntry ) : BindableBase {
    /// <summary>
    /// タブ種別を取得する
    /// </summary>
    public RootTabType TabType { get; } = tabType;

    public string Title { get; } = tabType.GetTabTitle();

    public FileEntryViewModel Root {
        get => rootEntry;
        set => SetProperty( ref rootEntry, value );
    }
}
