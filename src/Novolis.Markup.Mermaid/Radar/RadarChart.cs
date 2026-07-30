namespace Novolis.Markup.Mermaid;

/// <summary>Fluent builder for Mermaid <c>radar-beta</c> charts.</summary>
public sealed class RadarChart(string? title = null) : IMermaidable
{
    private readonly List<string> _axes = [];
    private readonly List<(string Id, string Label, double[] Values)> _curves = [];
    private double? _max;
    private string _graticule = "circle";

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>Sets radar axes.</summary>
    public RadarChart WithAxes(params string[] axes)
    {
        _axes.Clear();
        _axes.AddRange(axes);
        return this;
    }

    /// <summary>Adds a named curve of values matching axis count.</summary>
    public RadarChart AddCurve(string id, string label, params double[] values)
    {
        _curves.Add((id, label, values));
        return this;
    }

    /// <summary>Sets the maximum axis value.</summary>
    public RadarChart WithMax(double max)
    {
        _max = max;
        return this;
    }

    /// <summary>Sets graticule style (<c>circle</c> or <c>polygon</c>).</summary>
    public RadarChart WithGraticule(string graticule)
    {
        _graticule = graticule;
        return this;
    }

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine("radar-beta");
        writer.IncreaseIndent();
        if (!string.IsNullOrWhiteSpace(title))
            writer.WriteLine("title {0}", title);
        writer.WriteLine("axis {0}", string.Join(", ", _axes));
        foreach (var (id, label, values) in _curves)
            writer.WriteLine("curve {0}[\"{1}\"]{{{2}}}", id, label, string.Join(", ", values));
        if (_max is not null)
            writer.WriteLine("max {0}", _max);
        writer.WriteLine("graticule {0}", _graticule);
        writer.DecreaseIndent();
        return writer;
    }
}
