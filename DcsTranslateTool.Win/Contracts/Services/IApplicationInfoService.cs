namespace DcsTranslateTool.Win.Contracts.Services;

/// <summary>
/// アプリケーション情報を取得するサービスの契約
/// </summary>
public interface IApplicationInfoService {
    /// <summary>
    /// アプリケーションのバージョンを取得する
    /// </summary>
    /// <returns>バージョン番号</returns>
    Version GetVersion();
}
