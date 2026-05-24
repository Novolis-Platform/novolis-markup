using System.Globalization;

// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

/// <summary>Extension methods for building Markdown tables from objects.</summary>
public static class EnumerableExtensions
{
    /// <summary>Extracts table headers and rows from a sequence of items.</summary>
    /// <typeparam name="T">The item type whose public properties form columns.</typeparam>
    /// <param name="items">The items to render as table rows.</param>
    /// <returns>Header names and row cell values for a Markdown table.</returns>
    public static (IEnumerable<string>, IEnumerable<IEnumerable<string>>) ToMarkdownTablePrecursors<T>(this IEnumerable<T> items)
    {
        var type = typeof(T);
        var properties = type.GetProperties();
        var header = properties.Select(prop => prop.Name);
        var rows = items.Select(item => properties.Select(prop => FormatCellValue(prop.GetValue(item), prop.PropertyType)));
        return new ValueTuple<IEnumerable<string>, IEnumerable<IEnumerable<string>>>(header, rows);
    }

    /// <summary>Renders a sequence of items as a GitHub-flavored Markdown table.</summary>
    /// <typeparam name="T">The item type whose public properties form columns.</typeparam>
    /// <param name="items">The items to render as table rows.</param>
    /// <returns>A Markdown table string.</returns>
    public static string ToMarkdownTable<T>(this IEnumerable<T> items)
    {
        var type = typeof(T);
        var properties = type.GetProperties();

        var header = "| " + string.Join(" | ", properties.Select(prop => prop.Name)) + " |";
        var separator = "| " + string.Join(" | ", properties.Select(_ => "---")) + " |";
        var lines = new List<string> { header, separator };
        lines.AddRange(items.Select(item => "| " + string.Join(" | ", properties.Select(prop => FormatCellValue(prop.GetValue(item), prop.PropertyType))) + " |"));

        return string.Join(IMarkdownSection.NewLine, lines);
    }

    private static string FormatCellValue(object? value, Type type)
    {
        if (value == null)
            return string.Empty;

        if (IsSimpleType(type) || HasCustomToString(value))
            return ToInvariantString(value)?.ReplaceLineEndings(string.Empty) ?? string.Empty;

        return "{}";
    }

    private static bool HasCustomToString(object? obj)
    {
        var toString = ToInvariantString(obj);
        return !string.IsNullOrEmpty(toString) && toString != obj?.GetType().ToString() && toString.Length < 128;
    }

    private static string? ToInvariantString(this object? obj) =>
        Convert.ToString(obj, CultureInfo.InvariantCulture);

    private static bool IsSimpleType(Type type) =>
        type.IsPrimitive ||
        new[] { typeof(Enum), typeof(string), typeof(decimal), typeof(DateTime), typeof(DateTimeOffset), typeof(TimeSpan), typeof(Guid) }.Contains(type) ||
        Convert.GetTypeCode(type) != TypeCode.Object;
}
