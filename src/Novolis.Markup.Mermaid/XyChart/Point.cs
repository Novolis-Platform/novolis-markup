namespace Novolis.Markup.Mermaid;

/// <summary>Represents Point.</summary>
public class Point(int x, int y) : IMermaidable
{
    /// <summary>X.</summary>
    public double X { get; } = x;
    /// <summary>Y.</summary>
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
