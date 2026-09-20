namespace Novolis.Markup.Mermaid;

/// <summary>A gitGraph commit on a branch.</summary>
public class Commit : IMermaidable
{
    /// <summary>Creates a commit with a message (used as <c>id</c>) and branch name.</summary>
    public Commit(string message, string branch, DateTime? dateTime = null, CommitType type = CommitType.Normal, string? tag = null)
    {
        if (dateTime.HasValue)
            Id = new Hash(dateTime.Value);
        Message = message;
        Branch = branch;
        Type = type;
        Tag = tag;
    }

    /// <summary>Commit id / message.</summary>
    public string Message { get; }

    /// <summary>Branch this commit belongs to.</summary>
    public string Branch { get; }

    /// <summary>Commit marker type.</summary>
    public CommitType Type { get; }

    /// <summary>Optional tag (release label).</summary>
    public string? Tag { get; }

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine(Format());
        return writer;
    }

    internal string Format()
    {
        var parts = new List<string> { "commit" };
        if (!string.IsNullOrWhiteSpace(Message))
            parts.Add($"id: \"{Message}\"");
        if (Type != CommitType.Normal)
            parts.Add($"type: {Type.ToString().ToUpperInvariant()}");
        if (!string.IsNullOrWhiteSpace(Tag))
            parts.Add($"tag: \"{Tag}\"");
        return string.Join(' ', parts);
    }
}
