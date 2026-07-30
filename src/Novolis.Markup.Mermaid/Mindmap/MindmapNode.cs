namespace Novolis.Markup.Mermaid;

/// <summary>Mindmap node shape wrappers.</summary>
public enum MindmapShape
{
    /// <summary>Plain text node.</summary>
    Default,

    /// <summary>Square brackets.</summary>
    Square,

    /// <summary>Rounded parentheses.</summary>
    Rounded,

    /// <summary>Double-circle root style.</summary>
    Circle,

    /// <summary>Bang / cloud style.</summary>
    Bang,

    /// <summary>Hexagon.</summary>
    Hexagon,
}

/// <summary>A node in a mindmap tree.</summary>
public sealed class MindmapNode(string label, MindmapShape shape = MindmapShape.Default) : IMermaidable
{
    private readonly List<MindmapNode> _children = [];

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>Node label.</summary>
    public string Label { get; } = label;

    /// <summary>Shape wrapper.</summary>
    public MindmapShape Shape { get; } = shape;

    /// <summary>Child nodes.</summary>
    public IReadOnlyList<MindmapNode> Children => _children;

    /// <summary>Adds a child node.</summary>
    public MindmapNode AddChild(string childLabel, MindmapShape childShape = MindmapShape.Default)
    {
        var child = new MindmapNode(childLabel, childShape);
        _children.Add(child);
        return child;
    }

    /// <summary>Formats the label with shape delimiters.</summary>
    public string FormatLabel() => Shape switch
    {
        MindmapShape.Square => $"[{Label}]",
        MindmapShape.Rounded => $"({Label})",
        MindmapShape.Circle => $"(({Label}))",
        MindmapShape.Bang => $"){Label}(",
        MindmapShape.Hexagon => $"{{{{{Label}}}}}",
        _ => Label,
    };

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine(FormatLabel());
        return writer;
    }
}
