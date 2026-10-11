namespace Novolis.Markup.Svg;

/// <summary>A typed SVG line element.</summary>
public sealed class SvgLine : SvgElement
{
    /// <summary>Creates a line.</summary>
    public SvgLine(SvgPoint start, SvgPoint end)
        : base("line")
    {
        SetAttribute("x1", SvgNumber.Format(start.X));
        SetAttribute("y1", SvgNumber.Format(start.Y));
        SetAttribute("x2", SvgNumber.Format(end.X));
        SetAttribute("y2", SvgNumber.Format(end.Y));
    }
}
