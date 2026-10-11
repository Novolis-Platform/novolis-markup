namespace Novolis.Markup.Svg;

/// <summary>A typed SVG circle element.</summary>
public sealed class SvgCircle : SvgElement
{
    /// <summary>Creates a circle.</summary>
    public SvgCircle(SvgPoint center, double radius)
        : base("circle")
    {
        SvgNumber.EnsureNonNegative(radius, nameof(radius));
        SetAttribute("cx", SvgNumber.Format(center.X));
        SetAttribute("cy", SvgNumber.Format(center.Y));
        SetAttribute("r", SvgNumber.Format(radius));
    }
}
