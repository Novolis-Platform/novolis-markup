namespace Novolis.Markup.Svg;

/// <summary>Units accepted by an SVG length.</summary>
public enum SvgLengthUnit
{
    /// <summary>A unitless SVG number.</summary>
    Number,

    /// <summary>CSS pixels.</summary>
    Pixels,

    /// <summary>A percentage.</summary>
    Percent,

    /// <summary>The current font size.</summary>
    Em,

    /// <summary>The root font size.</summary>
    Rem,

    /// <summary>The x-height of the current font.</summary>
    Ex,

    /// <summary>One percent of the viewport width.</summary>
    ViewportWidth,

    /// <summary>One percent of the viewport height.</summary>
    ViewportHeight,

    /// <summary>One percent of the smaller viewport dimension.</summary>
    ViewportMin,

    /// <summary>One percent of the larger viewport dimension.</summary>
    ViewportMax,
}
