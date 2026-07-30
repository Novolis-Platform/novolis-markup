namespace Novolis.Markup.Mermaid;

/// <summary>Fluent builder for Mermaid <c>stateDiagram-v2</c> syntax.</summary>
public sealed class StateDiagram : IMermaidable
{
    private readonly List<string> _lines = [];

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>Adds a transition. Use <c>[*]</c> for start/end.</summary>
    public StateDiagram Transition(string from, string to, string? label = null)
    {
        _lines.Add(string.IsNullOrWhiteSpace(label) ? $"{from} --> {to}" : $"{from} --> {to} : {label}");
        return this;
    }

    /// <summary>Declares a named state with optional description.</summary>
    public StateDiagram State(string name, string? description = null)
    {
        _lines.Add(string.IsNullOrWhiteSpace(description) ? $"state {name}" : $"state \"{description}\" as {name}");
        return this;
    }

    /// <summary>Starts a composite state block.</summary>
    public StateDiagram BeginComposite(string name)
    {
        _lines.Add($"state {name} {{");
        return this;
    }

    /// <summary>Closes a composite state block.</summary>
    public StateDiagram EndComposite()
    {
        _lines.Add("}");
        return this;
    }

    /// <summary>Adds a note.</summary>
    public StateDiagram Note(string position, string state, string text)
    {
        _lines.Add($"note {position} of {state}");
        _lines.Add($"  {text}");
        return this;
    }

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine("stateDiagram-v2");
        writer.IncreaseIndent();
        foreach (var line in _lines)
            writer.WriteLine(line);
        writer.DecreaseIndent();
        return writer;
    }
}
