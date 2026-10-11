namespace Novolis.Markup.Svg;

/// <summary>A typed SVG group.</summary>
public sealed class SvgGroup : SvgContainer<SvgGroup>
{
    /// <summary>Creates an empty group.</summary>
    public SvgGroup()
        : base("g")
    {
    }
}
