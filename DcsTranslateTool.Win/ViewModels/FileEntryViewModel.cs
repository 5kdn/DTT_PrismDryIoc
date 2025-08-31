using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.Contracts.ViewModels;
using DcsTranslateTool.Win.Enums;

namespace DcsTranslateTool.Win.ViewModels;

/// <inheritdoc/>
public class FileEntryViewModel : BindableBase, IFileEntryViewModel {

    #region Fields

    private bool? checkState = false;
    private bool isSelected;
    private bool isExpanded;
    private bool isVisible = true;

    private ObservableCollection<IFileEntryViewModel> children = [];
    private bool _suppressCheckPropagation = false;
    private bool _suppressSelectPropagation = false;

    #endregion

    #region Properties

    /// <inheritdoc/>
    public event EventHandler<bool?>? CheckStateChanged;

    /// <inheritdoc/>
    public string Name => this.Model.Name;

    /// <inheritdoc/>
    public string Path => this.Model.Path;

    /// <inheritdoc/>
    public bool IsDirectory => this.Model.IsDirectory;

    /// <inheritdoc/>
    public FileEntry Model { get; }

    /// <inheritdoc/>
    public FileChangeType ChangeType {
        get {
            if(IsDirectory) {
                var count = Children.Count;

                // 子要素が0個
                if(count == 0) return FileChangeType.Unchanged;
                if(count == 1) return Children.First().ChangeType;

                // 直下の子ノードの ChangeType を集約
                var set = new HashSet<FileChangeType>(Children.Select(c => c.ChangeType));
                // 全て同一ならその値
                if(set.Count == 1) return set.Single();

                // 子要素にModified が含まれる
                if(set.Contains( FileChangeType.Modified )) return FileChangeType.Modified;
                // 子要素にAdded, Deletedが両方含まれる
                if(set.Contains( FileChangeType.Added ) && set.Contains( FileChangeType.Deleted )) return FileChangeType.Modified;
                // それ以外（例: Added + Unchanged などの複数種）は Modified とみなす
                return FileChangeType.Modified;
            }
            else {
                return (Model.LocalSha, Model.RepoSha) switch
                {
                    (string l, string r ) when l == r => FileChangeType.Unchanged,
                    (string l, string r ) when l != r => FileChangeType.Modified,
                    (string _, null ) => FileChangeType.Added,
                    (null, string _ ) => FileChangeType.Deleted,
                    _ => FileChangeType.Unchanged
                };
            }
        }
    }

    /// <inheritdoc/>
    public bool? CheckState {
        get => checkState;
        set {
            if(!SetProperty( ref checkState, value )) return;

            // 親->子への伝播
            if(!_suppressCheckPropagation && IsDirectory && value is not null) {
                try {
                    _suppressCheckPropagation = true;
                    foreach(var child in Children) {
                        if(child.CheckState != value) child.CheckState = value;
                    }
                }
                finally {
                    _suppressCheckPropagation = false;
                }
            }

            CheckStateChanged?.Invoke( this, value );
        }
    }

    /// <inheritdoc/>
    public bool IsSelected {
        get => isSelected;
        set {
            if(!SetProperty( ref isSelected, value )) return;
            if(!_suppressSelectPropagation && IsDirectory) {
                try {
                    _suppressSelectPropagation = true;
                    foreach(var child in Children) {
                        if(child.IsSelected != value) child.IsSelected = value;
                    }
                }
                finally {
                    _suppressSelectPropagation = false;
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
    public bool IsVisible {
        get => isVisible;
        set => SetProperty( ref isVisible, value );
    }

    /// <inheritdoc/>
    public ObservableCollection<IFileEntryViewModel> Children {
        get => children;
        set {
            if(ReferenceEquals( children, value )) return;
            DetachChildrenHandlers( children );
            if(SetProperty( ref children, value )) {
                AttachChildrenHandlers( children );
                // 参照入替に伴い集計系を更新
                RaisePropertyChanged( nameof( ChangeType ) );
                RecomputeCheckStateFromChildren();
            }
        }
    }

    #endregion


    public FileEntryViewModel( FileEntry model ) {
        this.Model = model;
        // 初期 children にも購読を張る
        AttachChildrenHandlers( children );
    }

    public void Dispose() {
        DetachChildrenHandlers( children );
        GC.SuppressFinalize( this );
    }

    #region Methods
    /// <inheritdoc/>
    public void SetSelectRecursive( bool value ) {
        IsSelected = value;
        foreach(var child in Children)
            child.SetSelectRecursive( value );
    }

    /// <inheritdoc/>
    public List<FileEntry> GetCheckedModelRecursive( bool fileOnly = false ) {
        List<FileEntry> checkedChildrenModels = [];

        switch(CheckState, IsDirectory, fileOnly) {
            case (true, false, _ ):
            case (true, true, false ):
            case (null, false, _ ):
            case (null, true, false ):
                checkedChildrenModels.Add( Model );
                break;
        }

        foreach(var child in Children)
            checkedChildrenModels.AddRange( child.GetCheckedModelRecursive( fileOnly ) );

        return checkedChildrenModels;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// 子ノードの状態から自分のチェック状態を再計算する
    /// </summary>
    private void RecomputeCheckStateFromChildren() {
        if(!IsDirectory || Children.Count == 0) return;

        var allChecked = Children.All( c => c.CheckState == true );
        var allUnchecked = Children.All( c => c.CheckState == false );

        bool? newState = (allChecked, allUnchecked) switch
        {
            (true, _ ) => true,
            (_, true ) => false,
            _ => null,
        };

        if(checkState == newState) return;
        // 直接フィールドを書き換え、派生通知を出す（無限ループ抑止のため SetProperty は使わない）
        checkState = newState;
        RaisePropertyChanged( nameof( CheckState ) );
        CheckStateChanged?.Invoke( this, checkState );
    }

    /// <summary>既存コレクションから購読を全解除</summary>
    private void DetachChildrenHandlers( ObservableCollection<IFileEntryViewModel> collection ) {
        if(collection == null) return;
        collection.CollectionChanged -= OnChildrenCollectionChanged;
        foreach(var ch in collection) {
            if(ch is INotifyPropertyChanged inpc) inpc.PropertyChanged -= OnChildPropertyChanged;
            ch.CheckStateChanged -= OnChildCheckStateChanged;
        }
    }

    /// <summary>コレクションへ購読を付与</summary>
    private void AttachChildrenHandlers( ObservableCollection<IFileEntryViewModel> collection ) {
        if(collection == null) return;
        collection.CollectionChanged -= OnChildrenCollectionChanged; // 二重防止
        collection.CollectionChanged += OnChildrenCollectionChanged;
        foreach(var ch in collection) {
            if(ch is INotifyPropertyChanged inpc) inpc.PropertyChanged += OnChildPropertyChanged;
            ch.CheckStateChanged += OnChildCheckStateChanged;
        }
    }

    #endregion

    #region Events

    private void OnChildrenCollectionChanged( object? sender, NotifyCollectionChangedEventArgs e ) {
        // Reset は付け直しが安全
        if(e.Action == NotifyCollectionChangedAction.Reset) {
            DetachChildrenHandlers( children );
            AttachChildrenHandlers( children );
            RaisePropertyChanged( nameof( ChangeType ) );
            RecomputeCheckStateFromChildren();
            return;
        }
        if(e.OldItems is not null) {
            foreach(var obj in e.OldItems) {
                if(obj is INotifyPropertyChanged inpc) inpc.PropertyChanged -= OnChildPropertyChanged;
                if(obj is IFileEntryViewModel vm) vm.CheckStateChanged -= OnChildCheckStateChanged;
            }
        }
        if(e.NewItems is not null) {
            foreach(var obj in e.NewItems) {
                if(obj is INotifyPropertyChanged inpc) inpc.PropertyChanged += OnChildPropertyChanged;
                if(obj is IFileEntryViewModel vm) vm.CheckStateChanged += OnChildCheckStateChanged;
            }
        }
        // 追加/削除/Reset などで ChangeType が変わり得る
        RaisePropertyChanged( nameof( ChangeType ) );
        // チェック状態の整合も再計算
        RecomputeCheckStateFromChildren();
    }

    private void OnChildPropertyChanged( object? sender, PropertyChangedEventArgs e ) {
        // 子の ChangeType が変わったら自分の ChangeType も再評価を通知
        if(e.PropertyName == nameof( ChangeType )) RaisePropertyChanged( nameof( ChangeType ) );
    }

    /// <summary>子のチェック状態が変わった時に自分の状態を再計算（イベントベースでバブルアップ）</summary>
    private void OnChildCheckStateChanged( object? sender, bool? e ) {
        if(_suppressCheckPropagation) return;
        RecomputeCheckStateFromChildren();
    }

    #endregion
}