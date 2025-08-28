using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.Enums;

namespace DcsTranslateTool.Win.ViewModels;
public class RepoEntryViewModel(
    RepoEntry model,
    DownloadStatus status = DownloadStatus.NotDownloaded
    ) : BindableBase {
    #region Fields

    private bool isSelected;
    private bool isExpanded;
    private bool isVisible = true;
    private DownloadStatus entryStatus = status;

    #endregion

    #region Properties

    /// <summary>
    /// ファイル名・フォルダ名を取得する
    /// </summary>
    public string Name => Model.Name;

    /// <summary>
    /// リポジトリ上の絶対パスを取得する
    /// </summary>
    public string AbsolutePath => Model.AbsolutePath;

    /// <summary>
    /// ディレクトリかどうかを判定する
    /// </summary>
    public bool IsDirectory => Model.IsDirectory;

    /// <summary>
    /// 実際の<see cref="RepoEntry"/>を取得する
    /// </summary>
    public RepoEntry Model { get; } = model;

    /// <summary>
    /// ダウンロード状態を取得または設定する
    /// </summary>
    public DownloadStatus Status {
        get => entryStatus;
        set => SetProperty(ref entryStatus, value);
    }

    /// <summary>
    /// チェックボックス選択されているかどうかを取得または設定する
    /// </summary>
    public bool IsSelected {
        get => isSelected;
        set {
            SetProperty(ref isSelected, value);
            // TODO: 一部選択状態を追加
            if(IsDirectory) {
                foreach(var child in Children) {
                    child.IsSelected = value;
                }
            }
        }
    }

    /// <summary>
    /// 展開しているかを取得または設定する
    /// </summary>
    public bool IsExpanded {
        get => isExpanded;
        set => SetProperty(ref isExpanded, value);
    }

    /// <summary>
    /// ツリー構造の子要素を取得する
    /// </summary>
    public ObservableCollection<RepoEntryViewModel> Children { get; } = [];

    /// <summary>
    /// 表示するかどうかを取得または設定する
    /// </summary>
    public bool IsVisible {
        get => isVisible;
        set => SetProperty(ref isVisible, value);
    }

    /// <summary>
    /// <see cref="IsSelected"/>の状態を再帰的に設定する
    /// </summary>
    /// <param name="value">状態</param>
    public void SetSelectRecursive(bool value) {
        IsSelected = value;
        foreach(var child in Children) {
            child.SetSelectRecursive(value);
        }
    }

    /// <summary>
    /// セレクト状態の子要素の<see cref="RepoEntry"/>を再帰的に取得する
    /// </summary>
    /// <returns>セレクト状態の子要素のリスト</returns>
    public List<RepoEntry> GetCheckedModelRecursice() {
        List<RepoEntry> checkedChildrenModels = [];

        if(IsSelected) checkedChildrenModels.Add(Model);

        foreach(var child in Children) {
            checkedChildrenModels.AddRange(child.GetCheckedModelRecursice());
        }
        return checkedChildrenModels;
    }

    /// <summary>
    /// フィルターを適用して<see cref="IsVisible"/>を設定する
    /// </summary>
    /// <param name="filter">適用するフィルター</param>
    public void ApplyFilter(DownloadFilterType filter) {
        foreach(var child in Children) {
            child.ApplyFilter(filter);
        }

        bool match = filter switch {
            DownloadFilterType.All => true,
            DownloadFilterType.Downloaded => Status == DownloadStatus.Downloaded,
            DownloadFilterType.NotDownloaded => Status == DownloadStatus.NotDownloaded,
            DownloadFilterType.New => Status == DownloadStatus.New,
            DownloadFilterType.Updated => Status == DownloadStatus.Updated,
            _ => true,
        };

        IsVisible = IsDirectory ? Children.Any(c => c.IsVisible) : match;
    }

    #endregion
}
