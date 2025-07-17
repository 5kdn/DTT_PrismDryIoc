namespace DcsTranslateTool.Win.ViewModels;

public class CreatePullRequestDialogViewModel : BindableBase, IDialogAware {
    private IEnumerable<string>? _files;
    private string _category;
    private string _subcategory;
    private List<ModityType> _modifyTypes = new();
    private string _prComment = "[概要]\n簡潔に変更内容を記載してください。\n\n[変更内容]\n- mizファイル単位で箇条書きで記載してください\n- 機体やキャンペーン全体に関連する場合、機体やキャンペーンごとの記載でも大丈夫です\n\n[備考]\n- 気になる点があれば箇条書きで記載してください";

    private DelegateCommand _createPullRequestCommand;

    public DelegateCommand CreatePullRequestCommand
        => _createPullRequestCommand ??= new DelegateCommand( OnCreatePullRequest );

    public string Title => "PR作成ダイアログ";
    public CreatePullRequestDialogViewModel() { }

    /// <summary>
    /// ローカルのフォルダツリー
    /// </summary>
    public IEnumerable<string>? Files {
        get => _files;
        set => SetProperty( ref _files, value );
    }

    public string Category {
        get => _category;
        set => SetProperty( ref _category, value );
    }

    public string Subcategory {
        get => _subcategory;
        set => SetProperty( ref _subcategory, value );
    }

    /// <summary>
    /// PRのタイトル
    /// </summary>
    public string PRTitle {
        get {
            string modifies = string.Join("/", _modifyTypes.Select( x => x.ToString() ));
            return $"{Category}/{Subcategory}の{modifies}";
        }
    }

    public string PRComment {
        get => _prComment;
        set => SetProperty( ref _prComment, value );
    }

    public DialogCloseListener RequestClose { get; }

    public bool CanCloseDialog() => true;
    public void OnDialogClosed() { }
    public void OnDialogOpened( IDialogParameters parameters ) {
        _files = parameters.GetValue<IEnumerable<string>>( "files" );
    }

    private void OnCreatePullRequest() {
        try {
            // TODO: PR作成処理をここに実装
            RequestClose.Invoke( new DialogResult( ButtonResult.OK ) );
        }
        catch(Exception ex) {
            // TODO: ユーザーにエラー通知
            RequestClose.Invoke( new DialogResult( ButtonResult.Abort ) );
        }
    }
}
public enum ModityType {
    ファイルの追加,
    ファイルの削除,
    バグ修正,
    誤字の修正,
    その他の修正
}