using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

using DcsTranslateTool.Win.Enums;

namespace DcsTranslateTool.Win.Converters;

/// <summary>
/// <see cref="FileChangeType"/>から表示色を取得するコンバーターである。
/// </summary>
public class FileChangeTypeToBrushConverter : IValueConverter {
    /// <summary>
    /// <see cref="FileChangeType"/>に応じたブラシを返す。
    /// </summary>
    /// <param name="value">変換元の値</param>
    /// <param name="targetType">変換後の型</param>
    /// <param name="parameter">未使用</param>
    /// <param name="culture">カルチャ</param>
    /// <returns>変換結果</returns>
    public object? Convert( object? value, Type targetType, object? parameter, CultureInfo culture ) {
        if(value is FileChangeType changeType) {
            return changeType switch
            {
                FileChangeType.Unchanged => Brushes.Silver,
                FileChangeType.Deleted => Brushes.Black,
                FileChangeType.Added => Brushes.Tomato,
                FileChangeType.Modified => Brushes.LimeGreen,
                _ => Brushes.Black,
            };
        }
        return Brushes.Black;
    }

    /// <summary>
    /// 変換結果から元の値を取得する。
    /// </summary>
    /// <param name="value">変換後の値</param>
    /// <param name="targetType">変換前の型</param>
    /// <param name="parameter">未使用</param>
    /// <param name="culture">カルチャ</param>
    /// <returns>変換結果</returns>
    public object? ConvertBack( object? value, Type targetType, object? parameter, CultureInfo culture ) {
        throw new NotImplementedException();
    }
}