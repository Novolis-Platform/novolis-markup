using System.Text;

namespace Novolis.Markup.Html;

/// <summary>Fluent HTML5 document (<c>&lt;!DOCTYPE html&gt;</c> + <c>html</c>/<c>head</c>/<c>body</c>).</summary>
public sealed class HtmlDocument : IHtmlNode
{
    /// <summary>Creates an empty document with head and body.</summary>
    public HtmlDocument()
    {
        HtmlRoot = new HtmlElement("html");
        Head = new HtmlElement("head");
        Body = new HtmlElement("body");
        HtmlRoot.Child(Head).Child(Body);
    }

    /// <summary>Root <c>html</c> element.</summary>
    public HtmlElement HtmlRoot { get; }

    /// <summary>Document <c>head</c>.</summary>
    public HtmlElement Head { get; }

    /// <summary>Document <c>body</c>.</summary>
    public HtmlElement Body { get; }

    /// <summary>Sets <c>lang</c> on the root element.</summary>
    public HtmlDocument Lang(string lang)
    {
        HtmlRoot.Attr("lang", lang);
        return this;
    }

    /// <summary>Configures <c>head</c>.</summary>
    public HtmlDocument WithHead(Action<HtmlElement> configure)
    {
        configure(Head);
        return this;
    }

    /// <summary>Configures <c>body</c>.</summary>
    public HtmlDocument WithBody(Action<HtmlElement> configure)
    {
        configure(Body);
        return this;
    }

    /// <summary>Sets the document title.</summary>
    public HtmlDocument Title(string title)
    {
        Head.DocTitle(title);
        return this;
    }

    /// <summary>Adds UTF-8 charset meta.</summary>
    public HtmlDocument CharsetUtf8()
    {
        Head.Meta(m => m.Charset("utf-8"));
        return this;
    }

    /// <summary>Adds a responsive viewport meta.</summary>
    public HtmlDocument Viewport()
    {
        Head.Meta(m => m.Attr("name", "viewport").Content("width=device-width, initial-scale=1"));
        return this;
    }

    /// <summary>Adds a stylesheet to head.</summary>
    public HtmlDocument StyleSheet(Action<Css.CssStylesheet> configure)
    {
        Head.StyleSheet(configure);
        return this;
    }

    /// <summary>Adds a script to the end of body.</summary>
    public HtmlDocument Script(Action<Js.JsScript> configure)
    {
        Body.Script(configure);
        return this;
    }

    /// <inheritdoc />
    public void WriteTo(TextWriter writer)
    {
        writer.WriteLine("<!DOCTYPE html>");
        HtmlRoot.WriteTo(writer);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var sb = new StringBuilder();
        using var writer = new StringWriter(sb);
        WriteTo(writer);
        return sb.ToString();
    }
}
