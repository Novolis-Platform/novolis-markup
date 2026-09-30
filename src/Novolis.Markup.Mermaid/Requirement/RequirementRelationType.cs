namespace Novolis.Markup.Mermaid;

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
