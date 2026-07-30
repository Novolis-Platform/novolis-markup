namespace Novolis.Markup.Mermaid;

/// <summary>Fluent builder for Mermaid <c>venn-beta</c> diagrams.</summary>
public sealed class VennDiagram : IMermaidable
{
    private readonly List<(string Id, string Label)> _sets = [];
    private readonly List<(string Left, string Right, string Label)> _unions = [];

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>Adds a set.</summary>
    public VennDiagram AddSet(string id, string label)
    {
        _sets.Add((id, label));
        return this;
    }

    /// <summary>Adds a labeled union between two sets.</summary>
    public VennDiagram AddUnion(string left, string right, string label)
    {
        _unions.Add((left, right, label));
        return this;
    }

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine("venn-beta");
        writer.IncreaseIndent();
        foreach (var (id, label) in _sets)
            writer.WriteLine("set {0}[\"{1}\"]", id, label);
        foreach (var (left, right, label) in _unions)
            writer.WriteLine("union {0}, {1}[\"{2}\"]", left, right, label);
        writer.DecreaseIndent();
        return writer;
    }
}
