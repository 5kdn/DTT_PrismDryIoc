using System.Collections.ObjectModel;

using DcsTranslateTool.Share.Models;

namespace DcsTranslateTool.Win.ViewModels;

/// <summary>
/// リポジトリツリー表示用の ViewModel
/// </summary>
public class RepoTreeItemViewModel : BindableBase {
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
    public bool IsDirectory {
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
    /// 子ノードを取得する
    /// </summary>
    public ObservableCollection<RepoTreeItemViewModel> Children { get; } = [];

    /// <summary>
    /// チェック状態を取得または設定する。
    /// ディレクトリの場合は子要素へ状態が伝搬する。
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
    /// <see cref="RepoTreeItemViewModel"/> の新しいインスタンスを生成する
    /// </summary>
    /// <param name="tree">基となる <see cref="RepoTree"/></param>
    public RepoTreeItemViewModel( RepoTree? tree = null ) {
        Name = tree?.Name ?? "初期値";
        AbsolutePath = tree?.AbsolutePath ?? "初期値";
        IsDirectory = tree?.IsDirectory ?? false;
        tree?.Children.ForEach( child => Children.Add( new RepoTreeItemViewModel( child ) ) );
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
    public IEnumerable<RepoTreeItemViewModel> GetCheckedFiles() {
        if(IsChecked && !IsDirectory) yield return this;
        foreach(var child in Children) {
            foreach(var c in child.GetCheckedFiles()) {
                yield return c;
            }
        }
    }
}
