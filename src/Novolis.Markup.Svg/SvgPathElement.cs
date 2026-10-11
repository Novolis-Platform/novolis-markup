namespace Novolis.Markup.Svg;

/// <summary>A typed SVG path element.</summary>
public sealed class SvgPathElement : SvgElement
{
    /// <summary>Creates a path from typed path data.</summary>
    public SvgPathElement(SvgPathData path)
        : base("path")
    {
        ArgumentNullException.ThrowIfNull(path);
        if (path.IsEmpty)
        {
            throw new ArgumentException("A path must contain at least one command.", nameof(path));
        }

        SetAttribute("d", path.ToString());
    }
}
