namespace Novolis.Markup.Mermaid;

/// <summary>UML association operators for use case diagrams.</summary>
public enum UseCaseRelation
{
    /// <summary>Solid association <c>--&gt;</c>.</summary>
    Association,

    /// <summary>Include <c>..&gt; : include</c>.</summary>
    Include,

    /// <summary>Extend <c>..&gt; : extend</c>.</summary>
    Extend,

    /// <summary>Generalization <c>--|&gt;</c>.</summary>
    Generalization,
}

/// <summary>Fluent builder for Mermaid <c>usecase-beta</c> diagrams.</summary>
public sealed class UseCaseDiagram : IMermaidable
{
    readonly List<string> _lines = [];
    Direction? _direction;

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>Sets layout direction.</summary>
    public UseCaseDiagram Direction(Direction direction)
    {
        _direction = direction;
        return this;
    }

    /// <summary>Declares an actor.</summary>
    public UseCaseDiagram Actor(string id, string? label = null)
    {
        _lines.Add(string.IsNullOrWhiteSpace(label) || label == id
            ? $"actor {id}"
            : $"actor {id}(\"{Escape(label)}\")");
        return this;
    }

    /// <summary>Declares an ellipse use case.</summary>
    public UseCaseDiagram UseCase(string id, string? label = null)
    {
        _lines.Add(string.IsNullOrWhiteSpace(label) || label == id
            ? $"{id}({id})"
            : $"{id}(\"{Escape(label)}\")");
        return this;
    }

    /// <summary>Declares a rectangular use case.</summary>
    public UseCaseDiagram UseCaseRect(string id, string? label = null)
    {
        _lines.Add(string.IsNullOrWhiteSpace(label) || label == id
            ? $"{id}[{id}]"
            : $"{id}[\"{Escape(label)}\"]");
        return this;
    }

    /// <summary>Opens a system boundary. Call <see cref="End"/> to close it.</summary>
    public UseCaseDiagram SystemBoundary(string title)
    {
        _lines.Add($"systemBoundary \"{Escape(title)}\"");
        return this;
    }

    /// <summary>Closes the current <c>systemBoundary</c> block.</summary>
    public UseCaseDiagram End()
    {
        _lines.Add("end");
        return this;
    }

    /// <summary>Adds a relationship between two identifiers.</summary>
    public UseCaseDiagram Rel(string from, string to, UseCaseRelation relation = UseCaseRelation.Association, string? label = null)
    {
        var op = relation switch
        {
            UseCaseRelation.Include => "..>",
            UseCaseRelation.Extend => "..>",
            UseCaseRelation.Generalization => "--|>",
            _ => "-->",
        };
        _lines.Add(relation switch
        {
            UseCaseRelation.Include => $"{from} {op} : include {to}",
            UseCaseRelation.Extend => $"{from} {op} : extend {to}",
            _ when !string.IsNullOrWhiteSpace(label) => $"{from} {op} {to} : {label}",
            _ => $"{from} {op} {to}",
        });
        return this;
    }

    /// <summary>Attaches a note to an actor or use case.</summary>
    public UseCaseDiagram Note(string target, string text)
    {
        _lines.Add($"note for {target} \"{Escape(text)}\"");
        return this;
    }

    /// <summary>Appends a raw statement (advanced syntax / parse round-trip).</summary>
    public UseCaseDiagram AddStatement(string statement)
    {
        _lines.Add(statement);
        return this;
    }

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine("usecase-beta");
        writer.IncreaseIndent();
        if (_direction is { } dir)
            writer.WriteLine("direction {0}", dir.GetBuilder());
        var depth = 0;
        foreach (var line in _lines)
        {
            if (line.Equals("end", StringComparison.Ordinal) && depth > 0)
            {
                writer.DecreaseIndent();
                writer.WriteLine("end");
                depth--;
                continue;
            }

            writer.WriteLine(line);
            if (line.StartsWith("systemBoundary", StringComparison.Ordinal))
            {
                writer.IncreaseIndent();
                depth++;
            }
        }

        writer.DecreaseIndent();
        return writer;
    }

    static string Escape(string value) => value.Replace("\"", "#quot;", StringComparison.Ordinal);
}
