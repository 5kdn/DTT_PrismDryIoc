using System.Collections.ObjectModel;

using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.Contracts.ViewModels;
using DcsTranslateTool.Win.Contracts.ViewModels.Factories;

namespace DcsTranslateTool.Win.ViewModels;

/// <inheritdoc/>
public class FileEntryViewModel : BindableBase, IFileEntryViewModel {
    private readonly IFileEntryViewModelFactory _factory;
    private readonly IFileEntryService _fileEntryService;
    private bool isSelected;
    private bool isExpanded;
    private bool childrenLoaded;

    /// <inheritdoc/>
    public string Name => this.Model.Name;

    /// <inheritdoc/>
    public string AbsolutePath => this.Model.AbsolutePath;

    /// <inheritdoc/>
    public bool IsDirectory => this.Model.IsDirectory;

    /// <inheritdoc/>
    public FileEntry Model { get; }

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
    public ObservableCollection<FileEntryViewModel?> Children { get; } = [];

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
        Children.Clear();
        try {
            foreach(var child in _fileEntryService.GetChildren( this.Model )) {
                Children.Add( _factory.Create( child ) );
            }
        }
        catch {
            // TODO: エラーハンドリング
        }
        RaisePropertyChanged( nameof( Children ) );
    }
}
