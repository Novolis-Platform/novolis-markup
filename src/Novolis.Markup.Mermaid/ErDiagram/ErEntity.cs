namespace Novolis.Markup.Mermaid;

/// <summary>An ER entity with typed attributes.</summary>
public sealed class ErEntity(string name) : IMermaidable
{
    private readonly List<string> _attributes = [];

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>Entity name.</summary>
    public string Name { get; } = name;

    /// <summary>Adds an attribute line (e.g. <c>string name PK</c>).</summary>
    public ErEntity AddAttribute(string attribute)
    {
        _attributes.Add(attribute);
        return this;
    }

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        if (_attributes.Count == 0)
        {
            writer.WriteLine(Name);
            return writer;
        }

        writer.WriteLine("{0} {{", Name);
        writer.IncreaseIndent();
        foreach (var attr in _attributes)
            writer.WriteLine(attr);
        writer.DecreaseIndent();
        writer.WriteLine("}");
        return writer;
    }
}
