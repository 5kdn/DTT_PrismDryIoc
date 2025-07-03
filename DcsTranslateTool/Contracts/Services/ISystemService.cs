namespace DcsTranslateTool.Contracts.Services;

/// <summary>
/// システム操作を提供するサービスの契約
/// </summary>
public interface ISystemService
{
    /// <summary>
    /// 既定のブラウザーで指定 URL を開く
    /// </summary>
    /// <param name="url">URL</param>
    void OpenInWebBrowser( string url );
}
