using DcsTranslateTool.Win.Contracts.Services;
using DcsTranslateTool.Win.ViewModels;
using DcsTranslateTool.Win.Views;
using DcsTranslateTool.Share.Contracts.Services;

namespace DcsTranslateTool.Win.Services;

/// <summary>
/// アップロードダイアログを表示するサービスである。
/// </summary>
public class UploadDialogService : IUploadDialogService {
    private readonly IRepositoryService _repositoryService;
    private readonly IAppSettingsService _appSettingsService;

    /// <summary>
    /// <see cref="UploadDialogService"/> の新しいインスタンスを生成する。
    /// </summary>
    public UploadDialogService( IRepositoryService repositoryService, IAppSettingsService appSettingsService ) {
        _repositoryService = repositoryService;
        _appSettingsService = appSettingsService;
    }

    /// <inheritdoc />
    public void ShowDialog( IEnumerable<string> paths ) {
        var vm = new UploadDialogViewModel( _repositoryService, _appSettingsService, paths );
        var dialog = new UploadDialog { DataContext = vm };
        dialog.ShowDialog();
    }
}
