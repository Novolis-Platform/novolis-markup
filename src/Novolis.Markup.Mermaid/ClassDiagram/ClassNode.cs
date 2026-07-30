namespace Novolis.Markup.Mermaid;

/// <summary>A class box in a class diagram.</summary>
public sealed class ClassNode(string name) : IMermaidable
{
    private readonly List<string> _members = [];
    private string? _stereotype;

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>Class name.</summary>
    public string Name { get; } = name;

    /// <summary>Sets an optional stereotype (emitted as <c>&lt;&lt;name&gt;&gt;</c>).</summary>
    public ClassNode WithStereotype(string stereotype)
    {
        _stereotype = stereotype;
        return this;
    }

    /// <summary>Adds a field or method line (include visibility prefix if desired).</summary>
    public ClassNode AddMember(string member)
    {
        _members.Add(member);
        return this;
    }

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        if (_members.Count == 0 && string.IsNullOrWhiteSpace(_stereotype))
        {
            writer.WriteLine("class {0}", Name);
            return writer;
        }

        writer.WriteLine("class {0} {{", Name);
        writer.IncreaseIndent();
        if (!string.IsNullOrWhiteSpace(_stereotype))
            writer.WriteLine("<<{0}>>", _stereotype);
        foreach (var member in _members)
            writer.WriteLine(member);
        writer.DecreaseIndent();
        writer.WriteLine("}");
        return writer;
    }
}
