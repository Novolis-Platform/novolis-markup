namespace Novolis.Markup.Html;

/// <summary>Static factories for HTML, CSS, JS, and SVG nodes.</summary>
public static partial class HtmlMarkup
{
    /// <summary>Creates a full HTML document.</summary>
    public static HtmlDocument Document(Action<HtmlDocument>? configure = null)
    {
        var document = new HtmlDocument();
        configure?.Invoke(document);
        return document;
    }

    /// <summary>Creates an unordered fragment of nodes.</summary>
    public static HtmlFragment Fragment(Action<HtmlFragment>? configure = null)
    {
        var fragment = new HtmlFragment();
        configure?.Invoke(fragment);
        return fragment;
    }

    /// <summary>Creates a fragment from nodes.</summary>
    public static HtmlFragment Fragment(params IHtmlNode[] nodes) => new HtmlFragment().Add(nodes);

    /// <summary>Creates an element.</summary>
    public static HtmlElement Element(string tagName, Action<HtmlElement>? configure = null)
    {
        var element = new HtmlElement(tagName);
        configure?.Invoke(element);
        return element;
    }

    /// <summary>Creates an element with text.</summary>
    public static HtmlElement Element(string tagName, string text) => new HtmlElement(tagName).Text(text);

    /// <summary>Escaped text node.</summary>
    public static HtmlText Text(string text) => new(text);

    /// <summary>Raw (unescaped) markup.</summary>
    public static HtmlRaw Raw(string markup) => new(markup);

    /// <summary>Creates a CSS stylesheet builder.</summary>
    public static Css.CssStylesheet Css(Action<Css.CssStylesheet>? configure = null)
    {
        var sheet = new Css.CssStylesheet();
        configure?.Invoke(sheet);
        return sheet;
    }

    /// <summary>Creates a JavaScript script builder (functions and classes).</summary>
    public static Js.JsScript Js(Action<Js.JsScript>? configure = null)
    {
        var script = new Js.JsScript();
        configure?.Invoke(script);
        return script;
    }

    /// <summary>Creates an SVG root.</summary>
    public static Svg.SvgRoot Svg(Action<Svg.SvgRoot>? configure = null)
    {
        var svg = new Svg.SvgRoot();
        configure?.Invoke(svg);
        return svg;
    }
}
