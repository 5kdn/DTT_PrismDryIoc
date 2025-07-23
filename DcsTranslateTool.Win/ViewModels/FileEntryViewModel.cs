using System.Collections.ObjectModel;

using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.Contracts.ViewModels.Factories;

namespace DcsTranslateTool.Win.ViewModels;

public class FileEntryViewModel : BindableBase {
    private readonly IFileEntryViewModelFactory _factory;
    private readonly IFileEntryService _fileEntryService;
    private bool isSelected;
    private bool isExpanded;
    private bool childrenLoaded;

    public string Name => this.Model.Name;
    public string AbsolutePath => this.Model.AbsolutePath;
    public bool IsDirectory => this.Model.IsDirectory;

    public FileEntry Model { get; }

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

    public bool IsExpanded {
        get => isExpanded;
        set {
            if(SetProperty( ref isExpanded, value ) && value) LoadChildren();
        }
    }

    public bool IsChildrenLoaded {
        get => childrenLoaded;
        set => SetProperty( ref childrenLoaded, value );
    }

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
