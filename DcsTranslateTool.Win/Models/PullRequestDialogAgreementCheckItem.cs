namespace DcsTranslateTool.Win.Models;
public class PullRequestDialogAgreementCheckItem : BindableBase {
    private bool _isAgreed;
    private readonly string _message;

    /// <summary>
    /// ユーザーが同意したか
    /// </summary>
    public bool IsAgreed {
        get => _isAgreed;
        set => SetProperty( ref _isAgreed, value );
    }

    /// <summary>
    /// ユーザーに同意を求めるメッセージ
    /// </summary>
    public string Message { get => _message; }

    public PullRequestDialogAgreementCheckItem( string message ) {
        _isAgreed = false;
        _message = message;
    }
}
