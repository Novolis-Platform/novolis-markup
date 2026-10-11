namespace Novolis.Markup.Svg;

/// <summary>A typed SVG polygon element.</summary>
public sealed class SvgPolygon : SvgElement
{
    /// <summary>Creates a polygon.</summary>
    public SvgPolygon(IEnumerable<SvgPoint> points)
        : base("polygon")
    {
        ArgumentNullException.ThrowIfNull(points);
        var values = points.ToArray();
        if (values.Length < 3)
        {
            throw new ArgumentException("A polygon requires at least three points.", nameof(points));
        }

        SetAttribute("points", string.Join(' ', values.Select(static point => point.ToString())));
    }
}
