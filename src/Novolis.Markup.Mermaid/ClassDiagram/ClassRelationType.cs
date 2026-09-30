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
