using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.Models;

namespace DcsTranslateTool.Win.ViewModels;

public class CreatePullRequestDialogViewModel : BindableBase, IDialogAware {
    #region Fields

    public static string Title => "PR作成ダイアログ";
    private IEnumerable<FileEntry>? _files;
    private string _prComment = "[概要]\n簡潔に変更内容を記載してください。\n\n[変更内容]\n- mizファイル単位で箇条書きで記載してください\n- 機体やキャンペーン全体に関連する場合、機体やキャンペーンごとの記載でも大丈夫です\n\n[備考]\n- 気になる点があれば箇条書きで記載してください";

    private DelegateCommand? _createPullRequestCommand;

    #endregion

    #region Properties

    /// <summary>
    /// ローカルのフォルダツリー
    /// </summary>
    public IEnumerable<FileEntry>? Files {
        get => _files;
        set => SetProperty( ref _files, value );
    }

    public string Category {
        get {
            // TODO: 取得ファイル(_files)からカテゴリを自動的に決定するロジックを実装する
            return "Cat";
        }
    }

    public string Subcategory {
        get {
            // TODO: 取得ファイル(_files)からカテゴリを自動的に決定するロジックを実装する
            return "Sub";
        }
    }

    public ObservableCollection<PullRequestChangeKindItem> PullRequestChangeKinds { get; }

    /// <summary>
    /// PRのタイトル
    /// </summary>
    public string PRTitle {
        get {
            var checkedKinds = PullRequestChangeKinds.Where( x => x.IsChecked ).Select( x => x.DisplayName );
            return $"[{Category}][{Subcategory}] {string.Join( "/", checkedKinds )}";
        }
    }

    /// <summary>
    /// PRコメント
    /// </summary>
    public string PRComment {
        get => _prComment;
        set => SetProperty( ref _prComment, value );
    }

    /// <summary>
    /// 同意を求める項目
    /// </summary>
    public ObservableCollection<PullRequestDialogAgreementCheckItem> AgreementItems { get; }

    #endregion

    /// <summary>
    /// クラスの新しいインスタンスを生成する
    /// </summary>
    public CreatePullRequestDialogViewModel() {
        PullRequestChangeKinds = new ObservableCollection<PullRequestChangeKindItem>(
            Enum.GetValues( typeof( PullRequestChangeKind ) )
                .Cast<PullRequestChangeKind>()
                .Select( kind => new PullRequestChangeKindItem( kind ) )
        );

        foreach(var item in PullRequestChangeKinds) {
            item.PropertyChanged += PullRequestChangeKindItem_PropertyChanged;
        }
        AgreementItems =
        [
            new PullRequestDialogAgreementCheckItem("アップロードするファイルに個人情報は含まれていません"),
        ];
        foreach(var item in AgreementItems) {
            item.PropertyChanged += AgreementItem_PropertyChanged;
        }
    }

    #region Commands
    public DelegateCommand CreatePullRequestCommand
    => _createPullRequestCommand ??= new DelegateCommand( OnCreatePullRequest );

    #endregion

    #region Methods

    /// <summary>
    /// PRを作成する上で十分な入力がされているか
    /// </summary>
    public bool CanCreatePR {
        get {
            // 最低でも1つの変更点にチェックを入れる必要がある
            if(!PullRequestChangeKinds.Any( x => x.IsChecked )) return false;
            // 全ての確認事項に同意する必要がある
            if(!AgreementItems.All( x => x.IsAgreed )) return false;
            return true;
        }
    }

    public DialogCloseListener RequestClose { get; }

    public bool CanCloseDialog() => true;
    public void OnDialogClosed() { }
    public void OnDialogOpened( IDialogParameters parameters ) {
        _files = parameters.GetValue<IEnumerable<FileEntry>>( "files" );
    }

    public IEnumerable<PullRequestChangeKind> SelectedChangeKinds =>
        PullRequestChangeKinds.Where( x => x.IsChecked ).Select( x => x.Kind );

    private void PullRequestChangeKindItem_PropertyChanged( object? sender, PropertyChangedEventArgs e ) {
        if(e.PropertyName == nameof( PullRequestChangeKindItem.IsChecked )) {
            RaisePropertyChanged( nameof( PRTitle ) );
            RaisePropertyChanged( nameof( CanCreatePR ) );
        }
    }

    private void AgreementItem_PropertyChanged( object? sender, PropertyChangedEventArgs e ) {
        if(e.PropertyName == nameof( PullRequestDialogAgreementCheckItem.IsAgreed )) {
            RaisePropertyChanged( nameof( CanCreatePR ) );
        }
    }

    private string CreateBranchName() {
        var jst = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time"));
        string dateStr = jst.ToString("yyyyMMdd-HHmmss") + "JST";

        var changes = string.Join("_", SelectedChangeKinds.Select( x => x.ToString() ));
        return $"feature/{Category}/{Subcategory}--{changes}--{dateStr}";
    }

    private void OnCreatePullRequest() {
        string newBranchName = CreateBranchName();

        // For Debugging purposes
        var msg = string.Join("\n",PullRequestChangeKinds.Where( x => x.IsChecked ).Select( x => x.DisplayName ));
        MessageBox.Show( $"{newBranchName}\n{msg}" );
        try {
            // TODO: PR作成処理をここに実装
            RequestClose.Invoke( new DialogResult( ButtonResult.OK ) );
        }
        catch {
            // TODO: ユーザーにエラー通知
            RequestClose.Invoke( new DialogResult( ButtonResult.Abort ) );
        }
    }
    #endregion
}
