using System.Diagnostics;
using System.IO;

using DcsTranslateTool.Win.Contracts.Services;

namespace DcsTranslateTool.Win.Services;

/// <summary>
/// システム関連の処理を提供するサービス
/// </summary>
public class SystemService : ISystemService {
    /// <summary>
    /// 新しいインスタンスを生成する
    /// </summary>
    public SystemService() { }

    /// <inheritdoc/>
    public void OpenInWebBrowser( string url ) {
        // For more info see https://github.com/dotnet/corefx/issues/10361
        var psi = new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        };
        Process.Start( psi );
    }

    /// <inheritdoc/>
    public void OpenDirectory( string path ) {
        if(!Directory.Exists( path )) {
            if(File.Exists( path ) && Path.GetDirectoryName( path ) is string p) {
                path = p;
            }
            else {
                throw new DirectoryNotFoundException( path );
            }
        }
        var psi = new ProcessStartInfo
        {
            FileName = "explorer.exe",
            Arguments = path,
            UseShellExecute = true
        };
        Process.Start( psi );
    }
}