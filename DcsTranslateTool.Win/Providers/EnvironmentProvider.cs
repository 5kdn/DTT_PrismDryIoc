using DcsTranslateTool.Win.Contracts.Providers;

namespace DcsTranslateTool.Win.Providers;

/// <summary>
/// 環境変数を扱うプロバイダ
/// </summary>
public class EnvironmentProvider : IEnvironmentProvider {
    /// <inheritdoc/>
    public string GetEnvironmentVariable( string variable )
        => Environment.GetEnvironmentVariable( variable ) ?? string.Empty;

    /// <inheritdoc/>
    public string GetUserProfilePath()
        => GetEnvironmentVariable( "UserProfile" );
}
