namespace Novolis.Markup.Mermaid;

/// <summary>Event-modeling entity kinds (compact and relaxed tokens).</summary>
public enum EventModelingEntityType
{
    /// <summary>UI (<c>ui</c>).</summary>
    Ui,

    /// <summary>Processor (<c>pcr</c> / <c>processor</c>).</summary>
    Processor,

    /// <summary>Command (<c>cmd</c> / <c>command</c>).</summary>
    Command,

    /// <summary>Read model (<c>rmo</c> / <c>readmodel</c>).</summary>
    ReadModel,

    /// <summary>Event (<c>evt</c> / <c>event</c>).</summary>
    Event,
}

/// <summary>Fluent builder for Mermaid <c>eventmodeling</c> diagrams.</summary>
public sealed class EventModelingDiagram : IMermaidable
{
    readonly List<string> _lines = [];

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>Adds a time frame (<c>tf</c> compact form).</summary>
    public EventModelingDiagram TimeFrame(string id, EventModelingEntityType type, string entity, string? inlineData = null)
    {
        var line = $"tf {id} {Token(type)} {entity}";
        if (!string.IsNullOrWhiteSpace(inlineData))
            line += " {" + inlineData.Trim().Trim('{', '}') + "}";
        _lines.Add(line);
        return this;
    }

    /// <summary>Adds a reset frame that breaks inferred flow.</summary>
    public EventModelingDiagram ResetFrame(string id)
    {
        _lines.Add($"rf {id}");
        return this;
    }

    /// <summary>Adds a named data block referenced as <c>[[id]]</c>.</summary>
    public EventModelingDiagram DataBlock(string id, string body, string? dataType = null)
    {
        var type = string.IsNullOrWhiteSpace(dataType) ? string.Empty : $"`{dataType}`";
        _lines.Add($"data {id} {type}{{");
        foreach (var row in body.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n'))
            _lines.Add(row);
        _lines.Add("}");
        return this;
    }

    /// <summary>Appends a raw DSL statement.</summary>
    public EventModelingDiagram AddStatement(string statement)
    {
        _lines.Add(statement);
        return this;
    }

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine("eventmodeling");
        writer.IncreaseIndent();
        foreach (var line in _lines)
            writer.WriteLine(line);
        writer.DecreaseIndent();
        return writer;
    }

    internal static string Token(EventModelingEntityType type) => type switch
    {
        EventModelingEntityType.Ui => "ui",
        EventModelingEntityType.Processor => "pcr",
        EventModelingEntityType.Command => "cmd",
        EventModelingEntityType.ReadModel => "rmo",
        EventModelingEntityType.Event => "evt",
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };

    internal static bool TryParseType(string token, out EventModelingEntityType type)
    {
        type = token.ToLowerInvariant() switch
        {
            "ui" => EventModelingEntityType.Ui,
            "pcr" or "processor" => EventModelingEntityType.Processor,
            "cmd" or "command" => EventModelingEntityType.Command,
            "rmo" or "readmodel" => EventModelingEntityType.ReadModel,
            "evt" or "event" => EventModelingEntityType.Event,
            _ => (EventModelingEntityType)(-1),
        };
        return Enum.IsDefined(type);
    }
}
