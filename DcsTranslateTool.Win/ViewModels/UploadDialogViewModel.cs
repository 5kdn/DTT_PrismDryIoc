using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;

using DcsTranslateTool.Share.Contracts.Services;
using DcsTranslateTool.Share.Models;
using DcsTranslateTool.Win.Contracts.Services;

namespace DcsTranslateTool.Win.ViewModels;

/// <summary>
/// UploadDialog の ViewModel である。
/// </summary>
public class UploadDialogViewModel : BindableBase {
    private readonly IRepositoryService _repositoryService;
    private readonly IAppSettingsService _appSettingsService;
    private DelegateCommand _commitCommand;

    /// <summary>
    /// ファイルパス一覧を取得する。
    /// </summary>
    public ObservableCollection<string> FilePaths { get; } = new();

    /// <summary>
    /// カテゴリ候補を取得する。
    /// </summary>
    public ObservableCollection<string> CategoryOptions { get; } = new() { "aircraft", "campaign" };

    /// <summary>
    /// 選択中のカテゴリを取得または設定する。
    /// </summary>
    public string SelectedCategory { get; set; } = "aircraft";

    /// <summary>
    /// 名称候補を取得する。
    /// </summary>
    public ObservableCollection<string> NameOptions { get; } = new();

    /// <summary>
    /// 選択中の名称を取得または設定する。
    /// </summary>
    public string SelectedName { get; set; } = string.Empty;

    /// <summary>
    /// 変更内容の候補を取得する。
    /// </summary>
    public ObservableCollection<ChangeItem> ChangeItems { get; } = new() {
        new ChangeItem("ファイルの追加"),
        new ChangeItem("ファイルの削除"),
        new ChangeItem("バグ修正"),
        new ChangeItem("誤字の修正"),
        new ChangeItem("その他の修正")
    };

    /// <summary>
    /// コメントを取得または設定する。
    /// </summary>
    public string Comment { get; set; } = "[概要]\n簡潔に変更内容を記載してください。\n\n[変更内容]\n- mizファイル単位で箇条書きで記載してください\n- 機体やキャンペーン全体に関連する場合、機体やキャンペーンごとの記載でも大丈夫です\n\n[備考]\n- 気になる点があれば箇条書きで記載してください";

    /// <summary>
    /// 個人情報確認チェックの状態を取得または設定する。
    /// </summary>
    public bool ConfirmPersonal { get; set; }

    /// <summary>
    /// 翻訳済み確認チェックの状態を取得または設定する。
    /// </summary>
    public bool ConfirmTranslate { get; set; }

    /// <summary>
    /// コミットコマンドを取得する。
    /// </summary>
    public DelegateCommand CommitCommand => _commitCommand ??= new DelegateCommand( OnCommit, () => CanCommit ).ObservesProperty(() => ConfirmPersonal).ObservesProperty(() => ConfirmTranslate);

    /// <summary>
    /// コマンド実行可否を取得する。
    /// </summary>
    public bool CanCommit => ConfirmPersonal && ConfirmTranslate;

    /// <summary>
    /// <see cref="UploadDialogViewModel"/> の新しいインスタンスを生成する。
    /// </summary>
    /// <param name="repositoryService">リポジトリサービス</param>
    /// <param name="appSettingsService">アプリ設定サービス</param>
    /// <param name="paths">アップロードするファイルパス</param>
    public UploadDialogViewModel( IRepositoryService repositoryService, IAppSettingsService appSettingsService, IEnumerable<string> paths ) {
        _repositoryService = repositoryService;
        _appSettingsService = appSettingsService;
        foreach(var p in paths) FilePaths.Add( p );
        LoadNameOptions();
    }

    private void LoadNameOptions() {
        var aircraftPath = Path.Combine( _appSettingsService.TranslateFileDir, "aircraft" );
        if(Directory.Exists( aircraftPath )) {
            foreach(string dir in Directory.GetDirectories( aircraftPath )) {
                NameOptions.Add( Path.GetFileName( dir ) );
            }
        }
        var campaignPath = Path.Combine( _appSettingsService.TranslateFileDir, "campaigns" );
        if(Directory.Exists( campaignPath )) {
            foreach(string dir in Directory.GetDirectories( campaignPath )) {
                NameOptions.Add( Path.GetFileName( dir ) );
            }
        }
    }

    private async void OnCommit() {
        string branchName = $"upload-{DateTime.Now:yyyyMMddHHmmss}";
        await _repositoryService.CreateBranchAsync( branchName );
        List<CommitFile> files = FilePaths.Select( p => new CommitFile {
            Operation = CommitOperation.AddOrUpdate,
            LocalPath = p,
            RepoPath = Path.GetRelativePath( _appSettingsService.TranslateFileDir, p ).Replace( Path.DirectorySeparatorChar, '/' )
        } ).ToList();
        string title = $"{SelectedCategory}/{SelectedName}の{string.Join("・", ChangeItems.Where(i => i.IsChecked).Select(i => i.Text))}";
        await _repositoryService.CommitAsync( branchName, files, title );
        await _repositoryService.CreatePullRequestAsync( branchName, title, Comment );
        if(System.Windows.Application.Current.Windows.OfType<Window>().FirstOrDefault( w => w.DataContext == this ) is Window win) {
            win.DialogResult = true;
            win.Close();
        }
    }

    /// <summary>
    /// 変更項目を表す内部クラスである。
    /// </summary>
    public class ChangeItem : BindableBase {
        private bool _isChecked;
        /// <summary>
        /// 表示テキストを取得する。
        /// </summary>
        public string Text { get; }
        /// <summary>
        /// チェック状態を取得または設定する。
        /// </summary>
        public bool IsChecked { get => _isChecked; set => SetProperty( ref _isChecked, value ); }
        /// <summary>
        /// <see cref="ChangeItem"/> の新しいインスタンスを生成する。
        /// </summary>
        /// <param name="text">表示テキスト</param>
        public ChangeItem( string text ) => Text = text;
    }
}
