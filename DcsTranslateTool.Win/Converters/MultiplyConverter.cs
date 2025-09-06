using System.Globalization;
using System.Windows.Data;

namespace DcsTranslateTool.Win.Converters;

/// <summary>
/// 値に倍率を掛け算するコンバーターである
/// </summary>
public class MultiplyConverter : IValueConverter {
    /// <summary>
    /// 倍率を指定して値を変換する
    /// </summary>
    /// <param name="value">基になる値</param>
    /// <param name="targetType">変換後の型</param>
    /// <param name="parameter">倍率を示す文字列</param>
    /// <param name="culture">カルチャ</param>
    /// <returns>計算結果</returns>
    public object Convert( object value, Type targetType, object parameter, CultureInfo culture ) {
        if(value is double baseValue && parameter is string paramStr && double.TryParse( paramStr, out var scale )) {
            return baseValue * scale;
        }

        return 0d;
    }

    /// <summary>
    /// 変換結果を元の値に戻す
    /// </summary>
    /// <param name="value">変換後の値</param>
    /// <param name="targetType">変換前の型</param>
    /// <param name="parameter">倍率を示す文字列</param>
    /// <param name="culture">カルチャ</param>
    /// <returns>計算結果</returns>
    public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture ) {
        if(
            value is double baseValue &&
            parameter is string paramStr &&
            double.TryParse( paramStr, out var scale )
            && scale != 0
        ) {
            return baseValue / scale;
        }

        return 0d;
    }
}