using System.Collections;
using System.IO;

using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Win.Contracts.Services;
using DcsTranslateTool.Win.Models;

namespace DcsTranslateTool.Win.Services;

/// <summary>
/// アプリケーション設定の保存と復元を行うサービス
/// </summary>
/// <remarks>
/// 新しいインスタンスを生成する
/// </remarks>
/// <param name="fileService">ファイルサービス</param>
/// <param name="appConfig">アプリ設定</param>
public class PersistAndRestoreService( IFileService fileService, AppConfig appConfig ) : IPersistAndRestoreService {
    private readonly string _localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

    /// <inheritdoc/>
    public void PersistData() {
        if(App.Current.Properties != null) {
            var folderPath = Path.Combine(_localAppData, appConfig.ConfigurationsFolder);
            var fileName = appConfig.AppPropertiesFileName;
            fileService.SaveToJson( folderPath, fileName, App.Current.Properties );
        }
    }

    /// <inheritdoc/>
    public void RestoreData() {
        var folderPath = Path.Combine(_localAppData, appConfig.ConfigurationsFolder);
        var fileName = appConfig.AppPropertiesFileName;
        var properties = fileService.ReadFromJson<IDictionary>(folderPath, fileName);
        if(properties != null) {
            foreach(DictionaryEntry property in properties) {
                App.Current.Properties.Add( property.Key, property.Value );
            }
        }
    }
}