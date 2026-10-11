namespace Novolis.Markup.Svg;

/// <summary>An SVG viewBox rectangle.</summary>
public readonly record struct SvgViewBox
{
    /// <summary>Creates a viewBox.</summary>
    public SvgViewBox(double minX, double minY, double width, double height)
    {
        SvgNumber.EnsureFinite(minX, nameof(minX));
        SvgNumber.EnsureFinite(minY, nameof(minY));
        if (width <= 0d || !double.IsFinite(width))
        {
            throw new ArgumentOutOfRangeException(nameof(width), width, "The viewBox width must be finite and positive.");
        }

        if (height <= 0d || !double.IsFinite(height))
        {
            throw new ArgumentOutOfRangeException(nameof(height), height, "The viewBox height must be finite and positive.");
        }

        MinX = minX;
        MinY = minY;
        Width = width;
        Height = height;
    }

    /// <summary>Minimum horizontal coordinate.</summary>
    public double MinX { get; }

    /// <summary>Minimum vertical coordinate.</summary>
    public double MinY { get; }

    /// <summary>ViewBox width.</summary>
    public double Width { get; }

    /// <summary>ViewBox height.</summary>
    public double Height { get; }

    internal string Serialize() =>
        $"{SvgNumber.Format(MinX)} {SvgNumber.Format(MinY)} {SvgNumber.Format(Width)} {SvgNumber.Format(Height)}";
}
