namespace Novolis.Markup.Mermaid;

/// <summary>Fluent builder for Mermaid <c>architecture-beta</c> diagrams.</summary>
public sealed class ArchitectureDiagram : IMermaidable
{
    private readonly List<string> _lines = [];

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>Adds a group.</summary>
    public ArchitectureDiagram Group(string id, string label, string? icon = null)
    {
        _lines.Add(string.IsNullOrWhiteSpace(icon)
            ? $"group {id}[{label}]"
            : $"group {id}({icon})[{label}]");
        return this;
    }

    /// <summary>Adds a service node in a group.</summary>
    public ArchitectureDiagram Service(string id, string label, string? icon = null, string? inGroup = null)
    {
        var iconPart = string.IsNullOrWhiteSpace(icon) ? string.Empty : $"({icon})";
        var line = $"service {id}{iconPart}[{label}]";
        if (!string.IsNullOrWhiteSpace(inGroup))
            line += $" in {inGroup}";
        _lines.Add(line);
        return this;
    }

    /// <summary>Adds a junction node.</summary>
    public ArchitectureDiagram Junction(string id, string? inGroup = null)
    {
        _lines.Add(string.IsNullOrWhiteSpace(inGroup) ? $"junction {id}" : $"junction {id} in {inGroup}");
        return this;
    }

    /// <summary>Adds an edge between nodes.</summary>
    public ArchitectureDiagram Edge(string from, string to, string? label = null)
    {
        _lines.Add(string.IsNullOrWhiteSpace(label) ? $"{from}:{to}" : $"{from}:{to} : {label}");
        return this;
    }

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine("architecture-beta");
        writer.IncreaseIndent();
        foreach (var line in _lines)
            writer.WriteLine(line);
        writer.DecreaseIndent();
        return writer;
    }
}
