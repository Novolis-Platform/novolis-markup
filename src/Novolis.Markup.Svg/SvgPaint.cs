namespace Novolis.Markup.Svg;

/// <summary>A validated SVG paint token.</summary>
public readonly record struct SvgPaint
{
    private readonly string? _value;

    private SvgPaint(string value)
    {
        _value = value;
    }

    /// <summary>No paint.</summary>
    public static SvgPaint None => new("none");

    /// <summary>The current CSS text color.</summary>
    public static SvgPaint CurrentColor => new("currentColor");

    /// <summary>Creates a paint from a CSS color token.</summary>
    public static SvgPaint Color(string value) => Css(value);

    /// <summary>Creates a paint from a CSS value such as <c>var(--accent)</c>.</summary>
    public static SvgPaint Css(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return new SvgPaint(value.Trim());
    }

    /// <summary>Creates a paint reference to an SVG definition.</summary>
    public static SvgPaint Url(string id)
    {
        return new SvgPaint($"url({new SvgReference(id)})");
    }

    /// <summary>Gets the serialized paint token.</summary>
    public string Value =>
        _value ?? throw new InvalidOperationException("The default SvgPaint value is not valid. Use a factory such as SvgPaint.Color.");

    /// <inheritdoc />
    public override string ToString() => _value ?? string.Empty;
}
