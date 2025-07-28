using System.Collections.ObjectModel;
using System.Collections.Generic;

using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.Contracts.ViewModels;
using DcsTranslateTool.Win.Contracts.ViewModels.Factories;
using DcsTranslateTool.Win.Enums;

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
    public FileEntry Model { get; }

    /// <inheritdoc/>
    public CheckState CheckState {
        get => checkState;
        set {
            if(!SetProperty( ref checkState, value )) return;
            if(IsDirectory && value != CheckState.Indeterminate) {
                foreach(var child in Children) {
                    if(child is not null) child.CheckState = value;
                }
            }
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

    public FileEntryViewModel(
        IFileEntryViewModelFactory factory,
        IFileEntryService fileEntryService,
        FileEntry model ) {
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
        }
        Children.Clear();
        foreach(var child in result.Value!) {
            var childVm = _factory.Create( child, CheckState );
            childVm.CheckStateChanged += (_, _) => CheckStateChanged?.Invoke( childVm, childVm.CheckState );
            Children.Add( childVm );
        }

        RaisePropertyChanged( nameof( Children ) );
    }

    /// <summary>
    /// 選択状態の子要素の <see cref="FileEntry"/> を再帰的に取得する
    /// </summary>
    /// <returns>選択状態の <see cref="FileEntry"/> の一覧</returns>
    public List<FileEntry> GetCheckedModelRecursice() {
        List<FileEntry> checkedChildrenModels = [];
        if(CheckState == CheckState.Checked) checkedChildrenModels.Add( Model );

        foreach(var child in Children) {
            if(child is null) continue;
            checkedChildrenModels.AddRange( child.GetCheckedModelRecursice() );
        }
        return checkedChildrenModels;
    }
}
