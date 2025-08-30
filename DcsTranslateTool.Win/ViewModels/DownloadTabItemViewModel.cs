using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.Contracts.ViewModels;
using DcsTranslateTool.Win.Enums;
using DcsTranslateTool.Win.Extensions;

namespace DcsTranslateTool.Win.ViewModels;
public class DownloadTabItemViewModel( RootTabType tabType, IFileEntryViewModel rootEntry ) : BindableBase {
    public RootTabType TabType { get; } = tabType;
    public string Title { get; } = tabType.GetTabTitle();

    public IFileEntryViewModel Root {
        get => rootEntry;
        set => SetProperty( ref rootEntry, value );
    }

    /// <summary>
    /// チェック状態を再帰的に設定するメソッド
    /// </summary>
    /// <param name="value">設定するチェック状態</param>
    public void SetCheckRecursive( bool value ) => Root.SetSelectRecursive( value );

    /// <summary>
    /// チェック状態のエントリを取得するメソッド。
    /// </summary>
    /// <returns><see cref="FileEntry"/>のリスト</returns>
    public List<FileEntry> GetCheckedEntries() => Root.GetCheckedModelRecursive();
}