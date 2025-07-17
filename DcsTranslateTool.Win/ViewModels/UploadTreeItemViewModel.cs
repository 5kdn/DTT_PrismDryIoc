using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;

using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Models;

namespace DcsTranslateTool.Win.ViewModels;

/// <summary>
/// アップロード用ファイルツリーを表示する ViewModel である。
/// </summary>
public class UploadTreeItemViewModel : BindableBase {
    private bool? _isChecked;

    /// <summary>
    /// 親要素を取得する。
    /// </summary>
    public UploadTreeItemViewModel? Parent { get; }

    /// <summary>
    /// 名称を取得する。
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 絶対パスを取得する。
    /// </summary>
    public string AbsolutePath { get; }

    /// <summary>
    /// ディレクトリかどうかを取得する。
    /// </summary>
    public bool IsDirectory { get; }

    /// <summary>
    /// 子要素を取得する。
    /// </summary>
    public ObservableCollection<UploadTreeItemViewModel> Children { get; } = new();

    /// <summary>
    /// チェック状態を取得または設定する。
    /// </summary>
    public bool? IsChecked {
        get => _isChecked;
        set {
            if(SetProperty(ref _isChecked, value)) {
                if(value.HasValue) {
                    foreach(var child in Children) {
                        child.SetCheckedRecursive(value.Value);
                    }
                }
                Parent?.UpdateCheckedStateFromChildren();
            }
        }
    }

    /// <summary>
    /// <see cref="UploadTreeItemViewModel"/> の新しいインスタンスを生成する。
    /// </summary>
    /// <param name="tree">元となる <see cref="FileTree"/></param>
    /// <param name="parent">親要素</param>
    public UploadTreeItemViewModel( FileTree tree, UploadTreeItemViewModel? parent = null ) {
        Name = tree.Name;
        AbsolutePath = tree.AbsolutePath;
        IsDirectory = tree.IsDirectory;
        Parent = parent;
    }

    /// <summary>
    /// 子要素を更新する。
    /// </summary>
    /// <param name="fileService">ファイルサービス</param>
    public void UpdateChildren( IFileService fileService ) {
        if(!IsDirectory) return;
        FileTree tree = fileService.GetFileTree( AbsolutePath );
        Children.Clear();
        foreach(FileTree child in tree.Children) {
            Children.Add( new UploadTreeItemViewModel( child, this ) );
        }
    }

    /// <summary>
    /// チェック状態を再帰的に設定する。
    /// </summary>
    /// <param name="value">設定する値</param>
    public void SetCheckedRecursive( bool value ) {
        _isChecked = value;
        RaisePropertyChanged( nameof(IsChecked) );
        foreach(var child in Children) {
            child.SetCheckedRecursive( value );
        }
    }

    private void UpdateCheckedStateFromChildren() {
        if(Children.Count == 0) return;
        bool allChecked = Children.All( c => c.IsChecked == true );
        bool allUnchecked = Children.All( c => c.IsChecked == false );
        _isChecked = allChecked ? true : allUnchecked ? false : null;
        RaisePropertyChanged( nameof(IsChecked) );
        Parent?.UpdateCheckedStateFromChildren();
    }

    /// <summary>
    /// チェックされたファイル要素を列挙する。
    /// </summary>
    /// <returns>チェック済みファイルの列挙</returns>
    public IEnumerable<UploadTreeItemViewModel> GetCheckedFiles() {
        if(IsChecked == true && !IsDirectory) {
            yield return this;
        }
        foreach(var child in Children) {
            foreach(var c in child.GetCheckedFiles()) {
                yield return c;
            }
        }
    }
}
