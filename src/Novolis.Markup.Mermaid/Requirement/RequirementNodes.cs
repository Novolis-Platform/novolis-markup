namespace Novolis.Markup.Mermaid;

/// <summary>A requirement block.</summary>
public sealed class RequirementNode(string name, string requirementId, string text, string? risk = null, string? verifymethod = null) : IMermaidable
{
    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine("requirement {0} {{", name);
        writer.IncreaseIndent();
        writer.WriteLine("id: {0}", requirementId);
        writer.WriteLine("text: {0}", text);
        if (!string.IsNullOrWhiteSpace(risk))
            writer.WriteLine("risk: {0}", risk);
        if (!string.IsNullOrWhiteSpace(verifymethod))
            writer.WriteLine("verifymethod: {0}", verifymethod);
        writer.DecreaseIndent();
        writer.WriteLine("}");
        return writer;
    }
}
