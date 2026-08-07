namespace Novolis.Markup.Html;

/// <summary>Tag factories for common HTML elements.</summary>
public static partial class HtmlMarkup
{
    /// <summary>Creates a <c>div</c>.</summary>
    public static HtmlElement Div(Action<HtmlElement>? configure = null) => Element("div", configure);

    /// <summary>Creates a <c>div</c> with text.</summary>
    public static HtmlElement Div(string text) => Element("div", text);

    /// <summary>Creates a <c>span</c>.</summary>
    public static HtmlElement Span(Action<HtmlElement>? configure = null) => Element("span", configure);

    /// <summary>Creates a <c>span</c> with text.</summary>
    public static HtmlElement Span(string text) => Element("span", text);

    /// <summary>Creates a <c>p</c>.</summary>
    public static HtmlElement P(Action<HtmlElement>? configure = null) => Element("p", configure);

    /// <summary>Creates a <c>p</c> with text.</summary>
    public static HtmlElement P(string text) => Element("p", text);

    /// <summary>Creates an <c>a</c>.</summary>
    public static HtmlElement A(Action<HtmlElement>? configure = null) => Element("a", configure);

    /// <summary>Creates an <c>a</c> with href and text.</summary>
    public static HtmlElement A(string href, string text) => Element("a", a => a.Href(href).Text(text));

    /// <summary>Creates a heading (<c>h1</c>–<c>h6</c>).</summary>
    public static HtmlElement H(int level, Action<HtmlElement>? configure = null)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(level, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(level, 6);
        return Element($"h{level}", configure);
    }

    /// <summary>Creates a heading with text.</summary>
    public static HtmlElement H(int level, string text)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(level, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(level, 6);
        return Element($"h{level}", text);
    }

    /// <summary>Creates <c>h1</c>.</summary>
    public static HtmlElement H1(Action<HtmlElement>? configure = null) => H(1, configure);

    /// <summary>Creates <c>h1</c> with text.</summary>
    public static HtmlElement H1(string text) => H(1, text);

    /// <summary>Creates <c>h2</c>.</summary>
    public static HtmlElement H2(Action<HtmlElement>? configure = null) => H(2, configure);

    /// <summary>Creates <c>h2</c> with text.</summary>
    public static HtmlElement H2(string text) => H(2, text);

    /// <summary>Creates <c>h3</c>.</summary>
    public static HtmlElement H3(Action<HtmlElement>? configure = null) => H(3, configure);

    /// <summary>Creates <c>h3</c> with text.</summary>
    public static HtmlElement H3(string text) => H(3, text);

    /// <summary>Creates <c>h4</c>.</summary>
    public static HtmlElement H4(Action<HtmlElement>? configure = null) => H(4, configure);

    /// <summary>Creates <c>h4</c> with text.</summary>
    public static HtmlElement H4(string text) => H(4, text);

    /// <summary>Creates <c>h5</c>.</summary>
    public static HtmlElement H5(Action<HtmlElement>? configure = null) => H(5, configure);

    /// <summary>Creates <c>h5</c> with text.</summary>
    public static HtmlElement H5(string text) => H(5, text);

    /// <summary>Creates <c>h6</c>.</summary>
    public static HtmlElement H6(Action<HtmlElement>? configure = null) => H(6, configure);

    /// <summary>Creates <c>h6</c> with text.</summary>
    public static HtmlElement H6(string text) => H(6, text);

    /// <summary>Creates <c>ul</c>.</summary>
    public static HtmlElement Ul(Action<HtmlElement>? configure = null) => Element("ul", configure);

    /// <summary>Creates <c>ol</c>.</summary>
    public static HtmlElement Ol(Action<HtmlElement>? configure = null) => Element("ol", configure);

    /// <summary>Creates <c>li</c>.</summary>
    public static HtmlElement Li(Action<HtmlElement>? configure = null) => Element("li", configure);

    /// <summary>Creates <c>li</c> with text.</summary>
    public static HtmlElement Li(string text) => Element("li", text);

    /// <summary>Creates <c>section</c>.</summary>
    public static HtmlElement Section(Action<HtmlElement>? configure = null) => Element("section", configure);

    /// <summary>Creates <c>article</c>.</summary>
    public static HtmlElement Article(Action<HtmlElement>? configure = null) => Element("article", configure);

    /// <summary>Creates <c>header</c>.</summary>
    public static HtmlElement Header(Action<HtmlElement>? configure = null) => Element("header", configure);

    /// <summary>Creates <c>footer</c>.</summary>
    public static HtmlElement Footer(Action<HtmlElement>? configure = null) => Element("footer", configure);

    /// <summary>Creates <c>main</c>.</summary>
    public static HtmlElement Main(Action<HtmlElement>? configure = null) => Element("main", configure);

    /// <summary>Creates <c>nav</c>.</summary>
    public static HtmlElement Nav(Action<HtmlElement>? configure = null) => Element("nav", configure);

    /// <summary>Creates <c>aside</c>.</summary>
    public static HtmlElement Aside(Action<HtmlElement>? configure = null) => Element("aside", configure);

    /// <summary>Creates <c>form</c>.</summary>
    public static HtmlElement Form(Action<HtmlElement>? configure = null) => Element("form", configure);

    /// <summary>Creates <c>label</c>.</summary>
    public static HtmlElement Label(Action<HtmlElement>? configure = null) => Element("label", configure);

    /// <summary>Creates <c>label</c> with text.</summary>
    public static HtmlElement Label(string text) => Element("label", text);

    /// <summary>Creates <c>input</c>.</summary>
    public static HtmlElement Input(Action<HtmlElement>? configure = null) => Element("input", configure);

    /// <summary>Creates <c>button</c>.</summary>
    public static HtmlElement Button(Action<HtmlElement>? configure = null) => Element("button", configure);

    /// <summary>Creates <c>button</c> with text.</summary>
    public static HtmlElement Button(string text) => Element("button", text);

    /// <summary>Creates <c>textarea</c>.</summary>
    public static HtmlElement TextArea(Action<HtmlElement>? configure = null) => Element("textarea", configure);

    /// <summary>Creates <c>select</c>.</summary>
    public static HtmlElement Select(Action<HtmlElement>? configure = null) => Element("select", configure);

    /// <summary>Creates <c>option</c>.</summary>
    public static HtmlElement Option(Action<HtmlElement>? configure = null) => Element("option", configure);

    /// <summary>Creates <c>option</c> with text.</summary>
    public static HtmlElement Option(string text) => Element("option", text);

    /// <summary>Creates <c>table</c>.</summary>
    public static HtmlElement Table(Action<HtmlElement>? configure = null) => Element("table", configure);

    /// <summary>Creates <c>thead</c>.</summary>
    public static HtmlElement Thead(Action<HtmlElement>? configure = null) => Element("thead", configure);

    /// <summary>Creates <c>tbody</c>.</summary>
    public static HtmlElement Tbody(Action<HtmlElement>? configure = null) => Element("tbody", configure);

    /// <summary>Creates <c>tfoot</c>.</summary>
    public static HtmlElement Tfoot(Action<HtmlElement>? configure = null) => Element("tfoot", configure);

    /// <summary>Creates <c>tr</c>.</summary>
    public static HtmlElement Tr(Action<HtmlElement>? configure = null) => Element("tr", configure);

    /// <summary>Creates <c>th</c>.</summary>
    public static HtmlElement Th(Action<HtmlElement>? configure = null) => Element("th", configure);

    /// <summary>Creates <c>th</c> with text.</summary>
    public static HtmlElement Th(string text) => Element("th", text);

    /// <summary>Creates <c>td</c>.</summary>
    public static HtmlElement Td(Action<HtmlElement>? configure = null) => Element("td", configure);

    /// <summary>Creates <c>td</c> with text.</summary>
    public static HtmlElement Td(string text) => Element("td", text);

    /// <summary>Creates <c>img</c>.</summary>
    public static HtmlElement Img(Action<HtmlElement>? configure = null) => Element("img", configure);

    /// <summary>Creates <c>img</c> with src and alt.</summary>
    public static HtmlElement Img(string src, string alt) => Element("img", img => img.Src(src).Alt(alt));

    /// <summary>Creates <c>br</c>.</summary>
    public static HtmlElement Br() => Element("br");

    /// <summary>Creates <c>hr</c>.</summary>
    public static HtmlElement Hr() => Element("hr");

    /// <summary>Creates <c>pre</c>.</summary>
    public static HtmlElement Pre(Action<HtmlElement>? configure = null) => Element("pre", configure);

    /// <summary>Creates <c>code</c>.</summary>
    public static HtmlElement Code(Action<HtmlElement>? configure = null) => Element("code", configure);

    /// <summary>Creates <c>code</c> with text.</summary>
    public static HtmlElement Code(string text) => Element("code", text);

    /// <summary>Creates <c>blockquote</c>.</summary>
    public static HtmlElement Blockquote(Action<HtmlElement>? configure = null) => Element("blockquote", configure);

    /// <summary>Creates <c>blockquote</c> with text.</summary>
    public static HtmlElement Blockquote(string text) => Element("blockquote", text);

    /// <summary>Creates <c>strong</c>.</summary>
    public static HtmlElement Strong(Action<HtmlElement>? configure = null) => Element("strong", configure);

    /// <summary>Creates <c>strong</c> with text.</summary>
    public static HtmlElement Strong(string text) => Element("strong", text);

    /// <summary>Creates <c>em</c>.</summary>
    public static HtmlElement Em(Action<HtmlElement>? configure = null) => Element("em", configure);

    /// <summary>Creates <c>em</c> with text.</summary>
    public static HtmlElement Em(string text) => Element("em", text);

    /// <summary>Creates <c>del</c>.</summary>
    public static HtmlElement Del(Action<HtmlElement>? configure = null) => Element("del", configure);

    /// <summary>Creates <c>del</c> with text.</summary>
    public static HtmlElement Del(string text) => Element("del", text);

    /// <summary>Creates <c>u</c>.</summary>
    public static HtmlElement U(Action<HtmlElement>? configure = null) => Element("u", configure);

    /// <summary>Creates <c>u</c> with text.</summary>
    public static HtmlElement U(string text) => Element("u", text);

    /// <summary>Creates <c>small</c>.</summary>
    public static HtmlElement Small(Action<HtmlElement>? configure = null) => Element("small", configure);

    /// <summary>Creates <c>small</c> with text.</summary>
    public static HtmlElement Small(string text) => Element("small", text);

    /// <summary>Creates <c>figure</c>.</summary>
    public static HtmlElement Figure(Action<HtmlElement>? configure = null) => Element("figure", configure);

    /// <summary>Creates <c>figcaption</c>.</summary>
    public static HtmlElement Figcaption(Action<HtmlElement>? configure = null) => Element("figcaption", configure);

    /// <summary>Creates <c>figcaption</c> with text.</summary>
    public static HtmlElement Figcaption(string text) => Element("figcaption", text);

    /// <summary>Creates <c>meta</c>.</summary>
    public static HtmlElement Meta(Action<HtmlElement>? configure = null) => Element("meta", configure);

    /// <summary>Creates <c>link</c>.</summary>
    public static HtmlElement Link(Action<HtmlElement>? configure = null) => Element("link", configure);

    /// <summary>Creates <c>style</c> with CSS text.</summary>
    public static HtmlElement Style(string css) => Element("style", s => s.Raw(css));

    /// <summary>Creates <c>style</c> from a stylesheet builder.</summary>
    public static HtmlElement Style(Action<Css.CssStylesheet> configure)
    {
        var sheet = new Css.CssStylesheet();
        configure(sheet);
        return Style(sheet.ToString());
    }

    /// <summary>Creates <c>script</c> with JavaScript text.</summary>
    public static HtmlElement Script(string javaScript) => Element("script", s => s.Raw(javaScript));

    /// <summary>Creates <c>script</c> from a JS builder.</summary>
    public static HtmlElement Script(Action<Js.JsScript> configure)
    {
        var script = new Js.JsScript();
        configure(script);
        return Script(script.ToString());
    }

    /// <summary>Creates an external <c>script</c>.</summary>
    public static HtmlElement ScriptSrc(string src, Action<HtmlElement>? configure = null) =>
        Element("script", s =>
        {
            s.Src(src);
            configure?.Invoke(s);
        });

    /// <summary>Creates a stylesheet <c>link</c>.</summary>
    public static HtmlElement StylesheetLink(string href) =>
        Element("link", link => link.Rel("stylesheet").Href(href));
}
