namespace Novolis.Markup.Mermaid;

/// <summary>An element that satisfies or relates to requirements.</summary>
public sealed class RequirementElement(string id, string type) : IMermaidable
{
    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine("element {0} {{", id);
        writer.IncreaseIndent();
        writer.WriteLine("type: {0}", type);
        writer.DecreaseIndent();
        writer.WriteLine("}");
        return writer;
    }
}
