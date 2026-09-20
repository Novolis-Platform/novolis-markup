namespace Novolis.Markup.Mermaid;

/// <summary>A 2D chart point. For XY series, <see cref="Y"/> is the plotted value.</summary>
public class Point(double x, double y) : IMermaidable
{
    /// <summary>X (category index or numeric x).</summary>
    public double X { get; } = x;

    /// <summary>Y (plotted value).</summary>
    public double Y { get; } = y;

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine("{0} {1}", X, Y);
        return writer;
    }
}
