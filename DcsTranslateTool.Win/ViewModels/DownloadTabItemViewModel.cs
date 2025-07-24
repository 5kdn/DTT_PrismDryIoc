using DcsTranslateTool.Core.Models;

namespace DcsTranslateTool.Win.ViewModels;
public class DownloadTabItemViewModel( string title, RepoEntryViewModel rootEntry ) : BindableBase {
    public string Title { get; } = title;

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
    /// チェック状態のエントリを取得するメソッド
    /// </summary>
    /// <returns><see cref="RepoEntry"/>のリスト</returns>
    public List<RepoEntry> GetCheckedEntries() => Root.GetCheckedModelRecursice();
}
