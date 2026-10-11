namespace Novolis.Markup.Svg;

/// <summary>A typed SVG ellipse element.</summary>
public sealed class SvgEllipse : SvgElement
{
    /// <summary>Creates an ellipse.</summary>
    public SvgEllipse(SvgPoint center, double radiusX, double radiusY)
        : base("ellipse")
    {
        SvgNumber.EnsureNonNegative(radiusX, nameof(radiusX));
        SvgNumber.EnsureNonNegative(radiusY, nameof(radiusY));
        SetAttribute("cx", SvgNumber.Format(center.X));
        SetAttribute("cy", SvgNumber.Format(center.Y));
        SetAttribute("rx", SvgNumber.Format(radiusX));
        SetAttribute("ry", SvgNumber.Format(radiusY));
    }
}
