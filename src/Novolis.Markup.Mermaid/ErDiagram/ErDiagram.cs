namespace Novolis.Markup.Mermaid;

/// <summary>Fluent builder for Mermaid <c>erDiagram</c> syntax.</summary>
public sealed class ErDiagram : IMermaidable
{
    private readonly List<ErEntity> _entities = [];
    private readonly List<ErRelationship> _relationships = [];

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>Adds an entity definition.</summary>
    public ErDiagram AddEntity(ErEntity entity)
    {
        _entities.Add(entity);
        return this;
    }

    /// <summary>Adds a relationship.</summary>
    public ErDiagram AddRelationship(ErRelationship relationship)
    {
        _relationships.Add(relationship);
        return this;
    }

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine("erDiagram");
        writer.IncreaseIndent();
        foreach (var rel in _relationships)
            writer.WriteLine(rel.GetBuilder());
        foreach (var entity in _entities)
            writer.WriteLine(entity.GetBuilder());
        writer.DecreaseIndent();
        return writer;
    }
}
