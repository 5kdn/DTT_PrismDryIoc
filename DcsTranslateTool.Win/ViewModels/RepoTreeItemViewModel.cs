using System.Collections.ObjectModel;
using System.Collections.Generic;
using DcsTranslateTool.Share.Models;
using Prism.Mvvm;

namespace DcsTranslateTool.Win.ViewModels;

/// <summary>
/// リポジトリツリー表示用の ViewModel である。
/// </summary>
public class RepoTreeItemViewModel : BindableBase {
    private bool _isChecked;

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
    public ObservableCollection<RepoTreeItemViewModel> Children { get; } = new();

    /// <summary>
    /// チェック状態を取得または設定する。
    /// 設定時はディレクトリの場合に子要素へ伝搬する。
    /// </summary>
    public bool IsChecked {
        get => _isChecked;
        set {
            if(SetProperty(ref _isChecked, value) && IsDirectory) {
                foreach(var child in Children) {
                    child.SetCheckedRecursive(value);
                }
            }
        }
    }

    /// <summary>
    /// <see cref="RepoTreeItemViewModel"/> の新しいインスタンスを生成する。
    /// </summary>
    /// <param name="tree">元となる <see cref="RepoTree"/></param>
    public RepoTreeItemViewModel( RepoTree tree ) {
        Name = tree.Name;
        AbsolutePath = tree.AbsolutePath;
        IsDirectory = tree.IsDirectory;
        foreach(var child in tree.Children) {
            Children.Add( new RepoTreeItemViewModel( child ) );
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

    /// <summary>
    /// チェックされたファイル要素を列挙する。
    /// </summary>
    /// <returns>チェック済みファイルの列挙</returns>
    public IEnumerable<RepoTreeItemViewModel> GetCheckedFiles() {
        if(IsChecked && !IsDirectory) {
            yield return this;
        }
        foreach(var child in Children) {
            foreach(var c in child.GetCheckedFiles()) {
                yield return c;
            }
        }
    }
}
