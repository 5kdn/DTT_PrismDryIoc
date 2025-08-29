using System.Collections.ObjectModel;

using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.Contracts.ViewModels;
using DcsTranslateTool.Win.Enums;
using DcsTranslateTool.Win.Extensions;

namespace DcsTranslateTool.Win.ViewModels;

/// <inheritdoc/>
public class FileEntryViewModel( FileEntry model ) : BindableBase, IFileEntryViewModel {

    #region Fields

    private CheckState checkState;
    private bool isSelected;
    private bool isExpanded;

    #endregion

    #region Properties

    /// <summary>
    /// 選択状態が変更されたときに通知するイベント
    /// </summary>
    public event EventHandler<CheckState>? CheckStateChanged;

    /// <inheritdoc/>
    public string Name => this.Model.Name;

    /// <inheritdoc/>
    public string Path => this.Model.Path;

    /// <inheritdoc/>
    public bool IsDirectory => this.Model.IsDirectory;

    /// <inheritdoc/>
    public FileEntry Model { get; } = model;

    /// <inheritdoc/>
    public FileChangeType? ChangeType {
        get {
            // TODO: 実装
            return null;
        }
    }

    /// <inheritdoc/>
    public CheckState CheckState {
        get => checkState;
        set {
            if(!SetProperty( ref checkState, value )) return;

            // 親->子への伝播
            if(IsDirectory && value != CheckState.Indeterminate) {
                foreach(var child in Children) {
                    if(child is not null && child.CheckState != value) child.CheckState = value;
                }
            }


            // 子->親への伝播
            Parent?.UpdateCheckStateFromChildren();

            CheckStateChanged?.Invoke( this, value );
        }
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public bool IsExpanded {
        get => isExpanded;
        set => SetProperty( ref isExpanded, value );
    }

    /// <inheritdoc/>
    public ObservableCollection<IFileEntryViewModel?> Children { get; } = [];

    /// <inheritdoc/>
    public IFileEntryViewModel? Parent { get; set; }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public void SetSelectRecursive( bool value ) {
        IsSelected = value;
        foreach(var child in Children) {
            child?.SetSelectRecursive( value );
        }
    }

    /// <inheritdoc/>
    public List<FileEntry> GetCheckedModelRecursice( bool fileOnly = false ) {
        List<FileEntry> checkedChildrenModels = [];

        switch(CheckState.IsSelectedLike(), IsDirectory, fileOnly) {
            case (true, false, _ ):
            case (true, true, false ):
                checkedChildrenModels.Add( Model );
                break;
        }

        foreach(var child in Children) {
            if(child is not null) checkedChildrenModels.AddRange( child.GetCheckedModelRecursice() );
        }

        return checkedChildrenModels;
    }

    /// <inheritdoc/>
    public void UpdateCheckStateFromChildren() {
        if(!IsDirectory || Children.Count == 0) return;

        var allChecked = Children.All(c => c?.CheckState == CheckState.Checked);
        var allUnchecked = Children.All(c => c?.CheckState == CheckState.Unchecked);

        // 全ての子がチェックされている場合はChecked
        // 全ての子がチェックされていない場合はUnchecked
        // それ以外はIndeterminate
        var newState = allChecked ? CheckState.Checked :
            allUnchecked ? CheckState.Unchecked :
            CheckState.Indeterminate;

        if(checkState == newState) return;
        checkState = newState;
        RaisePropertyChanged( nameof( CheckState ) );
        CheckStateChanged?.Invoke( this, checkState );

        // 祖先にも伝播
        Parent?.UpdateCheckStateFromChildren();
    }

    #endregion
}