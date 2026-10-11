namespace Novolis.Markup.Svg;

/// <summary>A typed SVG definitions container.</summary>
public sealed class SvgDefinitions : SvgContainer<SvgDefinitions>
{
    /// <summary>Creates an empty definitions container.</summary>
    public SvgDefinitions()
        : base("defs")
    {
    }
}
