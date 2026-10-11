namespace Novolis.Markup.Svg;

/// <summary>A typed SVG use element.</summary>
public sealed class SvgUse : SvgElement
{
    /// <summary>Creates a reference to an element in the current document.</summary>
    public SvgUse(SvgReference reference)
        : base("use")
    {
        SetAttribute("href", reference.ToString());
    }
}
