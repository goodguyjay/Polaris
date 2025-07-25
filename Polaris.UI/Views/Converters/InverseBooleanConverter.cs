using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Polaris.UI.Views.Converters;

public sealed class InverseBooleanConverter : IValueConverter
{
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    ) => value is bool b ? !b : value;

    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    ) => value is bool b ? !b : value;
}
