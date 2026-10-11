namespace Novolis.Markup.Svg;

/// <summary>A standalone SVG document builder.</summary>
public sealed class SvgDocument : SvgContainer<SvgDocument>
{
    /// <summary>The SVG XML namespace.</summary>
    public const string Namespace = "http://www.w3.org/2000/svg";

    /// <summary>Creates an SVG document.</summary>
    public SvgDocument()
        : base("svg")
    {
    }

    /// <summary>Sets the document width.</summary>
    public SvgDocument Width(SvgLength width)
    {
        width.EnsureNonNegative(nameof(width));
        SetAttribute("width", width.ToString());
        return this;
    }

    /// <summary>Sets a unitless document width.</summary>
    public SvgDocument Width(double width) => Width(SvgLength.Number(width));

    /// <summary>Sets the document height.</summary>
    public SvgDocument Height(SvgLength height)
    {
        height.EnsureNonNegative(nameof(height));
        SetAttribute("height", height.ToString());
        return this;
    }

    /// <summary>Sets a unitless document height.</summary>
    public SvgDocument Height(double height) => Height(SvgLength.Number(height));

    /// <summary>Sets the document viewBox.</summary>
    public SvgDocument ViewBox(SvgViewBox viewBox)
    {
        SetAttribute("viewBox", viewBox.Serialize());
        return this;
    }

    /// <summary>Sets the document viewBox from numeric bounds.</summary>
    public SvgDocument ViewBox(double minX, double minY, double width, double height) =>
        ViewBox(new SvgViewBox(minX, minY, width, height));

    /// <summary>Writes this document to a text writer without an XML declaration.</summary>
    public void WriteTo(TextWriter writer) => SvgSerialization.Write(this, writer);
}
