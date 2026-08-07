namespace Novolis.Markup.Html;

/// <summary>Controls empty-element serialization.</summary>
public enum HtmlRenderKind
{
    /// <summary>HTML5: void tags omit end tags; other empty tags still write open+close.</summary>
    Html,

    /// <summary>XML/SVG: empty elements self-close (<c>&lt;circle /&gt;</c>).</summary>
    Xml,
}
