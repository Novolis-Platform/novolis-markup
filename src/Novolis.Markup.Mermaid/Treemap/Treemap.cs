namespace Novolis.Markup.Mermaid;

/// <summary>Fluent builder for Mermaid <c>treemap-beta</c> diagrams.</summary>
public sealed class Treemap : IMermaidable
{
    private readonly List<(string Label, double Value)> _leaves = [];

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>Adds a leaf value.</summary>
    public Treemap AddLeaf(string label, double value)
    {
        _leaves.Add((label, value));
        return this;
    }

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine("treemap-beta");
        writer.IncreaseIndent();
        foreach (var (label, value) in _leaves)
            writer.WriteLine("\"{0}\": {1}", label, value);
        writer.DecreaseIndent();
        return writer;
    }
}
