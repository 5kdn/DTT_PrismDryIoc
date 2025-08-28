using System.Globalization;
using System.Windows;
using System.Windows.Data;

using DcsTranslateTool.Win.Enums;

namespace DcsTranslateTool.Win.Converters;

/// <summary>
/// <see cref="CheckState"/> と <see cref="bool?"/> を相互変換するコンバーターである
/// </summary>
public class CheckStateToNullableBoolConverter : IValueConverter {
    /// <inheritdoc />
    public object? Convert( object? value, Type targetType, object? parameter, CultureInfo culture ) {
        if(value is CheckState state) {
            return state switch
            {
                CheckState.Unchecked => false,
                CheckState.Checked => true,
                CheckState.Indeterminate => null,
                _ => DependencyProperty.UnsetValue
            };
        }
        return DependencyProperty.UnsetValue;
    }

    /// <inheritdoc />
    public object? ConvertBack( object? value, Type targetType, object? parameter, CultureInfo culture ) {
        return value switch
        {
            bool b when b => CheckState.Checked,
            bool b when !b => CheckState.Unchecked,
            null => CheckState.Indeterminate,
            _ => DependencyProperty.UnsetValue
        };
    }
}