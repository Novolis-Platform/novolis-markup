namespace Novolis.Markup.Svg;

/// <summary>A validated SVG length with an explicit unit.</summary>
public readonly record struct SvgLength
{
    /// <summary>Creates a length.</summary>
    public SvgLength(double value, SvgLengthUnit unit = SvgLengthUnit.Number)
    {
        SvgNumber.EnsureFinite(value, nameof(value));
        Value = value;
        Unit = unit;
    }

    /// <summary>Numeric part of the length.</summary>
    public double Value { get; }

    /// <summary>Unit applied to <see cref="Value"/>.</summary>
    public SvgLengthUnit Unit { get; }

    /// <summary>Creates a unitless SVG number.</summary>
    public static SvgLength Number(double value) => new(value, SvgLengthUnit.Number);

    /// <summary>Creates a pixel length.</summary>
    public static SvgLength Pixels(double value) => new(value, SvgLengthUnit.Pixels);

    /// <summary>Creates a percentage length.</summary>
    public static SvgLength Percent(double value) => new(value, SvgLengthUnit.Percent);

    /// <summary>Creates an <c>em</c> length.</summary>
    public static SvgLength Em(double value) => new(value, SvgLengthUnit.Em);

    /// <summary>Creates a <c>rem</c> length.</summary>
    public static SvgLength Rem(double value) => new(value, SvgLengthUnit.Rem);

    /// <summary>Creates an <c>ex</c> length.</summary>
    public static SvgLength Ex(double value) => new(value, SvgLengthUnit.Ex);

    /// <summary>Creates a viewport-width length.</summary>
    public static SvgLength ViewportWidth(double value) => new(value, SvgLengthUnit.ViewportWidth);

    /// <summary>Creates a viewport-height length.</summary>
    public static SvgLength ViewportHeight(double value) => new(value, SvgLengthUnit.ViewportHeight);

    /// <summary>Creates a viewport-minimum length.</summary>
    public static SvgLength ViewportMin(double value) => new(value, SvgLengthUnit.ViewportMin);

    /// <summary>Creates a viewport-maximum length.</summary>
    public static SvgLength ViewportMax(double value) => new(value, SvgLengthUnit.ViewportMax);

    /// <summary>Converts a number to a unitless SVG length.</summary>
    public static implicit operator SvgLength(double value) => Number(value);

    internal void EnsureNonNegative(string parameterName)
    {
        if (Value < 0d)
        {
            throw new ArgumentOutOfRangeException(parameterName, Value, "This SVG length must not be negative.");
        }
    }

    /// <inheritdoc />
    public override string ToString() =>
        SvgNumber.Format(Value) + (Unit switch
        {
            SvgLengthUnit.Number => string.Empty,
            SvgLengthUnit.Pixels => "px",
            SvgLengthUnit.Percent => "%",
            SvgLengthUnit.Em => "em",
            SvgLengthUnit.Rem => "rem",
            SvgLengthUnit.Ex => "ex",
            SvgLengthUnit.ViewportWidth => "vw",
            SvgLengthUnit.ViewportHeight => "vh",
            SvgLengthUnit.ViewportMin => "vmin",
            SvgLengthUnit.ViewportMax => "vmax",
            _ => throw new ArgumentOutOfRangeException(),
        });
}
