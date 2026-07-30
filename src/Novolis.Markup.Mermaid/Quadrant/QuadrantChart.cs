namespace Novolis.Markup.Mermaid;

/// <summary>Fluent builder for Mermaid <c>quadrantChart</c>.</summary>
public sealed class QuadrantChart(string title) : IMermaidable
{
    private string _xAxis = "Low --> High";
    private string _yAxis = "Low --> High";
    private string _q1 = "We should";
    private string _q2 = "We could";
    private string _q3 = "We won't";
    private string _q4 = "We must";
    private readonly List<(string Name, double X, double Y)> _points = [];

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>Sets axis labels (use Mermaid <c>A --&gt; B</c> form).</summary>
    public QuadrantChart WithAxes(string xAxis, string yAxis)
    {
        _xAxis = xAxis;
        _yAxis = yAxis;
        return this;
    }

    /// <summary>Sets quadrant titles (1=top-right … 4=bottom-right in Mermaid order).</summary>
    public QuadrantChart WithQuadrants(string q1, string q2, string q3, string q4)
    {
        _q1 = q1;
        _q2 = q2;
        _q3 = q3;
        _q4 = q4;
        return this;
    }

    /// <summary>Adds a plotted point in 0–1 coordinates.</summary>
    public QuadrantChart AddPoint(string name, double x, double y)
    {
        _points.Add((name, x, y));
        return this;
    }

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine("quadrantChart");
        writer.IncreaseIndent();
        writer.WriteLine("title {0}", title);
        writer.WriteLine("x-axis {0}", _xAxis);
        writer.WriteLine("y-axis {0}", _yAxis);
        writer.WriteLine("quadrant-1 {0}", _q1);
        writer.WriteLine("quadrant-2 {0}", _q2);
        writer.WriteLine("quadrant-3 {0}", _q3);
        writer.WriteLine("quadrant-4 {0}", _q4);
        foreach (var (name, x, y) in _points)
            writer.WriteLine("{0}: [{1}, {2}]", name, x, y);
        writer.DecreaseIndent();
        return writer;
    }
}
