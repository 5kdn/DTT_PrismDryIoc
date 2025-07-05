using System.Diagnostics;

using DcsTranslateTool.Win.Contracts.Services;

namespace DcsTranslateTool.Win.Services;

/// <summary>
/// システム関連の処理を提供するサービス
/// </summary>
public class SystemService : ISystemService
{
    /// <summary>
    /// 新しいインスタンスを生成する
    /// </summary>
    public SystemService() { }

    /// <inheritdoc/>
    public void OpenInWebBrowser( string url )
    {
        // For more info see https://github.com/dotnet/corefx/issues/10361
        var psi = new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        };
        Process.Start( psi );
    }
}
