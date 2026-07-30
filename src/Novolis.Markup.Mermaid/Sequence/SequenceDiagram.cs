namespace Novolis.Markup.Mermaid;

/// <summary>Fluent builder for Mermaid <c>sequenceDiagram</c> syntax.</summary>
public sealed class SequenceDiagram : IMermaidable
{
    private readonly List<SequenceParticipant> _participants = [];
    private readonly List<string> _lines = [];

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>Adds a participant (or alias).</summary>
    public SequenceDiagram AddParticipant(string id, string? alias = null, bool asActor = false)
    {
        _participants.Add(new SequenceParticipant(id, alias, asActor));
        return this;
    }

    /// <summary>Adds a solid arrow message (<c>-&gt;&gt;</c>).</summary>
    public SequenceDiagram Message(string from, string to, string text, SequenceArrow arrow = SequenceArrow.Solid)
    {
        _lines.Add($"{from}{arrow.ToToken()}{to}: {text}");
        return this;
    }

    /// <summary>Adds a note relative to one participant.</summary>
    public SequenceDiagram NoteOver(string participant, string text) =>
        Note("over", participant, text);

    /// <summary>Adds a note to the left or right of a participant.</summary>
    public SequenceDiagram Note(string position, string participant, string text)
    {
        _lines.Add($"Note {position} {participant}: {text}");
        return this;
    }

    /// <summary>Starts a named block (<c>loop</c>, <c>alt</c>, <c>opt</c>, <c>par</c>, <c>critical</c>, <c>break</c>).</summary>
    public SequenceDiagram BeginBlock(string keyword, string? title = null)
    {
        _lines.Add(string.IsNullOrWhiteSpace(title) ? keyword : $"{keyword} {title}");
        return this;
    }

    /// <summary>Adds an <c>else</c> divider inside an <c>alt</c>/<c>par</c>/<c>critical</c> block.</summary>
    public SequenceDiagram Else(string? title = null)
    {
        _lines.Add(string.IsNullOrWhiteSpace(title) ? "else" : $"else {title}");
        return this;
    }

    /// <summary>Closes the current block.</summary>
    public SequenceDiagram End()
    {
        _lines.Add("end");
        return this;
    }

    /// <summary>Marks participant activation.</summary>
    public SequenceDiagram Activate(string participant)
    {
        _lines.Add($"activate {participant}");
        return this;
    }

    /// <summary>Marks participant deactivation.</summary>
    public SequenceDiagram Deactivate(string participant)
    {
        _lines.Add($"deactivate {participant}");
        return this;
    }

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine("sequenceDiagram");
        writer.IncreaseIndent();
        foreach (var p in _participants)
            writer.WriteLine(p.GetBuilder());
        foreach (var line in _lines)
            writer.WriteLine(line);
        writer.DecreaseIndent();
        return writer;
    }
}
