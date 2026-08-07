using System.Text;

namespace Novolis.Markup.Html;

/// <summary>A fluent HTML or XML element builder.</summary>
public class HtmlElement : IHtmlNode
{
    private static readonly HashSet<string> VoidTags = new(StringComparer.OrdinalIgnoreCase)
    {
        "area", "base", "br", "col", "embed", "hr", "img", "input", "link", "meta", "source", "track", "wbr",
    };

    private readonly List<(string Name, string? Value)> _attributes = new();
    private readonly List<IHtmlNode> _children = new();
    private readonly HashSet<string> _classes = new(StringComparer.Ordinal);

    /// <summary>Creates an element with the given tag name.</summary>
    public HtmlElement(string tagName, HtmlRenderKind renderKind = HtmlRenderKind.Html)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tagName);
        TagName = tagName;
        RenderKind = renderKind;
    }

    /// <summary>Tag name (e.g. <c>div</c>, <c>circle</c>).</summary>
    public string TagName { get; }

    /// <summary>Serialization mode.</summary>
    public HtmlRenderKind RenderKind { get; }

    /// <summary>True when this is an HTML void element.</summary>
    public bool IsVoid => RenderKind == HtmlRenderKind.Html && VoidTags.Contains(TagName);

    /// <summary>Child nodes.</summary>
    public IReadOnlyList<IHtmlNode> Children => _children;

    /// <summary>Sets or replaces an attribute. Pass <see langword="null"/> to emit a boolean attribute.</summary>
    public HtmlElement Attr(string name, string? value = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        for (var i = 0; i < _attributes.Count; i++)
        {
            if (string.Equals(_attributes[i].Name, name, StringComparison.OrdinalIgnoreCase))
            {
                _attributes[i] = (name, value);
                return this;
            }
        }

        _attributes.Add((name, value));
        return this;
    }

    /// <summary>Sets <c>id</c>.</summary>
    public HtmlElement Id(string id) => Attr("id", id);

    /// <summary>Adds one or more CSS classes.</summary>
    public HtmlElement Class(params string[] classes)
    {
        foreach (var cssClass in classes)
        {
            if (string.IsNullOrWhiteSpace(cssClass))
            {
                continue;
            }

            foreach (var part in cssClass.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                _classes.Add(part);
            }
        }

        return this;
    }

    /// <summary>Sets the inline <c>style</c> attribute.</summary>
    public HtmlElement Style(string css) => Attr("style", css);

    /// <summary>Sets inline style from a CSS rule builder (declarations only).</summary>
    public HtmlElement Style(Action<Css.CssRule> configure)
    {
        var rule = new Css.CssRule(string.Empty);
        configure(rule);
        return Style(rule.Declarations);
    }

    /// <summary>Sets <c>title</c>.</summary>
    public HtmlElement Title(string title) => Attr("title", title);

    /// <summary>Sets <c>type</c>.</summary>
    public HtmlElement Type(string type) => Attr("type", type);

    /// <summary>Sets <c>href</c>.</summary>
    public HtmlElement Href(string href) => Attr("href", href);

    /// <summary>Sets <c>src</c>.</summary>
    public HtmlElement Src(string src) => Attr("src", src);

    /// <summary>Sets <c>alt</c>.</summary>
    public HtmlElement Alt(string alt) => Attr("alt", alt);

    /// <summary>Sets <c>role</c>.</summary>
    public HtmlElement Role(string role) => Attr("role", role);

    /// <summary>Sets <c>name</c>.</summary>
    public HtmlElement Name(string name) => Attr("name", name);

    /// <summary>Sets <c>value</c>.</summary>
    public HtmlElement Value(string value) => Attr("value", value);

    /// <summary>Sets <c>placeholder</c>.</summary>
    public HtmlElement Placeholder(string placeholder) => Attr("placeholder", placeholder);

    /// <summary>Sets <c>target</c>.</summary>
    public HtmlElement Target(string target) => Attr("target", target);

    /// <summary>Sets <c>rel</c>.</summary>
    public HtmlElement Rel(string rel) => Attr("rel", rel);

    /// <summary>Sets <c>for</c>.</summary>
    public HtmlElement For(string forId) => Attr("for", forId);

    /// <summary>Sets <c>method</c>.</summary>
    public HtmlElement Method(string method) => Attr("method", method);

    /// <summary>Sets <c>action</c>.</summary>
    public HtmlElement Action(string action) => Attr("action", action);

    /// <summary>Sets <c>charset</c>.</summary>
    public HtmlElement Charset(string charset) => Attr("charset", charset);

    /// <summary>Sets <c>content</c>.</summary>
    public HtmlElement Content(string content) => Attr("content", content);

    /// <summary>Sets <c>http-equiv</c>.</summary>
    public HtmlElement HttpEquiv(string value) => Attr("http-equiv", value);

    /// <summary>Sets a <c>data-*</c> attribute.</summary>
    public HtmlElement Data(string name, string value) => Attr($"data-{name}", value);

    /// <summary>Sets <c>aria-*</c>.</summary>
    public HtmlElement Aria(string name, string value) => Attr($"aria-{name}", value);

    /// <summary>Sets a boolean attribute when <paramref name="enabled"/> is true.</summary>
    public HtmlElement Bool(string name, bool enabled = true) => enabled ? Attr(name) : this;

    /// <summary>Sets <c>disabled</c>.</summary>
    public HtmlElement Disabled(bool disabled = true) => Bool("disabled", disabled);

    /// <summary>Sets <c>checked</c>.</summary>
    public HtmlElement Checked(bool isChecked = true) => Bool("checked", isChecked);

    /// <summary>Sets <c>selected</c>.</summary>
    public HtmlElement Selected(bool selected = true) => Bool("selected", selected);

    /// <summary>Sets <c>required</c>.</summary>
    public HtmlElement Required(bool required = true) => Bool("required", required);

    /// <summary>Sets <c>readonly</c>.</summary>
    public HtmlElement ReadOnly(bool readOnly = true) => Bool("readonly", readOnly);

    /// <summary>Sets <c>hidden</c>.</summary>
    public HtmlElement Hidden(bool hidden = true) => Bool("hidden", hidden);

    /// <summary>Sets <c>defer</c>.</summary>
    public HtmlElement Defer(bool defer = true) => Bool("defer", defer);

    /// <summary>Sets <c>async</c>.</summary>
    public HtmlElement Async(bool async = true) => Bool("async", async);

    /// <summary>Appends a child node.</summary>
    public HtmlElement Child(IHtmlNode node)
    {
        ArgumentNullException.ThrowIfNull(node);
        if (IsVoid)
        {
            throw new InvalidOperationException($"Void element <{TagName}> cannot have children.");
        }

        _children.Add(node);
        return this;
    }

    /// <summary>Appends child nodes.</summary>
    public HtmlElement Add(params IHtmlNode[] nodes)
    {
        foreach (var node in nodes)
        {
            Child(node);
        }

        return this;
    }

    /// <summary>Appends escaped text.</summary>
    public HtmlElement Text(string text) => Child(new HtmlText(text));

    /// <summary>Appends unescaped markup.</summary>
    public HtmlElement Raw(string markup) => Child(new HtmlRaw(markup));

    /// <summary>Creates a child element, configures it, and returns this parent.</summary>
    public HtmlElement Element(string tagName, Action<HtmlElement>? configure = null)
    {
        var child = new HtmlElement(tagName, RenderKind);
        configure?.Invoke(child);
        return Child(child);
    }

    /// <summary>Creates a child element with text content.</summary>
    public HtmlElement Element(string tagName, string text)
    {
        var child = new HtmlElement(tagName, RenderKind).Text(text);
        return Child(child);
    }

    // —— Common HTML tags (add child, return parent) ——

    /// <summary>Appends a <c>div</c>.</summary>
    public HtmlElement Div(Action<HtmlElement>? configure = null) => Element("div", configure);

    /// <summary>Appends a <c>div</c> with text.</summary>
    public HtmlElement Div(string text) => Element("div", text);

    /// <summary>Appends a <c>span</c>.</summary>
    public HtmlElement Span(Action<HtmlElement>? configure = null) => Element("span", configure);

    /// <summary>Appends a <c>span</c> with text.</summary>
    public HtmlElement Span(string text) => Element("span", text);

    /// <summary>Appends a <c>p</c>.</summary>
    public HtmlElement P(Action<HtmlElement>? configure = null) => Element("p", configure);

    /// <summary>Appends a <c>p</c> with text.</summary>
    public HtmlElement P(string text) => Element("p", text);

    /// <summary>Appends an <c>a</c>.</summary>
    public HtmlElement A(Action<HtmlElement>? configure = null) => Element("a", configure);

    /// <summary>Appends an <c>a</c> with href and text.</summary>
    public HtmlElement A(string href, string text) => Element("a", a => a.Href(href).Text(text));

    /// <summary>Appends a heading.</summary>
    public HtmlElement H(int level, Action<HtmlElement>? configure = null)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(level, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(level, 6);
        return Element($"h{level}", configure);
    }

    /// <summary>Appends a heading with text.</summary>
    public HtmlElement H(int level, string text)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(level, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(level, 6);
        return Element($"h{level}", text);
    }

    /// <summary>Appends <c>h1</c>.</summary>
    public HtmlElement H1(Action<HtmlElement>? configure = null) => H(1, configure);

    /// <summary>Appends <c>h1</c> with text.</summary>
    public HtmlElement H1(string text) => H(1, text);

    /// <summary>Appends <c>h2</c>.</summary>
    public HtmlElement H2(Action<HtmlElement>? configure = null) => H(2, configure);

    /// <summary>Appends <c>h2</c> with text.</summary>
    public HtmlElement H2(string text) => H(2, text);

    /// <summary>Appends <c>h3</c>.</summary>
    public HtmlElement H3(Action<HtmlElement>? configure = null) => H(3, configure);

    /// <summary>Appends <c>h3</c> with text.</summary>
    public HtmlElement H3(string text) => H(3, text);

    /// <summary>Appends <c>h4</c>.</summary>
    public HtmlElement H4(Action<HtmlElement>? configure = null) => H(4, configure);

    /// <summary>Appends <c>h4</c> with text.</summary>
    public HtmlElement H4(string text) => H(4, text);

    /// <summary>Appends <c>h5</c>.</summary>
    public HtmlElement H5(Action<HtmlElement>? configure = null) => H(5, configure);

    /// <summary>Appends <c>h5</c> with text.</summary>
    public HtmlElement H5(string text) => H(5, text);

    /// <summary>Appends <c>h6</c>.</summary>
    public HtmlElement H6(Action<HtmlElement>? configure = null) => H(6, configure);

    /// <summary>Appends <c>h6</c> with text.</summary>
    public HtmlElement H6(string text) => H(6, text);

    /// <summary>Appends <c>ul</c>.</summary>
    public HtmlElement Ul(Action<HtmlElement>? configure = null) => Element("ul", configure);

    /// <summary>Appends <c>ol</c>.</summary>
    public HtmlElement Ol(Action<HtmlElement>? configure = null) => Element("ol", configure);

    /// <summary>Appends <c>li</c>.</summary>
    public HtmlElement Li(Action<HtmlElement>? configure = null) => Element("li", configure);

    /// <summary>Appends <c>li</c> with text.</summary>
    public HtmlElement Li(string text) => Element("li", text);

    /// <summary>Appends <c>section</c>.</summary>
    public HtmlElement Section(Action<HtmlElement>? configure = null) => Element("section", configure);

    /// <summary>Appends <c>article</c>.</summary>
    public HtmlElement Article(Action<HtmlElement>? configure = null) => Element("article", configure);

    /// <summary>Appends <c>header</c>.</summary>
    public HtmlElement Header(Action<HtmlElement>? configure = null) => Element("header", configure);

    /// <summary>Appends <c>footer</c>.</summary>
    public HtmlElement Footer(Action<HtmlElement>? configure = null) => Element("footer", configure);

    /// <summary>Appends <c>main</c>.</summary>
    public HtmlElement Main(Action<HtmlElement>? configure = null) => Element("main", configure);

    /// <summary>Appends <c>nav</c>.</summary>
    public HtmlElement Nav(Action<HtmlElement>? configure = null) => Element("nav", configure);

    /// <summary>Appends <c>aside</c>.</summary>
    public HtmlElement Aside(Action<HtmlElement>? configure = null) => Element("aside", configure);

    /// <summary>Appends <c>form</c>.</summary>
    public HtmlElement Form(Action<HtmlElement>? configure = null) => Element("form", configure);

    /// <summary>Appends <c>label</c>.</summary>
    public HtmlElement Label(Action<HtmlElement>? configure = null) => Element("label", configure);

    /// <summary>Appends <c>label</c> with text.</summary>
    public HtmlElement Label(string text) => Element("label", text);

    /// <summary>Appends <c>input</c>.</summary>
    public HtmlElement Input(Action<HtmlElement>? configure = null) => Element("input", configure);

    /// <summary>Appends <c>button</c>.</summary>
    public HtmlElement Button(Action<HtmlElement>? configure = null) => Element("button", configure);

    /// <summary>Appends <c>button</c> with text.</summary>
    public HtmlElement Button(string text) => Element("button", text);

    /// <summary>Appends <c>textarea</c>.</summary>
    public HtmlElement TextArea(Action<HtmlElement>? configure = null) => Element("textarea", configure);

    /// <summary>Appends <c>select</c>.</summary>
    public HtmlElement Select(Action<HtmlElement>? configure = null) => Element("select", configure);

    /// <summary>Appends <c>option</c>.</summary>
    public HtmlElement Option(Action<HtmlElement>? configure = null) => Element("option", configure);

    /// <summary>Appends <c>option</c> with text.</summary>
    public HtmlElement Option(string text) => Element("option", text);

    /// <summary>Appends <c>table</c>.</summary>
    public HtmlElement Table(Action<HtmlElement>? configure = null) => Element("table", configure);

    /// <summary>Appends <c>thead</c>.</summary>
    public HtmlElement Thead(Action<HtmlElement>? configure = null) => Element("thead", configure);

    /// <summary>Appends <c>tbody</c>.</summary>
    public HtmlElement Tbody(Action<HtmlElement>? configure = null) => Element("tbody", configure);

    /// <summary>Appends <c>tfoot</c>.</summary>
    public HtmlElement Tfoot(Action<HtmlElement>? configure = null) => Element("tfoot", configure);

    /// <summary>Appends <c>tr</c>.</summary>
    public HtmlElement Tr(Action<HtmlElement>? configure = null) => Element("tr", configure);

    /// <summary>Appends <c>th</c>.</summary>
    public HtmlElement Th(Action<HtmlElement>? configure = null) => Element("th", configure);

    /// <summary>Appends <c>th</c> with text.</summary>
    public HtmlElement Th(string text) => Element("th", text);

    /// <summary>Appends <c>td</c>.</summary>
    public HtmlElement Td(Action<HtmlElement>? configure = null) => Element("td", configure);

    /// <summary>Appends <c>td</c> with text.</summary>
    public HtmlElement Td(string text) => Element("td", text);

    /// <summary>Appends <c>img</c>.</summary>
    public HtmlElement Img(Action<HtmlElement>? configure = null) => Element("img", configure);

    /// <summary>Appends <c>img</c> with src and alt.</summary>
    public HtmlElement Img(string src, string alt) => Element("img", img => img.Src(src).Alt(alt));

    /// <summary>Appends <c>br</c>.</summary>
    public HtmlElement Br() => Element("br");

    /// <summary>Appends <c>hr</c>.</summary>
    public HtmlElement Hr() => Element("hr");

    /// <summary>Appends <c>pre</c>.</summary>
    public HtmlElement Pre(Action<HtmlElement>? configure = null) => Element("pre", configure);

    /// <summary>Appends <c>code</c>.</summary>
    public HtmlElement Code(Action<HtmlElement>? configure = null) => Element("code", configure);

    /// <summary>Appends <c>code</c> with text.</summary>
    public HtmlElement Code(string text) => Element("code", text);

    /// <summary>Appends <c>blockquote</c>.</summary>
    public HtmlElement Blockquote(Action<HtmlElement>? configure = null) => Element("blockquote", configure);

    /// <summary>Appends <c>strong</c>.</summary>
    public HtmlElement Strong(Action<HtmlElement>? configure = null) => Element("strong", configure);

    /// <summary>Appends <c>strong</c> with text.</summary>
    public HtmlElement Strong(string text) => Element("strong", text);

    /// <summary>Appends <c>em</c>.</summary>
    public HtmlElement Em(Action<HtmlElement>? configure = null) => Element("em", configure);

    /// <summary>Appends <c>em</c> with text.</summary>
    public HtmlElement Em(string text) => Element("em", text);

    /// <summary>Appends <c>small</c>.</summary>
    public HtmlElement Small(Action<HtmlElement>? configure = null) => Element("small", configure);

    /// <summary>Appends <c>small</c> with text.</summary>
    public HtmlElement Small(string text) => Element("small", text);

    /// <summary>Appends <c>figure</c>.</summary>
    public HtmlElement Figure(Action<HtmlElement>? configure = null) => Element("figure", configure);

    /// <summary>Appends <c>figcaption</c>.</summary>
    public HtmlElement Figcaption(Action<HtmlElement>? configure = null) => Element("figcaption", configure);

    /// <summary>Appends <c>figcaption</c> with text.</summary>
    public HtmlElement Figcaption(string text) => Element("figcaption", text);

    /// <summary>Appends a <c>style</c> element with a stylesheet.</summary>
    public HtmlElement StyleSheet(Action<Css.CssStylesheet> configure)
    {
        var sheet = new Css.CssStylesheet();
        configure(sheet);
        return Element("style", style => style.Raw(sheet.ToString()));
    }

    /// <summary>Appends a <c>style</c> element with CSS text.</summary>
    public HtmlElement StyleSheet(string css) => Element("style", style => style.Raw(css));

    /// <summary>Appends a <c>script</c> element from a JS builder.</summary>
    public HtmlElement Script(Action<Js.JsScript> configure)
    {
        var script = new Js.JsScript();
        configure(script);
        return Element("script", s => s.Raw(script.ToString()));
    }

    /// <summary>Appends a <c>script</c> element with source text.</summary>
    public HtmlElement Script(string javaScript) => Element("script", s => s.Raw(javaScript));

    /// <summary>Appends an external <c>script</c>.</summary>
    public HtmlElement ScriptSrc(string src, Action<HtmlElement>? configure = null) =>
        Element("script", s =>
        {
            s.Src(src);
            configure?.Invoke(s);
        });

    /// <summary>Appends a stylesheet <c>link</c>.</summary>
    public HtmlElement StylesheetLink(string href) =>
        Element("link", link => link.Rel("stylesheet").Href(href));

    /// <summary>Appends <c>meta</c>.</summary>
    public HtmlElement Meta(Action<HtmlElement>? configure = null) => Element("meta", configure);

    /// <summary>Appends <c>link</c>.</summary>
    public HtmlElement Link(Action<HtmlElement>? configure = null) => Element("link", configure);

    /// <summary>Appends <c>title</c> with text (document title).</summary>
    public HtmlElement DocTitle(string title) => Element("title", title);

    /// <summary>Appends an SVG root.</summary>
    public HtmlElement Svg(Action<Svg.SvgRoot>? configure = null)
    {
        var svg = new Svg.SvgRoot();
        configure?.Invoke(svg);
        return Child(svg);
    }

    /// <inheritdoc />
    public void WriteTo(TextWriter writer)
    {
        SyncClassAttribute();
        writer.Write('<');
        writer.Write(TagName);
        WriteAttributes(writer);

        if (IsVoid)
        {
            writer.Write('>');
            return;
        }

        if (_children.Count == 0 && RenderKind == HtmlRenderKind.Xml)
        {
            writer.Write(" />");
            return;
        }

        writer.Write('>');
        foreach (var child in _children)
        {
            child.WriteTo(writer);
        }

        writer.Write("</");
        writer.Write(TagName);
        writer.Write('>');
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var sb = new StringBuilder();
        using var writer = new StringWriter(sb);
        WriteTo(writer);
        return sb.ToString();
    }

    private void SyncClassAttribute()
    {
        if (_classes.Count == 0)
        {
            return;
        }

        Attr("class", string.Join(' ', _classes));
    }

    private void WriteAttributes(TextWriter writer)
    {
        foreach (var (name, value) in _attributes)
        {
            writer.Write(' ');
            writer.Write(name);
            if (value is null)
            {
                continue;
            }

            writer.Write("=\"");
            writer.Write(HtmlEscape.Attribute(value));
            writer.Write('"');
        }
    }
}
