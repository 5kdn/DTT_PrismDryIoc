namespace DcsTranslateTool.Win.Models;
public class PullRequestDialogAgreementCheckItem( string message ) : BindableBase {
    private bool _isAgreed = false;

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
    public string Message { get; } = message;
}