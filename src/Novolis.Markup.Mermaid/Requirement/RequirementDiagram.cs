namespace Novolis.Markup.Mermaid;

/// <summary>Fluent builder for Mermaid <c>requirementDiagram</c>.</summary>
public sealed class RequirementDiagram : IMermaidable
{
    private readonly List<RequirementNode> _requirements = [];
    private readonly List<RequirementElement> _elements = [];
    private readonly List<RequirementRelation> _relations = [];

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>Adds a requirement definition.</summary>
    public RequirementDiagram AddRequirement(RequirementNode node)
    {
        _requirements.Add(node);
        return this;
    }

    /// <summary>Adds an element definition.</summary>
    public RequirementDiagram AddElement(RequirementElement element)
    {
        _elements.Add(element);
        return this;
    }

    /// <summary>Adds a relationship.</summary>
    public RequirementDiagram AddRelation(RequirementRelation relation)
    {
        _relations.Add(relation);
        return this;
    }

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine("requirementDiagram");
        writer.IncreaseIndent();
        foreach (var req in _requirements)
            writer.WriteLine(req.GetBuilder());
        foreach (var el in _elements)
            writer.WriteLine(el.GetBuilder());
        foreach (var rel in _relations)
            writer.WriteLine(rel.GetBuilder());
        writer.DecreaseIndent();
        return writer;
    }
}
