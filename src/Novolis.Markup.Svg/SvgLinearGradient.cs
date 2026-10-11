namespace Novolis.Markup.Svg;

/// <summary>A typed linear gradient definition.</summary>
public sealed class SvgLinearGradient : SvgGradient
{
    /// <summary>Creates a linear gradient.</summary>
    public SvgLinearGradient(string id)
        : base("linearGradient", id)
    {
    }

    /// <summary>Sets the start x coordinate.</summary>
    public SvgLinearGradient X1(SvgLength value)
    {
        SetAttribute("x1", value.ToString());
        return this;
    }

    /// <summary>Sets the start y coordinate.</summary>
    public SvgLinearGradient Y1(SvgLength value)
    {
        SetAttribute("y1", value.ToString());
        return this;
    }

    /// <summary>Sets the end x coordinate.</summary>
    public SvgLinearGradient X2(SvgLength value)
    {
        SetAttribute("x2", value.ToString());
        return this;
    }

    /// <summary>Sets the end y coordinate.</summary>
    public SvgLinearGradient Y2(SvgLength value)
    {
        SetAttribute("y2", value.ToString());
        return this;
    }
}
