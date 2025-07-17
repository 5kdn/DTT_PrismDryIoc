using System;
using System.Globalization;
using System.Windows.Data;

namespace DcsTranslateTool.Win.Converters;

/// <summary>
/// ボタンサイズからアイコンサイズを計算するコンバーターである
/// </summary>
public class IconSizeConverter : IMultiValueConverter {
    /// <summary>
    /// ボタンの幅と高さからアイコンの大きさを計算する
    /// </summary>
    /// <param name="values">幅と高さ</param>
    /// <param name="targetType">変換後の型</param>
    /// <param name="parameter">倍率を示す文字列</param>
    /// <param name="culture">カルチャ</param>
    /// <returns>計算結果</returns>
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
        if (values.Length == 2 && values[0] is double width && values[1] is double height) {
            var min = Math.Min(width, height);
            var scale = parameter is string paramStr && double.TryParse(paramStr, out var s) ? s : 1.0;
            return min * scale;
        }

        return 0d;
    }

    /// <summary>
    /// 変換結果から元の値を計算する
    /// </summary>
    /// <param name="value">変換後の値</param>
    /// <param name="targetTypes">変換前の型配列</param>
    /// <param name="parameter">倍率</param>
    /// <param name="culture">カルチャ</param>
    /// <returns>計算結果</returns>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}
