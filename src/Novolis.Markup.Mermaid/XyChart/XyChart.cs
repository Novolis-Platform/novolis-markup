namespace Novolis.Markup.Mermaid;

/// <remarks>Beta</remarks>
public class XyChart(string? title) : IMermaidable
{
    /// <summary>Title.</summary>
    public string? Title { get; } = title;
    /// <summary>YAxis.</summary>
    public Axis XAxis { get; private set; } = null!;
    /// <summary>Series.</summary>
    public Axis YAxis { get; private set; } = null!;
    /// <summary>Series.</summary>
    public List<Series> Series { get; } = new();

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();
    
    /// <summary>Adds an item.</summary>
    public void AddSeries(Series series) => Series.Add(series);
/// <summary>Sets a value.</summary>

    public void AddSeries(IEnumerable<Series> series) => Series.AddRange(series);
    
    /// <summary>Sets a value.</summary>
    public void SetXAxis(Axis axis) => XAxis = axis;
    /// <summary>Sets a value.</summary>
    
    public void SetYAxis(Axis axis) => YAxis = axis;
    
    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.Write("xyChart-beta");
        writer.IncreaseIndent();
        writer.Write("title \"{0}\"", Title);
        writer.WriteLine("x-axis {0} {1}", XAxis, "CCC");
        writer.WriteLine("y-axis {0}", YAxis);
        foreach (var series in Series)
        {
            writer.WriteLine(series.GetBuilder());
        }
        
        writer.DecreaseIndent();
        
        return writer;
    }
}
