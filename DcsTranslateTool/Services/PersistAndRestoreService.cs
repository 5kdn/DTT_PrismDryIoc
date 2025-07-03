using System.Collections;
using System.IO;

using DcsTranslateTool.Contracts.Services;
using DcsTranslateTool.Core.Contracts.Services;
using DcsTranslateTool.Models;

namespace DcsTranslateTool.Services;

/// <summary>
/// アプリケーション設定の保存と復元を行うサービス
/// </summary>
public class PersistAndRestoreService : IPersistAndRestoreService
{
    private readonly IFileService _fileService;
    private readonly AppConfig _appConfig;
    private readonly string _localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

    /// <summary>
    /// 新しいインスタンスを生成する
    /// </summary>
    /// <param name="fileService">ファイルサービス</param>
    /// <param name="appConfig">アプリ設定</param>
    public PersistAndRestoreService( IFileService fileService, AppConfig appConfig )
    {
        _fileService = fileService;
        _appConfig = appConfig;
    }

    /// <inheritdoc/>
    public void PersistData()
    {
        if(App.Current.Properties != null)
        {
            var folderPath = Path.Combine(_localAppData, _appConfig.ConfigurationsFolder);
            var fileName = _appConfig.AppPropertiesFileName;
            _fileService.Save( folderPath, fileName, App.Current.Properties );
        }
    }

    /// <inheritdoc/>
    public void RestoreData()
    {
        var folderPath = Path.Combine(_localAppData, _appConfig.ConfigurationsFolder);
        var fileName = _appConfig.AppPropertiesFileName;
        var properties = _fileService.Read<IDictionary>(folderPath, fileName);
        if(properties != null)
        {
            foreach(DictionaryEntry property in properties)
            {
                App.Current.Properties.Add( property.Key, property.Value );
            }
        }
    }
}
