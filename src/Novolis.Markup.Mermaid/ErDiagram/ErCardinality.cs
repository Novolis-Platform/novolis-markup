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
