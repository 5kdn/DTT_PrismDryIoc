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

        var configFile = _appConfig.AppConfigFileName;
        if(!string.IsNullOrEmpty( configFile ))
        {
            var folderPath = Path.Combine(_localAppData, _appConfig.ConfigurationsFolder);
            _fileService.Save( folderPath, configFile, _appConfig );
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

        var configFile = _appConfig.AppConfigFileName;
        if(!string.IsNullOrEmpty( configFile ))
        {
            var config = _fileService.Read<AppConfig>(folderPath, configFile);
            if(config != null)
            {
                _appConfig.SourceAircraftDir = config.SourceAircraftDir;
                _appConfig.SourceDlcCampaignDir = config.SourceDlcCampaignDir;
                _appConfig.SourceUserDir = config.SourceUserDir;
                _appConfig.TranslateFileDir = config.TranslateFileDir;
            }
        }
    }
}
