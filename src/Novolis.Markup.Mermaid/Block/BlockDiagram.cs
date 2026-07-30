namespace Novolis.Markup.Mermaid;

/// <summary>Fluent builder for Mermaid <c>block-beta</c> diagrams.</summary>
public sealed class BlockDiagram(int columns = 1) : IMermaidable
{
    private readonly List<string> _lines = [];

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>Adds a block id with label.</summary>
    public BlockDiagram Block(string id, string label)
    {
        _lines.Add($"{id}[\"{label}\"]");
        return this;
    }

    /// <summary>Adds space spanning columns.</summary>
    public BlockDiagram Space(int columns = 1)
    {
        _lines.Add(columns <= 1 ? "space" : $"space:{columns}");
        return this;
    }

    /// <summary>Adds a block edge.</summary>
    public BlockDiagram Edge(string from, string to, string? label = null)
    {
        _lines.Add(string.IsNullOrWhiteSpace(label) ? $"{from} --> {to}" : $"{from} -- \"{label}\" --> {to}");
        return this;
    }

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine("block-beta");
        writer.IncreaseIndent();
        writer.WriteLine("columns {0}", columns);
        foreach (var line in _lines)
            writer.WriteLine(line);
        writer.DecreaseIndent();
        return writer;
    }
}
