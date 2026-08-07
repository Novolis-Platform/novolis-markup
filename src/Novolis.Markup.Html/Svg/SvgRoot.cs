using System.Globalization;

namespace Novolis.Markup.Html.Svg;

/// <summary>Fluent SVG root (<c>&lt;svg xmlns=...&gt;</c>).</summary>
public sealed class SvgRoot : HtmlElement
{
    /// <summary>Creates an SVG root with the SVG namespace.</summary>
    public SvgRoot()
        : base("svg", HtmlRenderKind.Xml)
    {
        Attr("xmlns", "http://www.w3.org/2000/svg");
    }

    /// <summary>Sets <c>width</c>.</summary>
    public SvgRoot Width(double value) => SizeAttr("width", value);

    /// <summary>Sets <c>width</c>.</summary>
    public SvgRoot Width(string value)
    {
        Attr("width", value);
        return this;
    }

    /// <summary>Sets <c>height</c>.</summary>
    public SvgRoot Height(double value) => SizeAttr("height", value);

    /// <summary>Sets <c>height</c>.</summary>
    public SvgRoot Height(string value)
    {
        Attr("height", value);
        return this;
    }

    /// <summary>Sets <c>viewBox</c>.</summary>
    public SvgRoot ViewBox(double minX, double minY, double width, double height)
    {
        Attr("viewBox", string.Create(CultureInfo.InvariantCulture, $"{minX} {minY} {width} {height}"));
        return this;
    }

    /// <summary>Sets <c>viewBox</c> from a string.</summary>
    public SvgRoot ViewBox(string viewBox)
    {
        Attr("viewBox", viewBox);
        return this;
    }

    /// <summary>Sets <c>fill</c>.</summary>
    public SvgRoot Fill(string value)
    {
        Attr("fill", value);
        return this;
    }

    /// <summary>Sets <c>stroke</c>.</summary>
    public SvgRoot Stroke(string value)
    {
        Attr("stroke", value);
        return this;
    }

    /// <summary>Appends a configured SVG child and returns this root.</summary>
    public SvgRoot Add(string tagName, Action<HtmlElement>? configure = null)
    {
        Element(tagName, configure);
        return this;
    }

    /// <summary>Appends <c>g</c>.</summary>
    public SvgRoot G(Action<HtmlElement>? configure = null) => Add("g", configure);

    /// <summary>Appends <c>circle</c>.</summary>
    public SvgRoot Circle(double cx, double cy, double r, Action<HtmlElement>? configure = null) =>
        Add("circle", c =>
        {
            c.Attr("cx", N(cx)).Attr("cy", N(cy)).Attr("r", N(r));
            configure?.Invoke(c);
        });

    /// <summary>Appends <c>ellipse</c>.</summary>
    public SvgRoot Ellipse(double cx, double cy, double rx, double ry, Action<HtmlElement>? configure = null) =>
        Add("ellipse", e =>
        {
            e.Attr("cx", N(cx)).Attr("cy", N(cy)).Attr("rx", N(rx)).Attr("ry", N(ry));
            configure?.Invoke(e);
        });

    /// <summary>Appends <c>rect</c>.</summary>
    public SvgRoot Rect(double x, double y, double width, double height, Action<HtmlElement>? configure = null) =>
        Add("rect", r =>
        {
            r.Attr("x", N(x)).Attr("y", N(y)).Attr("width", N(width)).Attr("height", N(height));
            configure?.Invoke(r);
        });

    /// <summary>Appends <c>line</c>.</summary>
    public SvgRoot Line(double x1, double y1, double x2, double y2, Action<HtmlElement>? configure = null) =>
        Add("line", l =>
        {
            l.Attr("x1", N(x1)).Attr("y1", N(y1)).Attr("x2", N(x2)).Attr("y2", N(y2));
            configure?.Invoke(l);
        });

    /// <summary>Appends <c>path</c>.</summary>
    public SvgRoot Path(string d, Action<HtmlElement>? configure = null) =>
        Add("path", p =>
        {
            p.Attr("d", d);
            configure?.Invoke(p);
        });

    /// <summary>Appends <c>polyline</c>.</summary>
    public SvgRoot Polyline(string points, Action<HtmlElement>? configure = null) =>
        Add("polyline", p =>
        {
            p.Attr("points", points);
            configure?.Invoke(p);
        });

    /// <summary>Appends <c>polygon</c>.</summary>
    public SvgRoot Polygon(string points, Action<HtmlElement>? configure = null) =>
        Add("polygon", p =>
        {
            p.Attr("points", points);
            configure?.Invoke(p);
        });

    /// <summary>Appends <c>text</c>.</summary>
    public SvgRoot Text(double x, double y, string content, Action<HtmlElement>? configure = null) =>
        Add("text", t =>
        {
            t.Attr("x", N(x)).Attr("y", N(y)).Text(content);
            configure?.Invoke(t);
        });

    /// <summary>Appends <c>defs</c>.</summary>
    public SvgRoot Defs(Action<HtmlElement>? configure = null) => Add("defs", configure);

    /// <summary>Appends <c>use</c>.</summary>
    public SvgRoot Use(string href, Action<HtmlElement>? configure = null) =>
        Add("use", u =>
        {
            u.Attr("href", href);
            configure?.Invoke(u);
        });

    /// <summary>Appends <c>title</c>.</summary>
    public SvgRoot SvgTitle(string title) => Add("title", t => t.Text(title));

    /// <summary>Appends <c>desc</c>.</summary>
    public SvgRoot Desc(string description) => Add("desc", d => d.Text(description));

    /// <summary>Appends a linear gradient definition.</summary>
    public SvgRoot LinearGradient(string id, Action<HtmlElement>? configure = null) =>
        Add("linearGradient", g =>
        {
            g.Id(id);
            configure?.Invoke(g);
        });

    /// <summary>Appends a radial gradient definition.</summary>
    public SvgRoot RadialGradient(string id, Action<HtmlElement>? configure = null) =>
        Add("radialGradient", g =>
        {
            g.Id(id);
            configure?.Invoke(g);
        });

    /// <summary>Helper to append a gradient <c>stop</c> onto a parent gradient element.</summary>
    public static void Stop(HtmlElement gradient, string offset, string color, double? opacity = null)
    {
        gradient.Element("stop", s =>
        {
            s.Attr("offset", offset).Attr("stop-color", color);
            if (opacity is not null)
            {
                s.Attr("stop-opacity", N(opacity.Value));
            }
        });
    }

    private SvgRoot SizeAttr(string name, double value)
    {
        Attr(name, N(value));
        return this;
    }

    private static string N(double value) => value.ToString(CultureInfo.InvariantCulture);
}
