namespace Novolis.Markup.Mermaid;

/// <summary>XY chart series kind.</summary>
public enum XySeriesKind
{
    /// <summary><c>line</c> plot.</summary>
    Line,

    /// <summary><c>bar</c> plot.</summary>
    Bar,
}

/// <summary>A named line or bar series on an XY chart.</summary>
public class Series(string name, XySeriesKind kind = XySeriesKind.Line) : IMermaidable
{
    /// <summary>Legend name.</summary>
    public string Name { get; } = name;

    /// <summary>Plot kind.</summary>
    public XySeriesKind Kind { get; } = kind;

    /// <summary>Data points. Y values are plotted in order; X is the category index unless a numeric x-axis is used.</summary>
    public List<Point> Points { get; } = [];

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine(Format());
        return writer;
    }

    internal string Format()
    {
        var keyword = Kind == XySeriesKind.Bar ? "bar" : "line";
        var values = string.Join(", ", Points.Select(p => p.Y.ToString(System.Globalization.CultureInfo.InvariantCulture)));
        return string.IsNullOrWhiteSpace(Name)
            ? $"{keyword} [{values}]"
            : $"{keyword} \"{Name}\" [{values}]";
    }
}
