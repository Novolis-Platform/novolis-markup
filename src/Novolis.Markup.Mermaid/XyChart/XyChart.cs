namespace Novolis.Markup.Mermaid;

/// <summary>Fluent builder for Mermaid <c>xychart</c> (bar / line) diagrams.</summary>
public class XyChart : IMermaidable
{
    readonly List<string> _xCategories = [];
    bool _horizontal;
    string? _xTitle;
    string? _xRange;
    string? _yTitle;
    string? _yRange;

    /// <summary>Creates an XY chart with an optional title.</summary>
    public XyChart(string? title = null) => Title = title;

    /// <summary>Chart title.</summary>
    public string? Title { get; }

    /// <summary>X axis (optional).</summary>
    public Axis? XAxis { get; private set; }

    /// <summary>Y axis (optional).</summary>
    public Axis? YAxis { get; private set; }

    /// <summary>Line and bar series.</summary>
    public List<Series> Series { get; } = [];

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>Draws the chart horizontally.</summary>
    public XyChart Horizontal()
    {
        _horizontal = true;
        return this;
    }

    /// <summary>Adds a series.</summary>
    public XyChart AddSeries(Series series)
    {
        Series.Add(series);
        return this;
    }

    /// <summary>Adds many series.</summary>
    public void AddSeries(IEnumerable<Series> series) => Series.AddRange(series);

    /// <summary>Sets the x-axis from an <see cref="Axis"/> (title only; range is auto).</summary>
    public void SetXAxis(Axis axis)
    {
        XAxis = axis;
        _xTitle = axis.Title;
    }

    /// <summary>Sets the y-axis from an <see cref="Axis"/>.</summary>
    public void SetYAxis(Axis axis)
    {
        YAxis = axis;
        _yTitle = axis.Title;
    }

    /// <summary>Sets categorical x-axis labels.</summary>
    public XyChart WithXAxis(string? title, params string[] categories)
    {
        _xTitle = title;
        _xCategories.Clear();
        _xCategories.AddRange(categories);
        _xRange = null;
        return this;
    }

    /// <summary>Sets a numeric x-axis range.</summary>
    public XyChart WithXAxisRange(string? title, double min, double max)
    {
        _xTitle = title;
        _xCategories.Clear();
        _xRange = $"{Format(min)} --> {Format(max)}";
        return this;
    }

    /// <summary>Sets the y-axis title and optional numeric range.</summary>
    public XyChart WithYAxis(string? title, double? min = null, double? max = null)
    {
        _yTitle = title;
        _yRange = min is not null && max is not null ? $"{Format(min.Value)} --> {Format(max.Value)}" : null;
        return this;
    }

    /// <summary>Adds a bar plot.</summary>
    public XyChart Bar(string? name, params double[] values)
    {
        var series = new Series(name ?? string.Empty, XySeriesKind.Bar);
        for (var i = 0; i < values.Length; i++)
            series.Points.Add(new Point(i, values[i]));
        Series.Add(series);
        return this;
    }

    /// <summary>Adds a line plot.</summary>
    public XyChart Line(string? name, params double[] values)
    {
        var series = new Series(name ?? string.Empty, XySeriesKind.Line);
        for (var i = 0; i < values.Length; i++)
            series.Points.Add(new Point(i, values[i]));
        Series.Add(series);
        return this;
    }

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine(_horizontal ? "xychart horizontal" : "xychart");
        writer.IncreaseIndent();
        if (!string.IsNullOrWhiteSpace(Title))
            writer.WriteLine("title {0}", Quote(Title));

        if (_xCategories.Count > 0)
        {
            var cats = string.Join(", ", _xCategories.Select(Quote));
            if (string.IsNullOrWhiteSpace(_xTitle))
                writer.WriteLine("x-axis [{0}]", cats);
            else
                writer.WriteLine("x-axis {0} [{1}]", Quote(_xTitle), cats);
        }
        else if (!string.IsNullOrWhiteSpace(_xRange))
        {
            if (string.IsNullOrWhiteSpace(_xTitle))
                writer.WriteLine("x-axis {0}", _xRange);
            else
                writer.WriteLine("x-axis {0} {1}", Quote(_xTitle!), _xRange);
        }
        else if (!string.IsNullOrWhiteSpace(_xTitle))
        {
            writer.WriteLine("x-axis {0}", Quote(_xTitle));
        }

        if (!string.IsNullOrWhiteSpace(_yTitle) && !string.IsNullOrWhiteSpace(_yRange))
            writer.WriteLine("y-axis {0} {1}", Quote(_yTitle), _yRange);
        else if (!string.IsNullOrWhiteSpace(_yTitle))
            writer.WriteLine("y-axis {0}", Quote(_yTitle));
        else if (!string.IsNullOrWhiteSpace(_yRange))
            writer.WriteLine("y-axis {0}", _yRange);

        foreach (var series in Series)
            writer.WriteLine(series.Format());

        writer.DecreaseIndent();
        return writer;
    }

    static string Quote(string value) =>
        value.Contains(' ') || value.Contains('"') ? $"\"{value.Replace("\"", "#quot;", StringComparison.Ordinal)}\"" : value;

    static string Format(double value) => value.ToString(System.Globalization.CultureInfo.InvariantCulture);
}
