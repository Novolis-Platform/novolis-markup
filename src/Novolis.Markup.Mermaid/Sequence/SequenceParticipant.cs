namespace Novolis.Markup.Mermaid;

/// <summary>A sequence-diagram participant or actor declaration.</summary>
public sealed class SequenceParticipant(string id, string? alias = null, bool asActor = false) : IMermaidable
{
    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>Participant identifier used in messages.</summary>
    public string ParticipantId { get; } = id;

    /// <summary>Optional display alias.</summary>
    public string? Alias { get; } = alias;

    /// <summary>When true, emits <c>actor</c> instead of <c>participant</c>.</summary>
    public bool AsActor { get; } = asActor;

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var keyword = AsActor ? "actor" : "participant";
        var writer = new IndentedStringBuilder();
        if (string.IsNullOrWhiteSpace(Alias))
            writer.WriteLine("{0} {1}", keyword, ParticipantId);
        else
            writer.WriteLine("{0} {1} as {2}", keyword, ParticipantId, Alias);
        return writer;
    }
}
