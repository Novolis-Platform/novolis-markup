using System.Globalization;

namespace Novolis.Markup.Svg;

internal static class SvgNumber
{
    public static string Format(double value)
    {
        EnsureFinite(value, nameof(value));
        if (value == 0d)
        {
            value = 0d;
        }

        return value.ToString("0.#########", CultureInfo.InvariantCulture);
    }

    public static void EnsureFinite(double value, string parameterName)
    {
        if (!double.IsFinite(value))
        {
            throw new ArgumentOutOfRangeException(parameterName, value, "SVG numeric values must be finite.");
        }
    }

    public static void EnsureNonNegative(double value, string parameterName)
    {
        EnsureFinite(value, parameterName);
        if (value < 0d)
        {
            throw new ArgumentOutOfRangeException(parameterName, value, "SVG numeric values must not be negative.");
        }
    }
}
