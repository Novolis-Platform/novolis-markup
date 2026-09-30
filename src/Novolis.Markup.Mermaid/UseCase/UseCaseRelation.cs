namespace Novolis.Markup.Mermaid;

/// <summary>UML association operators for use case diagrams.</summary>
public enum UseCaseRelation
{
    /// <summary>Solid association <c>--&gt;</c>.</summary>
    Association,

    /// <summary>Include <c>..&gt; : include</c>.</summary>
    Include,

    /// <summary>Extend <c>..&gt; : extend</c>.</summary>
    Extend,

    /// <summary>Generalization <c>--|&gt;</c>.</summary>
    Generalization,
}
