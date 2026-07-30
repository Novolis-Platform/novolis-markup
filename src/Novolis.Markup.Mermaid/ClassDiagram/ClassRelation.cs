namespace Novolis.Markup.Mermaid;

/// <summary>Relationship arrow styles for class diagrams.</summary>
public enum ClassRelationType
{
    /// <summary>Inheritance <c>&lt;|--</c>.</summary>
    Inheritance,

    /// <summary>Composition <c>*--</c>.</summary>
    Composition,

    /// <summary>Aggregation <c>o--</c>.</summary>
    Aggregation,

    /// <summary>Association <c>--&gt;</c>.</summary>
    Association,

    /// <summary>Link (solid) <c>--</c>.</summary>
    Link,

    /// <summary>Dependency <c>..&gt;</c>.</summary>
    Dependency,

    /// <summary>Realization <c>..|&gt;</c>.</summary>
    Realization,
}

/// <summary>A relationship between two classes.</summary>
public sealed class ClassRelation(string from, string to, ClassRelationType type, string? label = null) : IMermaidable
{
    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>Source class name.</summary>
    public string From { get; } = from;

    /// <summary>Target class name.</summary>
    public string To { get; } = to;

    /// <summary>Relation arrow type.</summary>
    public ClassRelationType Type { get; } = type;

    /// <summary>Optional label.</summary>
    public string? Label { get; } = label;

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var token = Type switch
        {
            ClassRelationType.Inheritance => "<|--",
            ClassRelationType.Composition => "*--",
            ClassRelationType.Aggregation => "o--",
            ClassRelationType.Association => "-->",
            ClassRelationType.Link => "--",
            ClassRelationType.Dependency => "..>",
            ClassRelationType.Realization => "..|>",
            _ => "-->",
        };

        var writer = new IndentedStringBuilder();
        if (string.IsNullOrWhiteSpace(Label))
            writer.WriteLine("{0} {1} {2}", From, token, To);
        else
            writer.WriteLine("{0} {1} {2} : {3}", From, token, To, Label);
        return writer;
    }
}
