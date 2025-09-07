using DcsTranslateTool.Win.Contracts.ViewModels;
using DcsTranslateTool.Win.Enums;

namespace DcsTranslateTool.Win.ViewModels;

/// <summary>
/// ファイルの変更種別によるフィルタ状態を保持する ViewModel である。
/// </summary>
public class FilterViewModel() : BindableBase, IFilterViewModel {
    #region Fields

    private bool _all = true;
    private bool _unchanged = true;
    private bool _repoOnly = true;
    private bool _localOnly = true;
    private bool _modified = true;

    #endregion

    #region Properties

    /// <inheritdoc/>
    public bool All {
        get => _all;
        set {
            if(!SetProperty( ref _all, value )) return;
            if(value) {
                Unchanged = RepoOnly = LocalOnly = Modified = true;
            }
            else {
                Unchanged = RepoOnly = LocalOnly = Modified = false;
                FiltersChanged?.Invoke( this, EventArgs.Empty );
            }
        }
    }

    /// <inheritdoc/>
    public bool Unchanged {
        get => _unchanged;
        set {
            if(!SetProperty( ref _unchanged, value )) return;
            UpdateAll();
        }
    }

    /// <inheritdoc/>
    public bool RepoOnly {
        get => _repoOnly;
        set {
            if(!SetProperty( ref _repoOnly, value )) return;
            UpdateAll();
        }
    }

    /// <inheritdoc/>
    public bool LocalOnly {
        get => _localOnly;
        set {
            if(!SetProperty( ref _localOnly, value )) return;
            UpdateAll();
        }
    }

    /// <inheritdoc/>
    public bool Modified {
        get => _modified;
        set {
            if(!SetProperty( ref _modified, value )) return;
            UpdateAll();
        }
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public IEnumerable<FileChangeType?> GetActiveTypes() {
        if(Unchanged) yield return FileChangeType.Unchanged;
        if(RepoOnly) yield return FileChangeType.RepoOnly;
        if(LocalOnly) yield return FileChangeType.LocalOnly;
        if(Modified) yield return FileChangeType.Modified;
    }

    /// <summary>
    /// All の状態を更新するメソッドである。
    /// </summary>
    private void UpdateAll() {
        bool allChecked = Unchanged && RepoOnly && LocalOnly && Modified;
        if(_all != allChecked) {
            _all = allChecked;
            RaisePropertyChanged( nameof( All ) );
        }
        FiltersChanged?.Invoke( this, EventArgs.Empty );
    }

    #endregion

    #region Events

    /// <inheritdoc/>
    public event EventHandler? FiltersChanged;

    #endregion
}