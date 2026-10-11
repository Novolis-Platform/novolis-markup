namespace Novolis.Markup.Svg;

/// <summary>A two-dimensional SVG coordinate.</summary>
public readonly record struct SvgPoint
{
    /// <summary>Creates a point.</summary>
    public SvgPoint(double x, double y)
    {
        SvgNumber.EnsureFinite(x, nameof(x));
        SvgNumber.EnsureFinite(y, nameof(y));
        X = x;
        Y = y;
    }

    /// <summary>Horizontal coordinate.</summary>
    public double X { get; }

    /// <summary>Vertical coordinate.</summary>
    public double Y { get; }

    /// <inheritdoc />
    public override string ToString() => $"{SvgNumber.Format(X)},{SvgNumber.Format(Y)}";
}
