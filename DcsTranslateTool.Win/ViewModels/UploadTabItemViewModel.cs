using DcsTranslateTool.Win.Enums;
using DcsTranslateTool.Win.Extensions;
using Prism.Mvvm;

namespace DcsTranslateTool.Win.ViewModels;

/// <summary>
/// Uploadページのタブを表す ViewModel である。
/// </summary>
/// <param name="tabType">タブ種別</param>
/// <param name="rootEntry">ルートエントリ</param>
public class UploadTabItemViewModel( RootTabType tabType, EntryViewModel rootEntry ) : BindableBase {
    /// <summary>
    /// タブタイトルを取得する。
    /// </summary>
    public string Title { get; } = tabType.GetTabTitle();

    /// <summary>
    /// ルートエントリを取得または設定する。
    /// </summary>
    public EntryViewModel Root {
        get => rootEntry;
        set => SetProperty( ref rootEntry, value );
    }
}
