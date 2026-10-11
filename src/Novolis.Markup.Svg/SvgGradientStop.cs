namespace Novolis.Markup.Svg;

internal sealed class SvgGradientStop : SvgElement
{
    public SvgGradientStop(SvgLength offset, SvgPaint color, double? opacity)
        : base("stop")
    {
        SetAttribute("offset", offset.ToString());
        SetAttribute("stop-color", color.Value);
        if (opacity is not null)
        {
            SetAttribute("stop-opacity", SvgNumber.Format(opacity.Value));
        }
    }
}
