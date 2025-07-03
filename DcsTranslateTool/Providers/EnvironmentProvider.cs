using System;

using DcsTranslateTool.Contracts.Providers;

namespace DcsTranslateTool.Providers;

/// <summary>
/// 環境変数を取得するサービス
/// </summary>
public class EnvironmentProvider : IEnvironmentProvider
{
    /// <inheritdoc />
    public string? GetEnvironmentVariable( string variable )
        => Environment.GetEnvironmentVariable( variable );

    /// <inheritdoc />
    public string GetUserProfile()
        => Environment.GetFolderPath( Environment.SpecialFolder.UserProfile );
}
