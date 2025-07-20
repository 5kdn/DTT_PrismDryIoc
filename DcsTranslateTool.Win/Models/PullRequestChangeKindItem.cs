namespace DcsTranslateTool.Win.Models;
public class PullRequestChangeKindItem : BindableBase {
    public PullRequestChangeKind Kind { get; }
    public string DisplayName => Kind.ToString();

    private bool _isChecked;
    public bool IsChecked {
        get => _isChecked;
        set => SetProperty( ref _isChecked, value );
    }

    public PullRequestChangeKindItem( PullRequestChangeKind kind ) => Kind = kind;
}