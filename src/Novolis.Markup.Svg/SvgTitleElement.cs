using System.Xml;

namespace Novolis.Markup.Svg;

/// <summary>A title element for an SVG document or group.</summary>
public sealed class SvgTitleElement : SvgElement
{
    private readonly string _content;

    /// <summary>Creates a title element.</summary>
    public SvgTitleElement(string content)
        : base("title")
    {
        ArgumentNullException.ThrowIfNull(content);
        _content = content;
    }

    /// <inheritdoc />
    internal override void WriteContent(XmlWriter writer) => writer.WriteString(_content);
}
