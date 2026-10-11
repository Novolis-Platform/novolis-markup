namespace Novolis.Markup.Svg;

/// <summary>A typed radial gradient definition.</summary>
public sealed class SvgRadialGradient : SvgGradient
{
    /// <summary>Creates a radial gradient.</summary>
    public SvgRadialGradient(string id)
        : base("radialGradient", id)
    {
    }

    /// <summary>Sets the center x coordinate.</summary>
    public SvgRadialGradient Cx(SvgLength value)
    {
        SetAttribute("cx", value.ToString());
        return this;
    }

    /// <summary>Sets the center y coordinate.</summary>
    public SvgRadialGradient Cy(SvgLength value)
    {
        SetAttribute("cy", value.ToString());
        return this;
    }

    /// <summary>Sets the radius.</summary>
    public SvgRadialGradient Radius(SvgLength value)
    {
        SetAttribute("r", value.ToString());
        return this;
    }

    /// <summary>Sets the focal point x coordinate.</summary>
    public SvgRadialGradient Fx(SvgLength value)
    {
        SetAttribute("fx", value.ToString());
        return this;
    }

    /// <summary>Sets the focal point y coordinate.</summary>
    public SvgRadialGradient Fy(SvgLength value)
    {
        SetAttribute("fy", value.ToString());
        return this;
    }
}
