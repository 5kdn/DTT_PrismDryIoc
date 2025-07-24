namespace DcsTranslateTool.Win.Models;
// TODO: VM化
public class PullRequestChangeKindItem( PullRequestChangeKind kind ) : BindableBase {
    public PullRequestChangeKind Kind { get; } = kind;
    public string DisplayName => Kind.ToString();

    private bool _isChecked;
    public bool IsChecked {
        get => _isChecked;
        set => SetProperty( ref _isChecked, value );
    }
}