using System.Collections.ObjectModel;
using System.IO;

using DcsTranslateTool.Core.Models;

namespace DcsTranslateTool.Win.ViewModels;

/// <summary>
/// ファイルツリー表示用の ViewModel
/// </summary>
public class FileTreeItemViewModel : BindableBase {
    private bool _isChecked;

    /// <summary>
    /// 名称を取得する
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 絶対パスを取得する
    /// </summary>
    public string AbsolutePath { get; }

    /// <summary>
    /// ディレクトリかどうかを取得する
    /// </summary>
    public bool IsDirectory { get; }

    /// <summary>
    /// 子ノードを取得する
    /// </summary>
    public ObservableCollection<FileTreeItemViewModel> Children { get; } = [];

    /// <summary>
    /// チェック状態を取得または設定する
    /// ディレクトリの場合は子要素へ状態が伝搬する
    /// </summary>
    public bool IsChecked {
        get => _isChecked;
        set {
            if(SetProperty( ref _isChecked, value ) && IsDirectory) {
                foreach(var child in Children) {
                    child.SetCheckedRecursive( value );
                }
            }
        }
    }

    /// <summary>
    /// <see cref="FileTreeItemViewModel"/> の新しいインスタンスを生成する
    /// </summary>
    /// <param name="tree">基となる <see cref="FileTree"/></param>
    public FileTreeItemViewModel( FileTree? tree = null ) {
        Name = tree?.Name ?? "初期値";
        AbsolutePath = tree?.AbsolutePath ?? "初期値";
        IsDirectory = tree?.IsDirectory ?? false;

        tree?.Children.ForEach( child => Children.Add( new FileTreeItemViewModel( child ) ) );
    }

    /// <summary>
    /// チェック状態を再帰的に設定する
    /// </summary>
    /// <param name="value">設定する値</param>
    public void SetCheckedRecursive( bool value ) {
        _isChecked = value;
        RaisePropertyChanged( nameof( IsChecked ) );
        foreach(var child in Children) {
            child.SetCheckedRecursive( value );
        }
    }

    /// <summary>
    /// チェックされたファイル要素を列挙する
    /// </summary>
    /// <returns>チェック済みファイルの列挙</returns>
    public IEnumerable<FileTreeItemViewModel> GetCheckedFiles() {
        if(IsChecked && !IsDirectory) yield return this;
        foreach(var child in Children) {
            foreach(var c in child.GetCheckedFiles()) {
                yield return c;
            }
        }
    }

    /// <summary>
    /// 子要素を更新する
    /// </summary>
    public void UpdateChildren() {
        if(!IsDirectory) return;

        var childrenPath = Directory.EnumerateFileSystemEntries( AbsolutePath ).Select( f => new FileTree
        {
            Name = Path.GetFileName( f ),
            AbsolutePath = f,
            IsDirectory = Directory.Exists( f ),
        } )
        .OrderBy( f => f.Name )
        .ToList();

        Children.Clear();
        childrenPath.ForEach( child => Children.Add( new FileTreeItemViewModel( child ) ) );
    }
}
