namespace Novolis.Markup.Mermaid;

/// <summary>A cause node in an Ishikawa (fishbone) diagram.</summary>
public sealed class IshikawaNode(string label) : IMermaidable
{
    readonly List<IshikawaNode> _children = [];

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>Cause label.</summary>
    public string Label { get; } = label;

    /// <summary>Nested causes.</summary>
    public IReadOnlyList<IshikawaNode> Children => _children;

    /// <summary>Adds a nested cause.</summary>
    public IshikawaNode AddCause(string childLabel)
    {
        var child = new IshikawaNode(childLabel);
        _children.Add(child);
        return child;
    }

    internal void AddChild(IshikawaNode child) => _children.Add(child);

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine(Label);
        return writer;
    }
}

/// <summary>Fluent builder for Mermaid <c>ishikawa-beta</c> fishbone diagrams.</summary>
public sealed class IshikawaDiagram(string problem) : IMermaidable
{
    readonly IshikawaNode _root = new(problem);

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>The problem / effect at the fish head.</summary>
    public IshikawaNode Root => _root;

    /// <summary>Adds a top-level cause category.</summary>
    public IshikawaNode AddCause(string label) => _root.AddCause(label);

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine("ishikawa-beta");
        writer.IncreaseIndent();
        WriteNode(writer, _root);
        writer.DecreaseIndent();
        return writer;
    }

    static void WriteNode(IIndentedStringBuilder writer, IshikawaNode node)
    {
        writer.WriteLine(node.Label);
        writer.IncreaseIndent();
        foreach (var child in node.Children)
            WriteNode(writer, child);
        writer.DecreaseIndent();
    }
}
