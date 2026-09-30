namespace Novolis.Markup.Mermaid;

/// <summary>A relationship between requirement diagram nodes.</summary>
public sealed class RequirementRelation(string from, RequirementRelationType type, string to) : IMermaidable
{
    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var token = type.ToString().ToLowerInvariant();
        var writer = new IndentedStringBuilder();
        writer.WriteLine("{0} - {1} -> {2}", from, token, to);
        return writer;
    }
}
