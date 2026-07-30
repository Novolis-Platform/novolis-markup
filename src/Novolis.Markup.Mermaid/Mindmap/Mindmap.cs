namespace Novolis.Markup.Mermaid;

/// <summary>Fluent builder for Mermaid <c>mindmap</c> syntax.</summary>
public sealed class Mindmap(string rootLabel) : IMermaidable
{
    private readonly MindmapNode _root = new(rootLabel, MindmapShape.Circle);

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>Root node.</summary>
    public MindmapNode Root => _root;

    /// <summary>Adds a child under the root.</summary>
    public MindmapNode AddChild(string label, MindmapShape shape = MindmapShape.Default) =>
        _root.AddChild(label, shape);

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine("mindmap");
        writer.IncreaseIndent();
        WriteNode(writer, _root);
        writer.DecreaseIndent();
        return writer;
    }

    private static void WriteNode(IIndentedStringBuilder writer, MindmapNode node)
    {
        writer.WriteLine(node.FormatLabel());
        writer.IncreaseIndent();
        foreach (var child in node.Children)
            WriteNode(writer, child);
        writer.DecreaseIndent();
    }
}
