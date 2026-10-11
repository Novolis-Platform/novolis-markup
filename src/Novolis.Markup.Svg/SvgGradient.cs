namespace Novolis.Markup.Svg;

/// <summary>Base class for typed SVG gradients.</summary>
public abstract class SvgGradient : SvgElement
{
    /// <summary>Creates a gradient.</summary>
    internal SvgGradient(string elementName, string id)
        : base(elementName)
    {
        SetAttribute("id", new SvgReference(id).Id);
    }

    /// <summary>Adds a gradient stop.</summary>
    public SvgGradient Stop(SvgLength offset, SvgPaint color, double? opacity = null)
    {
        var maximum = offset.Unit == SvgLengthUnit.Percent ? 100d : 1d;
        if (offset.Value < 0d || offset.Value > maximum)
        {
            throw new ArgumentOutOfRangeException(
                nameof(offset),
                offset,
                $"A {offset.Unit} gradient offset must be between 0 and {maximum}.");
        }

        if (opacity is not null)
        {
            SvgNumber.EnsureFinite(opacity.Value, nameof(opacity));
            if (opacity.Value is < 0d or > 1d)
            {
                throw new ArgumentOutOfRangeException(nameof(opacity), opacity, "Stop opacity must be between 0 and 1.");
            }
        }

        AddChild(new SvgGradientStop(offset, color, opacity));
        return this;
    }
}
