namespace DcsTranslateTool.Win.Helpers;

/// <summary>
/// <see cref="IRegionNavigationService"/> に関する拡張メソッド
/// </summary>
public static class IRegionNavigationServiceExtensions {
    /// <summary>
    /// 現在の URI と異なるときのみ遷移可能か判定する
    /// </summary>
    /// <param name="navigationService">対象のサービス</param>
    /// <param name="target">遷移先の URI</param>
    /// <returns>遷移可能なら <see langword="true"/></returns>
    public static bool CanNavigate( this IRegionNavigationService navigationService, string target ) {
        if(string.IsNullOrEmpty( target )) return false;

        if(navigationService.Journal.CurrentEntry == null) return true;

        return target != navigationService.Journal.CurrentEntry.Uri.ToString();
    }
}