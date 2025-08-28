using System.Collections.ObjectModel;

using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.Contracts.ViewModels;
using DcsTranslateTool.Win.Contracts.ViewModels.Factories;
using DcsTranslateTool.Win.Enums;
using DcsTranslateTool.Win.Extensions;

namespace DcsTranslateTool.Win.ViewModels;

/// <inheritdoc/>
public class FileEntryViewModel : BindableBase, IFileEntryViewModel {
    private readonly IFileEntryViewModelFactory _factory;
    private readonly IFileEntryService _fileEntryService;
    private CheckState checkState;
    private bool isExpanded;
    private bool childrenLoaded;

    /// <summary>
    /// 選択状態が変更されたときに通知するイベント
    /// </summary>
    public event EventHandler<CheckState>? CheckStateChanged;

    /// <inheritdoc/>
    public string Name => this.Model.Name;

    /// <inheritdoc/>
    public string AbsolutePath => this.Model.AbsolutePath;

    /// <inheritdoc/>
    public bool IsDirectory => this.Model.IsDirectory;

    /// <inheritdoc/>
    public Entry Model { get; }

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
    public bool IsExpanded {
        get => isExpanded;
        set {
            if(SetProperty( ref isExpanded, value ) && value) LoadChildren();
        }
    }

    /// <inheritdoc/>
    public bool IsChildrenLoaded {
        get => childrenLoaded;
        set => SetProperty( ref childrenLoaded, value );
    }

    /// <inheritdoc/>
    public ObservableCollection<IFileEntryViewModel?> Children { get; } = [];

    /// <summary>
    /// 親ノード（CheckStateの伝播用）
    /// </summary>
    public FileEntryViewModel? Parent { get; set; }

    public FileEntryViewModel(
        IFileEntryViewModelFactory factory,
        IFileEntryService fileEntryService,
        Entry model ) {
        _factory = factory;
        _fileEntryService = fileEntryService;
        this.Model = model;
        if(model.IsDirectory) Children.Add( null );     // Placeholder for lazy loading
    }

    /// <inheritdoc/>
    public void LoadChildren() {
        if(!this.Model.IsDirectory || childrenLoaded) return;
        childrenLoaded = true;
        var result = _fileEntryService.GetChildren( this.Model );
        if(!result.IsSuccess) {
            // TODO: エラーハンドリング
            return;
        }
        Children.Clear();
        foreach(var child in result.Value!) {
            var childVm = _factory.Create( child, this, CheckState );
            childVm.CheckStateChanged += ( _, _ ) => CheckStateChanged?.Invoke( childVm, childVm.CheckState );
            Children.Add( childVm );
        }

        RaisePropertyChanged( nameof( Children ) );
    }

    /// <summary>
    /// 子要素の状態を確認して自身のCheckStateを更新する
    /// </summary>
    private void UpdateCheckStateFromChildren() {
        if(!IsDirectory || Children.Count == 0) return;

        var allChecked = Children.All(c => c?.CheckState == CheckState.Checked);
        var allUnchecked = Children.All(c => c?.CheckState == CheckState.Unchecked);

        var newState = Children.All(c => c?.CheckState == CheckState.Checked) ? CheckState.Checked :
            Children.All(c => c?.CheckState == CheckState.Unchecked) ? CheckState.Unchecked :
            CheckState.Indeterminate;

        if(checkState == newState) return;
        checkState = newState;
        RaisePropertyChanged( nameof( CheckState ) );
        CheckStateChanged?.Invoke( this, checkState );

        // 祖先にも伝播
        Parent?.UpdateCheckStateFromChildren();
    }

    /// <summary>
    /// 選択状態の子要素の <see cref="Entry"/> を再帰的に取得する。
    /// </summary>
    /// <returns>選択状態の <see cref="Entry"/> の一覧</returns>
    public List<Entry> GetCheckedModelRecursice() {
        List<Entry> checkedChildrenModels = [];

        if(CheckState.IsSelectedLike() && !IsDirectory) {
            checkedChildrenModels.Add( Model );
        }

        if(!IsChildrenLoaded && CheckState.IsSelectedLike()) {
            LoadChildren();
        }

        foreach(var child in Children) {
            if(child is null) continue;
            checkedChildrenModels.AddRange( child.GetCheckedModelRecursice() );
        }

        return checkedChildrenModels;
    }
}