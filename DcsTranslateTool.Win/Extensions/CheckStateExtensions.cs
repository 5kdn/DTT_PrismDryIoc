using DcsTranslateTool.Win.Enums;

namespace DcsTranslateTool.Win.Extensions;

/// <summary>
/// <see cref="CheckState"/> に関連する拡張メソッドを提供する
/// </summary>
public static class CheckStateExtensions {
    /// <summary>
    /// チェック状態が選択されているように見えるかどうかを判定する
    /// <para>
    /// <see cref="CheckState.Checked"/> および <see cref="CheckState.Indeterminate"/> の場合は <c>true</c> を返し、
    /// <see cref="CheckState.Unchecked"/> の場合は <c>false</c> を返す
    /// </para>
    /// </summary>
    /// <param name="state">判定対象の <see cref="CheckState"/></param>
    /// <returns>選択されているように見える状態であれば <c>true</c>、そうでなければ <c>false</c></returns>
    /// <exception cref="ArgumentOutOfRangeException">想定外の <see cref="CheckState"/> 値が指定された場合にスローされる</exception>
    public static bool IsSelectedLike( this CheckState state ) => state switch
    {
        CheckState.Unchecked => false,
        CheckState.Checked => true,
        CheckState.Indeterminate => true,
        _ => throw new ArgumentOutOfRangeException(
            nameof( state ),
            state,
            $"Unexpected CheckState value: {state}" )
    };
}
