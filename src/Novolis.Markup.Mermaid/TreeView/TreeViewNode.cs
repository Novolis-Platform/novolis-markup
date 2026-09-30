namespace Novolis.Markup.Mermaid;

/// <summary>A node in a tree-view hierarchy.</summary>
public sealed class TreeViewNode(string label) : IMermaidable
{
    private readonly List<TreeViewNode> _children = [];

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>Node label.</summary>
    public string Label { get; } = label;

    /// <summary>Child nodes.</summary>
    public IReadOnlyList<TreeViewNode> Children => _children;

    /// <summary>Adds a child.</summary>
    public TreeViewNode AddChild(string childLabel)
    {
        var child = new TreeViewNode(childLabel);
        _children.Add(child);
        return child;
    }

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine(Label);
        return writer;
    }
}
