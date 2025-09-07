using DcsTranslateTool.Win.Models;

namespace DcsTranslateTool.Win.ViewModels;

public class PullRequestChangeKindViewModel( PullRequestChangeKind kind ) : BindableBase {
    private bool _isChecked;

    public PullRequestChangeKind Kind { get; } = kind;

    public string DisplayName => Kind.ToString();

    public bool IsChecked {
        get => _isChecked;
        set => SetProperty( ref _isChecked, value );
    }
}