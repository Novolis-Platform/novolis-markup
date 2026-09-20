namespace Novolis.Markup.Mermaid;

/// <summary>Common ER cardinality tokens.</summary>
public static class ErCardinality
{
    /// <summary>Exactly one.</summary>
    public const string ExactlyOne = "||";

    /// <summary>Zero or one.</summary>
    public const string ZeroOrOne = "|o";

    /// <summary>One or more.</summary>
    public const string OneOrMore = "}|";

    /// <summary>Zero or more.</summary>
    public const string ZeroOrMore = "}o";
}

/// <summary>An ER relationship between two entities.</summary>
public sealed class ErRelationship(
    string from,
    string fromCardinality,
    string toCardinality,
    string to,
    string label,
    bool identifying = true) : IMermaidable
{
    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>When true, emits a solid identifying relationship (<c>--</c>); otherwise <c>..</c>.</summary>
    public bool Identifying { get; } = identifying;

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var connector = Identifying ? "--" : "..";
        var writer = new IndentedStringBuilder();
        writer.WriteLine("{0} {1}{2}{3} {4} : {5}", from, fromCardinality, connector, toCardinality, to, label);
        return writer;
    }
}
