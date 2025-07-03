using DcsTranslateTool.Properties;

namespace DcsTranslateTool.Contracts.Services;

/// <summary>
/// アプリ設定を操作するサービスの契約
/// </summary>
public interface IAppSettingsService
{
    /// <summary>
    /// 機体フォルダのパスを取得または設定する
    /// </summary>
    string SourceAircraftDir { get; set; }

    /// <summary>
    /// DLCキャンペーンフォルダのパスを取得または設定する
    /// </summary>
    string SourceDlcCampaignDir { get; set; }

    /// <summary>
    /// ユーザーフォルダのパスを取得または設定する
    /// </summary>
    string SourceUserDir { get; set; }

    /// <summary>
    /// 翻訳ファイルフォルダのパスを取得または設定する
    /// </summary>
    string TranslateFileDir { get; set; }

    /// <summary>
    /// 設定を既定値に戻す
    /// </summary>
    void Reset();
}
