namespace DcsTranslateTool.Win.Constants;
/// <summary>
/// GitHub App の固定設定値を保持するクラス。
/// <para>
/// このクラスはアプリケーション全体で共有される GitHub App の アプリケーション名 および AppId 、 InstallationIdを提供する。
/// </para>
/// </summary>
public static class GitHubAppSettings {
    private const string appName = "DcsTranslateTool";
    private const int appId = 123456;
    private const long installationId = 1234567890;

    /// <summary>
    /// アプリケーション名。
    /// </summary>
    /// <returns>
    /// アプリケーション名。
    /// </returns>
    public static string AppName { get => appName; }

    /// <summary>
    /// GitHub App の一意な識別子。
    /// GitHub にアプリケーションを登録した際に発行される。
    /// </summary>
    public static int AppId { get => appId; }

    /// <summary>
    /// GitHub App がインストールされているリポジトリまたは組織に紐づく識別子。
    /// GitHub のインストール情報から取得される。
    /// </summary>
    public static long InstallationId { get => installationId; }
}