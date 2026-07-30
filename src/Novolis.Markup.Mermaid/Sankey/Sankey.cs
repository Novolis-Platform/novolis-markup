namespace Novolis.Markup.Mermaid;

/// <summary>Fluent builder for Mermaid <c>sankey-beta</c> diagrams.</summary>
public sealed class Sankey : IMermaidable
{
    private readonly List<(string Source, string Target, double Value)> _links = [];

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>Adds a flow from source to target with a numeric value.</summary>
    public Sankey AddLink(string source, string target, double value)
    {
        _links.Add((source, target, value));
        return this;
    }

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine("sankey-beta");
        writer.IncreaseIndent();
        foreach (var (source, target, value) in _links)
            writer.WriteLine("{0},{1},{2}", source, target, value);
        writer.DecreaseIndent();
        return writer;
    }
}
