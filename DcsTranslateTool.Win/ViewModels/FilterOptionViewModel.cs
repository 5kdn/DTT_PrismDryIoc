using DcsTranslateTool.Core.Enums;

namespace DcsTranslateTool.Win.ViewModels;

/// <summary>
/// ダウンロードページのフィルタ項目の ViewModel である。
/// </summary>
/// <param name="displayName">表示名</param>
/// <param name="changeType">対象の <see cref="FileChangeType"/>。全てを示すときは <see langword="null"/> を指定する。</param>
public class FilterOptionViewModel( string displayName, FileChangeType? changeType ) : BindableBase {
    /// <summary>
    /// 表示名を取得する。
    /// </summary>
    public string DisplayName { get; } = displayName;

    /// <summary>
    /// 対象の <see cref="FileChangeType"/> を取得する。
    /// </summary>
    public FileChangeType? ChangeType { get; } = changeType;

    private bool isChecked;

    /// <summary>
    /// 選択状態を取得または設定する。
    /// </summary>
    public bool IsChecked {
        get => isChecked;
        set {
            if(SetProperty( ref isChecked, value )) CheckedChanged?.Invoke( this );
        }
    }

    /// <summary>
    /// 選択状態変更時に発生するイベントである。
    /// </summary>
    public event Action<FilterOptionViewModel>? CheckedChanged;
}