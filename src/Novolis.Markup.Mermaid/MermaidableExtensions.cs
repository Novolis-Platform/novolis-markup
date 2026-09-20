namespace Novolis.Markup.Mermaid;

/// <summary>Convenience helpers for <see cref="IMermaidable"/> builders.</summary>
public static class MermaidableExtensions
{
    /// <summary>Returns the unique identifier of the object as a string without dashes.</summary>
    public static string GetId(this IMermaidable source) => source.Id.ToString();

    /// <summary>Returns Mermaid source ready to paste into a renderer.</summary>
    public static string GetMermaidString(this IMermaidable source) => source.GetBuilder().ToString() ?? string.Empty;

    /// <summary>Wraps the builder as a <see cref="MermaidDocument"/> (no front matter).</summary>
    public static MermaidDocument ToDocument(this IMermaidable source, MermaidFrontMatter? frontMatter = null) =>
        MermaidDocument.From(source, frontMatter);
}
