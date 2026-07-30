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

/// <summary>Requirement relationship kinds.</summary>
public enum RequirementRelationType
{
    /// <summary><c>contains</c>.</summary>
    Contains,

    /// <summary><c>copies</c>.</summary>
    Copies,

    /// <summary><c>derives</c>.</summary>
    Derives,

    /// <summary><c>satisfies</c>.</summary>
    Satisfies,

    /// <summary><c>verifies</c>.</summary>
    Verifies,

    /// <summary><c>refines</c>.</summary>
    Refines,

    /// <summary><c>traces</c>.</summary>
    Traces,
}

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
