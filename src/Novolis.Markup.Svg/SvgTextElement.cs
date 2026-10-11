using System.Xml;

namespace Novolis.Markup.Svg;

/// <summary>A typed SVG text element.</summary>
public sealed class SvgTextElement : SvgElement
{
    private readonly string _content;

    /// <summary>Creates a text element.</summary>
    public SvgTextElement(SvgPoint origin, string content)
        : base("text")
    {
        ArgumentNullException.ThrowIfNull(content);
        _content = content;
        SetAttribute("x", SvgNumber.Format(origin.X));
        SetAttribute("y", SvgNumber.Format(origin.Y));
    }

    /// <inheritdoc />
    internal override void WriteContent(XmlWriter writer) => writer.WriteString(_content);
}
