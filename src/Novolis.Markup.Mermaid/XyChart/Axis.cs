namespace Novolis.Markup.Mermaid;

/// <summary>Represents Axis.</summary>
public class Axis(string xAxis, bool logarithmic = false) : IMermaidable
{
    /// <summary>Title.</summary>
    public string Title { get; } = xAxis;
/// <summary>Logarithmic.</summary>

    public bool Logarithmic { get; } = logarithmic;

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine("axis {0}", Title);
        writer.IncreaseIndent();
        writer.WriteLine("log {0}", Logarithmic.ToString().ToLower());
        writer.DecreaseIndent();
        return writer;
    }
}
