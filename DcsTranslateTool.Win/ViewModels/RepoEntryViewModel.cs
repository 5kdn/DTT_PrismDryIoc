using System.Collections.ObjectModel;

using DcsTranslateTool.Core.Enums;
using DcsTranslateTool.Core.Models;

namespace DcsTranslateTool.Win.ViewModels;

/// <summary>
/// リポジトリのエントリを表す ViewModel である。
/// </summary>
/// <param name="model">元となるモデル</param>
public class RepoEntryViewModel(
    Entry model
    ) : BindableBase {
    #region Fields

    private bool isSelected;
    private bool isExpanded;

    #endregion

    #region Properties

    /// <summary>
    /// ファイル名・フォルダ名を取得する
    /// </summary>
    public string Name => this.Model.Name;

    /// <summary>
    /// リポジトリルートからのパスを取得する
    /// </summary>
    public string Path => this.Model.Path;

    /// <summary>
    /// ファイルの変更種別を取得する。
    /// </summary>
    public FileChangeType ChangeType => this.Model.ChangeType;

    /// <summary>
    /// ディレクトリかどうかを判定する
    /// </summary>
    public bool IsDirectory => this.Model.IsDirectory;

    /// <summary>
    /// 実際の<see cref="Entry"/>を取得する。
    /// </summary>
    public Entry Model { get; } = model;

    /// <summary>
    /// チェックボックス選択されているかどうかを取得または設定する
    /// </summary>
    public bool IsSelected {
        get => isSelected;
        set {
            SetProperty( ref isSelected, value );
            // TODO: 一部選択状態を追加
            if(IsDirectory) {
                foreach(var child in Children) {
                    if(child is not null) child.IsSelected = value;
                }
            }
        }
    }

    /// <summary>
    /// 展開しているかを取得または設定する
    /// </summary>
    public bool IsExpanded {
        get => isExpanded;
        set => SetProperty( ref isExpanded, value );
    }

    /// <summary>
    /// ツリー構造の子要素を取得する
    /// </summary>
    public ObservableCollection<RepoEntryViewModel> Children { get; } = [];

    /// <summary>
    /// <see cref="IsSelected"/>の状態を再帰的に設定する
    /// </summary>
    /// <param name="value">状態</param>
    public void SetSelectRecursive( bool value ) {
        IsSelected = value;
        foreach(var child in Children) {
            child.SetSelectRecursive( value );
        }
    }

    /// <summary>
    /// セレクト状態の子要素の<see cref="Entry"/>を再帰的に取得する。
    /// </summary>
    /// <returns>セレクト状態の子要素のリスト</returns>
    public List<Entry> GetCheckedModelRecursice() {
        List<Entry> checkedChildrenModels = [];

        if(IsSelected) checkedChildrenModels.Add( Model );

        foreach(var child in Children) {
            checkedChildrenModels.AddRange( child.GetCheckedModelRecursice() );
        }
        return checkedChildrenModels;
    }
    #endregion
}