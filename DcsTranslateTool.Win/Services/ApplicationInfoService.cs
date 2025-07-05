using System.Diagnostics;
using System.Reflection;

using DcsTranslateTool.Win.Contracts.Services;

namespace DcsTranslateTool.Win.Services;

/// <summary>
/// アプリケーション情報を提供するサービス
/// </summary>
public class ApplicationInfoService : IApplicationInfoService {
    /// <summary>
    /// 新しいインスタンスを生成する
    /// </summary>
    public ApplicationInfoService() { }

    /// <inheritdoc/>
    public Version GetVersion() {
        // Set the app version in DcsTranslateTool.Win > Properties > Package > PackageVersion
        string assemblyLocation = Assembly.GetExecutingAssembly().Location;
        var version = FileVersionInfo.GetVersionInfo(assemblyLocation).FileVersion;
        return new Version( version! );
    }
}
