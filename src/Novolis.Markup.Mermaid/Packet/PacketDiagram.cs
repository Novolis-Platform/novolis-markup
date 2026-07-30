namespace Novolis.Markup.Mermaid;

/// <summary>Fluent builder for Mermaid <c>packet-beta</c> diagrams.</summary>
public sealed class PacketDiagram(string? title = null) : IMermaidable
{
    private readonly List<(int Start, int End, string Label)> _fields = [];

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>Adds a bit-range field.</summary>
    public PacketDiagram AddField(int startBit, int endBit, string label)
    {
        _fields.Add((startBit, endBit, label));
        return this;
    }

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine("packet-beta");
        writer.IncreaseIndent();
        if (!string.IsNullOrWhiteSpace(title))
            writer.WriteLine("title {0}", title);
        foreach (var (start, end, label) in _fields)
            writer.WriteLine("{0}-{1}: \"{2}\"", start, end, label);
        writer.DecreaseIndent();
        return writer;
    }
}
