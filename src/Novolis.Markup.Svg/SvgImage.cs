namespace Novolis.Markup.Svg;

/// <summary>A typed SVG image element.</summary>
public sealed class SvgImage : SvgElement
{
    /// <summary>Creates an image element.</summary>
    public SvgImage(SvgRect bounds, Uri href)
        : base("image")
    {
        ArgumentNullException.ThrowIfNull(href);
        SetAttribute("x", SvgNumber.Format(bounds.X));
        SetAttribute("y", SvgNumber.Format(bounds.Y));
        SetAttribute("width", SvgNumber.Format(bounds.Width));
        SetAttribute("height", SvgNumber.Format(bounds.Height));
        SetAttribute("href", href.ToString());
    }
}
