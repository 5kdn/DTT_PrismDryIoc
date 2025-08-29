using DcsTranslateTool.Core.Enums;
using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.Enums;
using DcsTranslateTool.Win.Extensions;
using Prism.Mvvm;

namespace DcsTranslateTool.Win.ViewModels;

/// <summary>
/// Downloadページのタブを表す ViewModel である。
/// </summary>
/// <param name="tabType">タブ種別</param>
/// <param name="rootEntry">ルートエントリ</param>
public class DownloadTabItemViewModel( RootTabType tabType, EntryViewModel rootEntry ) : BindableBase {
    /// <summary>
    /// タブの種別を取得する。
    /// </summary>
    public RootTabType TabType { get; } = tabType;

    /// <summary>
    /// タブのタイトルを取得する。
    /// </summary>
    public string Title { get; } = tabType.GetTabTitle();

    private EntryViewModel _root = rootEntry;

    /// <summary>
    /// 表示用ルートを取得または設定する。
    /// </summary>
    public EntryViewModel Root {
        get => _root;
        set => SetProperty( ref _root, value );
    }

    /// <summary>
    /// 元のルートを取得する。
    /// </summary>
    public EntryViewModel OriginalRoot { get; private set; } = rootEntry;

    /// <summary>
    /// ルート情報を更新する。
    /// </summary>
    /// <param name="root">新しいルート</param>
    public void UpdateRoot( EntryViewModel root ) {
        OriginalRoot = root;
        Root = root;
    }

    /// <summary>
    /// フィルタを適用する。
    /// </summary>
    /// <param name="types">表示する<see cref="FileChangeType"/>の列挙</param>
    public void ApplyFilter( IEnumerable<FileChangeType> types ) {
        if(types is null || !types.Any()) {
            Root = OriginalRoot;
            return;
        }
        Root = FilterRecursive( OriginalRoot, types ) ?? new EntryViewModel( new Entry( "", "", true ) );
    }

    private static EntryViewModel? FilterRecursive( EntryViewModel source, IEnumerable<FileChangeType> types ) {
        var matchedChildren = source.Children
            .OfType<EntryViewModel>()
            .Select( child => FilterRecursive( child, types ) )
            .Where( child => child is not null )
            .ToList();

        bool includeSelf = types.Contains( source.ChangeType );
        if(!includeSelf && matchedChildren.Count == 0) return null;

        var clone = new EntryViewModel( source.Model ) { IsChildrenLoaded = true };
        foreach(var child in matchedChildren) {
            if(child is not null) clone.Children.Add( child );
        }
        return clone;
    }

    /// <summary>
    /// チェック状態を再帰的に設定するメソッド
    /// </summary>
    /// <param name="value">設定するチェック状態</param>
    public void SetCheckRecursive( bool value ) =>
        Root.CheckState = value ? CheckState.Checked : CheckState.Unchecked;

    /// <summary>
    /// チェック状態のエントリを取得するメソッド。
    /// </summary>
    /// <returns><see cref="Entry"/>のリスト</returns>
    public List<Entry> GetCheckedEntries() => Root.GetCheckedModelRecursice();
}