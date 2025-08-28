using System.Globalization;
using System.Windows.Data;

namespace DcsTranslateTool.Win.Converters;

/// <summary>
/// 列挙体と bool の変換を行うコンバーター
/// </summary>
public class EnumToBooleanConverter : IValueConverter {
    /// <summary>
    /// 対象となる列挙体の型
    /// </summary>
    public required Type EnumType { get; set; }

    /// <summary>
    /// 列挙体の値を bool へ変換する
    /// </summary>
    /// <inheritdoc/>
    public object Convert( object? value, Type? targetType, object? parameter, CultureInfo? culture ) {
        if(EnumType is null) return false;
        if(parameter is string enumString && value is not null) {
            if(Enum.IsDefined( EnumType, value )) {
                var enumValue = Enum.Parse(EnumType, enumString);

                return enumValue.Equals( value );
            }
        }

        return false;
    }

    /// <summary>
    /// bool から列挙体へ変換する
    /// </summary>
    /// <inheritdoc/>
    public object? ConvertBack( object? value, Type? targetType, object? parameter, CultureInfo culture ) {
        if(EnumType is null) return null;
        if(parameter is string enumString) return Enum.Parse( EnumType, enumString );
        return null;
    }
}