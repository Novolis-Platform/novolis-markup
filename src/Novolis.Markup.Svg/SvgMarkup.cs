namespace Novolis.Markup.Svg;

/// <summary>Static factories for typed SVG documents.</summary>
public static class SvgMarkup
{
    /// <summary>Creates and optionally configures an SVG document.</summary>
    public static SvgDocument Document(Action<SvgDocument>? configure = null)
    {
        var document = new SvgDocument();
        configure?.Invoke(document);
        return document;
    }
}
