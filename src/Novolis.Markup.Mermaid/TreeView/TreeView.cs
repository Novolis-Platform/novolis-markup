namespace Novolis.Markup.Mermaid;

/// <summary>Fluent builder for Mermaid <c>treeView</c> / hierarchical tree diagrams.</summary>
public sealed class TreeView(string rootLabel) : IMermaidable
{
    private readonly TreeViewNode _root = new(rootLabel);

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>Root node.</summary>
    public TreeViewNode Root => _root;

    /// <summary>Adds a child under the root.</summary>
    public TreeViewNode AddChild(string label) => _root.AddChild(label);

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine("treeView");
        writer.IncreaseIndent();
        WriteNode(writer, _root);
        writer.DecreaseIndent();
        return writer;
    }

    private static void WriteNode(IIndentedStringBuilder writer, TreeViewNode node)
    {
        writer.WriteLine(node.Label);
        writer.IncreaseIndent();
        foreach (var child in node.Children)
            WriteNode(writer, child);
        writer.DecreaseIndent();
    }
}
