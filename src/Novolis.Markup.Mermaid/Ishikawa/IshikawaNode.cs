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
