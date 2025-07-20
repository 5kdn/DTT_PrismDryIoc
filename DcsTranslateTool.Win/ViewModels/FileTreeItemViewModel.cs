using System.Collections.ObjectModel;

using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Models;

namespace DcsTranslateTool.Win.ViewModels;

/// <summary>
/// ファイルツリー表示用の ViewModel
/// </summary>
public class FileTreeItemViewModel : BindableBase {
    private bool _isChecked;

    /// <summary>
    /// 名前
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 絶対パス
    /// </summary>
    public string AbsolutePath { get; }

    /// <summary>
    /// ディレクトリかどうか
    /// </summary>
    public bool IsDirectory { get; }

    /// <summary>
    /// チェック状態
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
    /// 子ノード
    /// </summary>
    public ObservableCollection<FileTreeItemViewModel> Children { get; } = new();

    /// <summary>
    /// <see cref="FileTreeItemViewModel"/> の新しいインスタンスを生成する
    /// </summary>
    /// <param name="tree">基となる <see cref="FileTree"/></param>
    public FileTreeItemViewModel( FileTree tree ) {
        Name = tree.Name;
        AbsolutePath = tree.AbsolutePath;
        IsDirectory = tree.IsDirectory;
        Children = new ObservableCollection<FileTreeItemViewModel>(
            tree.Children.Select( child => new FileTreeItemViewModel( child ) )
        );
    }

    /// <summary>
    /// 子要素を更新する
    /// </summary>
    /// <param name="fileService">ファイルサービス</param>
    public void UpdateChildren( IFileService fileService ) {
        if(!IsDirectory) return;
        FileTree tree = fileService.GetFileTree( AbsolutePath );
        Children.Clear();
        foreach(FileTree child in tree.Children) {
            Children.Add( new FileTreeItemViewModel( child ) );
        }
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
}
