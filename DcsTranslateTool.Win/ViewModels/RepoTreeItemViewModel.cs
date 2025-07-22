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
    public string Name => this.Model.Name;

    /// <summary>
    /// 絶対パスを取得する
    /// </summary>
    public string AbsolutePath => this.Model.AbsolutePath;

    /// <summary>
    /// ディレクトリかどうかを取得する
    /// </summary>
    public bool IsDirectory => this.Model.IsDirectory;

    /// <summary>
    /// 子ノードを取得する
    /// </summary>
    public ObservableCollection<RepoTreeItemViewModel> Children { get; }

    /// <summary>
    /// モデル本体を必要に応じて取得できるプロパティ
    /// </summary>
    public RepoTree Model { get; }

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
    /// <param name="model">基となる <see cref="RepoTree"/></param>
    public RepoTreeItemViewModel( RepoTree model ) {
        this.Model = model;

        Children = new ObservableCollection<RepoTreeItemViewModel>(
            this.Model.Children.Select( c => new RepoTreeItemViewModel( c ) ) );
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
