namespace Novolis.Markup.Mermaid;

/// <summary>An object that can emit Mermaid diagram source.</summary>
public interface IMermaidable
{
    /// <summary>Stable identifier for this diagram or node.</summary>
    Hash Id { get; }

    /// <summary>Returns Mermaid syntax for this object.</summary>
    IIndentedStringBuilder GetBuilder();
}
