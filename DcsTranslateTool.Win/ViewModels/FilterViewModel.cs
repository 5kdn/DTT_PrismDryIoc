using DcsTranslateTool.Win.Enums;

namespace DcsTranslateTool.Win.ViewModels;

/// <summary>
/// ファイルの変更種別によるフィルタ状態を保持する ViewModel である。
/// </summary>
public class FilterViewModel() : BindableBase {
    #region Fields

    private bool _all = true;
    private bool _unchanged = true;
    private bool _deleted = true;
    private bool _added = true;
    private bool _modified = true;

    #endregion

    #region Properties

    /// <summary>
    /// すべての項目を対象とするかどうかを取得または設定するプロパティである。
    /// </summary>
    public bool All {
        get => _all;
        set {
            if(!SetProperty( ref _all, value )) return;
            if(value) {
                Unchanged = Deleted = Added = Modified = true;
            }
            else {
                FiltersChanged?.Invoke( this, EventArgs.Empty );
            }
        }
    }

    /// <summary>
    /// 変更なしの項目を含めるかどうかを取得または設定するプロパティである。
    /// </summary>
    public bool Unchanged {
        get => _unchanged;
        set {
            if(!SetProperty( ref _unchanged, value )) return;
            UpdateAll();
        }
    }

    /// <summary>
    /// 削除された項目を含めるかどうかを取得または設定するプロパティである。
    /// </summary>
    public bool Deleted {
        get => _deleted;
        set {
            if(!SetProperty( ref _deleted, value )) return;
            UpdateAll();
        }
    }

    /// <summary>
    /// 新規項目を含めるかどうかを取得または設定するプロパティである。
    /// </summary>
    public bool Added {
        get => _added;
        set {
            if(!SetProperty( ref _added, value )) return;
            UpdateAll();
        }
    }

    /// <summary>
    /// 変更された項目を含めるかどうかを取得または設定するプロパティである。
    /// </summary>
    public bool Modified {
        get => _modified;
        set {
            if(!SetProperty( ref _modified, value )) return;
            UpdateAll();
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// 有効なフィルタを列挙するメソッドである。
    /// </summary>
    /// <returns>選択されている<see cref="FileChangeType"/>の列挙。</returns>
    public IEnumerable<FileChangeType> GetActiveTypes() {
        if(Unchanged) yield return FileChangeType.Unchanged;
        if(Deleted) yield return FileChangeType.Deleted;
        if(Added) yield return FileChangeType.Added;
        if(Modified) yield return FileChangeType.Modified;
    }

    /// <summary>
    /// All の状態を更新するメソッドである。
    /// </summary>
    private void UpdateAll() {
        bool allChecked = Unchanged && Deleted && Added && Modified;
        if(_all != allChecked) {
            _all = allChecked;
            RaisePropertyChanged( nameof( All ) );
        }
        FiltersChanged?.Invoke( this, EventArgs.Empty );
    }

    #endregion

    #region Events

    /// <summary>
    /// フィルタ状態が変更されたときに通知するイベントである。
    /// </summary>
    public event EventHandler? FiltersChanged;

    #endregion
}