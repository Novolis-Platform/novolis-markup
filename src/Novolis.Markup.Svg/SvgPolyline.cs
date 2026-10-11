namespace Novolis.Markup.Svg;

/// <summary>A typed SVG polyline element.</summary>
public sealed class SvgPolyline : SvgElement
{
    /// <summary>Creates a polyline.</summary>
    public SvgPolyline(IEnumerable<SvgPoint> points)
        : base("polyline")
    {
        ArgumentNullException.ThrowIfNull(points);
        var values = points.ToArray();
        if (values.Length < 2)
        {
            throw new ArgumentException("A polyline requires at least two points.", nameof(points));
        }

        SetAttribute("points", string.Join(' ', values.Select(static point => point.ToString())));
    }
}
