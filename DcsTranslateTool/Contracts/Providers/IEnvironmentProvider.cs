namespace DcsTranslateTool.Contracts.Providers;

/// <summary>
/// 環境変数を取得するプロバイダ
/// </summary>
public interface IEnvironmentProvider
{
    /// <summary>
    /// 環境変数を取得する
    /// </summary>
    /// <param name="variable">環境変数名</param>
    /// <returns>取得した値。存在しない場合は null</returns>
    string? GetEnvironmentVariable( string variable );

    /// <summary>
    /// %UserProfile% の値を取得する
    /// </summary>
    /// <returns>ユーザープロファイルのパス</returns>
    string GetUserProfile();
}
