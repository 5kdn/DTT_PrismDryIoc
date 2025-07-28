using DcsTranslateTool.Win.Enums;

namespace DcsTranslateTool.Win.Extensions;

/// <summary>
/// <see cref="RootTabType"/> の拡張メソッドを提供する
/// </summary>
public static class RootTabTypeExtensions {
    /// <summary>
    /// タブの表示名を取得する
    /// </summary>
    /// <param name="tabType">対象の <see cref="RootTabType"/></param>
    /// <returns>タブの表示名</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// 未対応の <see cref="RootTabType"/> が指定された場合にスローされる
    /// </exception>
    public static string GetTabTitle( this RootTabType tabType ) => tabType switch
    {
        RootTabType.Aircraft => "Aircraft",
        RootTabType.DlcCampaigns => "DLC Campaigns",
        _ => throw new ArgumentOutOfRangeException(
            nameof( tabType ),
            tabType,
            $"Unexpected RootTabType value: {tabType}" )
    };

    /// <summary>
    /// タブに対応するリポジトリのディレクトリルートを取得する
    /// </summary>
    /// <param name="tabType">対象の <see cref="RootTabType"/></param>
    /// <returns>ディレクトリルートのパス</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// 未対応の <see cref="RootTabType"/> が指定された場合にスローされる
    /// </exception>
    public static string[] GetRepoDirRoot( this RootTabType tabType ) => tabType switch
    {
        RootTabType.Aircraft => ["DCSWorld", "Mods", "aircraft"],
        RootTabType.DlcCampaigns => ["DCSWorld", "Mods", "campaigns"],
        _ => throw new ArgumentOutOfRangeException(
            nameof( tabType ),
            tabType,
            $"Unexpected RootTabType value: {tabType}" )
    };
}
