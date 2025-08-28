using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.Enums;
using DcsTranslateTool.Win.Extensions;

namespace DcsTranslateTool.Win.ViewModels;
public class DownloadTabItemViewModel( RootTabType tabType, RepoEntryViewModel rootEntry ) : BindableBase {
    public RootTabType TabType { get; } = tabType;

    public string Title { get; } = tabType.GetTabTitle();

    public RepoEntryViewModel Root {
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
    /// <returns><see cref="Entry"/>のリスト</returns>
    public List<Entry> GetCheckedEntries() => Root.GetCheckedModelRecursice();
}