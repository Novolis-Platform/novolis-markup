namespace Novolis.Markup.Svg;

/// <summary>A typed SVG rectangle element.</summary>
public sealed class SvgRectangle : SvgElement
{
    /// <summary>Creates a rectangle.</summary>
    public SvgRectangle(SvgRect bounds)
        : base("rect")
    {
        SetAttribute("x", SvgNumber.Format(bounds.X));
        SetAttribute("y", SvgNumber.Format(bounds.Y));
        SetAttribute("width", SvgNumber.Format(bounds.Width));
        SetAttribute("height", SvgNumber.Format(bounds.Height));
    }

    /// <summary>Sets the horizontal corner radius.</summary>
    public SvgRectangle CornerRadius(double radius)
    {
        SvgNumber.EnsureNonNegative(radius, nameof(radius));
        return (SvgRectangle)SetAttribute("rx", SvgNumber.Format(radius));
    }

    /// <summary>Sets the vertical corner radius.</summary>
    public SvgRectangle CornerRadius(double radiusX, double radiusY)
    {
        SvgNumber.EnsureNonNegative(radiusX, nameof(radiusX));
        SvgNumber.EnsureNonNegative(radiusY, nameof(radiusY));
        SetAttribute("rx", SvgNumber.Format(radiusX));
        SetAttribute("ry", SvgNumber.Format(radiusY));
        return this;
    }
}
