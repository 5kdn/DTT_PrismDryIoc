using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Threading.Tasks;

using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Core.Models;
using FluentResults;
using DcsTranslateTool.Win.Enums;
using DcsTranslateTool.Win.Models;

namespace DcsTranslateTool.Win.ViewModels;

public class CreatePullRequestDialogViewModel : BindableBase, IDialogAware {
    #region Fields

    public static string Title => "PR作成ダイアログ";
    private IEnumerable<FileEntry>? _files;
    private string _prComment = "[概要]\n簡潔に変更内容を記載してください。\n\n[変更内容]\n- mizファイル単位で箇条書きで記載してください\n- 機体やキャンペーン全体に関連する場合、機体やキャンペーンごとの記載でも大丈夫です\n\n[備考]\n- 気になる点があれば箇条書きで記載してください";

    private RootTabType _category;

    private readonly IRepositoryService repositoryService;

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

    /// <summary>
    /// 選択されたタブのカテゴリを取得する
    /// </summary>
    public RootTabType Category => _category;

    /// <summary>
    /// 選択ファイルの共通ディレクトリ名を取得する
    /// </summary>
    public string Subcategory {
        get {
            if(Files?.Any() != true) return string.Empty;

            var directories = Files
                .Select(f => Path.GetDirectoryName(f.AbsolutePath) ?? string.Empty)
                .Select(p => p.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, System.StringSplitOptions.RemoveEmptyEntries))
                .ToList();

            if(directories.Count == 0) return string.Empty;

            int minLength = directories.Min(arr => arr.Length);
            int prefixLength = 0;
            for(int i = 0; i < minLength; i++) {
                var segment = directories[0][i];
                if(!directories.All( a => string.Equals( a[i], segment, System.StringComparison.OrdinalIgnoreCase ) )) break;
                prefixLength = i + 1;
            }

            return prefixLength == 0 ? string.Empty : directories[0][prefixLength - 1];
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
    /// <param name="repositoryService">リポジトリサービス</param>
    public CreatePullRequestDialogViewModel( IRepositoryService repositoryService ) {
        this.repositoryService = repositoryService;
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

    /// <summary>
    /// ダイアログが開いたときに呼び出される
    /// </summary>
    /// <param name="parameters">ダイアログ引数</param>
    public void OnDialogOpened( IDialogParameters parameters ) {
        _files = parameters.GetValue<IEnumerable<FileEntry>>( "files" );
        _category = parameters.GetValue<RootTabType>( "Category" );
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

    /// <summary>
    /// RepositoryServiceを利用してプルリクエストを作成する
    /// </summary>
    /// <param name="branchName">作成するブランチ名</param>
    /// <returns>処理結果</returns>
    private async Task<Result> CreatePullRequestInternalAsync( string branchName ) {
        try {
            return await repositoryService.CreatePullRequestAsync( branchName, PRTitle, PRComment );
        }
        catch( Exception ex ) {
            return Result.Fail( ex.Message );
        }
    }

    private async void OnCreatePullRequest() {
        string newBranchName = CreateBranchName();

        // For Debugging purposes
        var msg = string.Join("\n", PullRequestChangeKinds.Where( x => x.IsChecked ).Select( x => x.DisplayName ));
        MessageBox.Show( $"{newBranchName}\n{msg}" );

        var result = await CreatePullRequestInternalAsync( newBranchName );
        if(result.IsSuccess) {
            RequestClose.Invoke( new DialogResult( ButtonResult.OK ) );
        }
        else {
            RequestClose.Invoke( new DialogResult( ButtonResult.Abort ) );
        }
    }
    #endregion
}
