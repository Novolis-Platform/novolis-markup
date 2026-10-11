using System.Xml;

namespace Novolis.Markup.Svg;

/// <summary>A description element for an SVG document or group.</summary>
public sealed class SvgDescriptionElement : SvgElement
{
    private readonly string _content;

    /// <summary>Creates a description element.</summary>
    public SvgDescriptionElement(string content)
        : base("desc")
    {
        ArgumentNullException.ThrowIfNull(content);
        _content = content;
    }

    /// <inheritdoc />
    internal override void WriteContent(XmlWriter writer) => writer.WriteString(_content);
}
