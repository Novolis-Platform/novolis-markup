namespace Novolis.Markup.Svg;

/// <summary>A rectangle expressed in SVG user-space units.</summary>
public readonly record struct SvgRect
{
    /// <summary>Creates a rectangle.</summary>
    public SvgRect(double x, double y, double width, double height)
    {
        SvgNumber.EnsureFinite(x, nameof(x));
        SvgNumber.EnsureFinite(y, nameof(y));
        SvgNumber.EnsureNonNegative(width, nameof(width));
        SvgNumber.EnsureNonNegative(height, nameof(height));
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    /// <summary>Horizontal coordinate.</summary>
    public double X { get; }

    /// <summary>Vertical coordinate.</summary>
    public double Y { get; }

    /// <summary>Width.</summary>
    public double Width { get; }

    /// <summary>Height.</summary>
    public double Height { get; }
}
