using Novolis.Markup.Html;
using Novolis.Markup.Html.Css;
using Novolis.Markup.Html.Js;
using Novolis.Markup.Html.Svg;

namespace Novolis.Markup.Unit.Html;

public class HtmlDocumentTests
{
    [Test]
    public async Task Document_EmitsDoctypeHeadAndBody()
    {
        var html = HtmlMarkup.Document(doc => doc
            .Lang("en")
            .CharsetUtf8()
            .Viewport()
            .Title("Hi")
            .WithBody(body => body.H1("Hello")))
            .ToString();

        await Assert.That(html).Contains("<!DOCTYPE html>");
        await Assert.That(html).Contains("<html lang=\"en\">");
        await Assert.That(html).Contains("<title>Hi</title>");
        await Assert.That(html).Contains("<h1>Hello</h1>");
        await Assert.That(html).Contains("<meta charset=\"utf-8\">");
        await Assert.That(html).Contains("viewport");
    }

    [Test]
    public async Task TextAndAttributes_AreEscaped()
    {
        var html = HtmlMarkup.P("<b>&</b>").Attr("title", "a\"b").ToString();

        await Assert.That(html).Contains("&lt;b&gt;&amp;&lt;/b&gt;");
        await Assert.That(html).Contains("title=\"a&quot;b\"");
        await Assert.That(html).DoesNotContain("<b>");
    }

    [Test]
    public async Task VoidElements_HaveNoClosingTag()
    {
        var html = HtmlMarkup.Img("a.png", "Alt").ToString();

        await Assert.That(html).IsEqualTo("<img src=\"a.png\" alt=\"Alt\">");
    }

    [Test]
    public async Task Class_MergesTokens()
    {
        var html = HtmlMarkup.Div(d => d.Class("a", "b c").Text("x")).ToString();

        await Assert.That(html).Contains("class=\"a b c\"");
        await Assert.That(html).Contains(">x</div>");
    }

    [Test]
    public async Task BooleanAttributes_EmitWithoutValue()
    {
        var html = HtmlMarkup.Button(b => b.Text("Go").Disabled().Attr("data-x", "1")).ToString();

        await Assert.That(html).Contains(" disabled");
        await Assert.That(html).Contains("data-x=\"1\"");
        await Assert.That(html).Contains(">Go</button>");
    }

    [Test]
    public async Task VoidElement_RejectsChildren()
    {
        var threw = false;
        try
        {
            HtmlMarkup.Img(img => img.Src("x.png").Child(HtmlMarkup.Text("nope")));
        }
        catch (InvalidOperationException)
        {
            threw = true;
        }

        await Assert.That(threw).IsTrue();
    }

    [Test]
    public async Task NestedStructure_AndRawMarkup()
    {
        var html = HtmlMarkup.Section(s => s
            .Id("main")
            .Header(h => h.H1("Title"))
            .Ul(ul => ul.Li("One").Li("Two"))
            .Raw("<!-- note -->")
            .Add(HtmlMarkup.Span("extra"), HtmlMarkup.A("/x", "link")))
            .ToString();

        await Assert.That(html).Contains("id=\"main\"");
        await Assert.That(html).Contains("<li>One</li>");
        await Assert.That(html).Contains("<!-- note -->");
        await Assert.That(html).Contains("<a href=\"/x\">link</a>");
        await Assert.That(html).Contains("<span>extra</span>");
    }

    [Test]
    public async Task Escape_Helpers_HandleNullAndSpecialChars()
    {
        await Assert.That(HtmlEscape.Text(null)).IsEqualTo("");
        await Assert.That(HtmlEscape.Attribute(null)).IsEqualTo("");
        await Assert.That(HtmlEscape.Text("a<b>")).Contains("&lt;");
        await Assert.That(HtmlEscape.Attribute("a\"b&c")).Contains("&quot;");
        await Assert.That(HtmlEscape.Attribute("a\"b&c")).Contains("&amp;");
        await Assert.That(HtmlEscape.Attribute("x\0y")).IsEqualTo("xy");
    }

    [Test]
    public async Task Factories_CoverCommonTags()
    {
        await Assert.That(HtmlMarkup.H(2, "Sub").ToString()).IsEqualTo("<h2>Sub</h2>");
        await Assert.That(HtmlMarkup.H3("T").ToString()).Contains("<h3>");
        await Assert.That(HtmlMarkup.Nav(n => n.A("/", "Home")).ToString()).Contains("<nav>");
        await Assert.That(HtmlMarkup.Table(t => t.Tr(tr => tr.Td("c"))).ToString()).Contains("<td>c</td>");
        await Assert.That(HtmlMarkup.Form(f => f.Method("post").Action("/s")).ToString()).Contains("method=\"post\"");
    }

    [Test]
    public async Task CompositeFactories_ListsTableCodeAlert()
    {
        await Assert.That(HtmlMarkup.Ul(["a", "b"]).ToString()).Contains("<li>a</li>");
        await Assert.That(HtmlMarkup.Ol(["1"]).ToString()).Contains("<ol>");
        await Assert.That(HtmlMarkup.Table(["H"], [["c"]]).ToString()).Contains("<th>H</th>");
        await Assert.That(HtmlMarkup.PreCode("x", "cs").ToString())
            .IsEqualTo("<pre><code class=\"language-cs\">x</code></pre>");
        await Assert.That(HtmlMarkup.Alert("Warning", "Careful").ToString()).Contains("alert-Warning");
        await Assert.That(HtmlMarkup.Fragment(HtmlMarkup.H1("A"), HtmlMarkup.P("B")).ToString())
            .IsEqualTo("<h1>A</h1><p>B</p>");
    }

    [Test]
    public async Task InlineStyle_FromCssRule()
    {
        var html = HtmlMarkup.Div(d => d.Style(r => r.Color("#f00").Margin(4)).Text("x")).ToString();

        await Assert.That(html).Contains("style=\"color: #f00; margin: 4px;\"");
    }
}

public class CssBuilderTests
{
    [Test]
    public async Task Rule_EmitsCommonDeclarations()
    {
        var css = HtmlMarkup.Css(sheet => sheet
            .Rule(".card", r => r
                .Flex()
                .Gap("1rem")
                .Padding(16)
                .BorderRadius(8)
                .BackgroundColor("#fff")
                .Color("#111")))
            .ToString();

        await Assert.That(css).Contains(".card {");
        await Assert.That(css).Contains("display: flex;");
        await Assert.That(css).Contains("gap: 1rem;");
        await Assert.That(css).Contains("padding: 16px;");
        await Assert.That(css).Contains("border-radius: 8px;");
    }

    [Test]
    public async Task MediaAndKeyframes_EmitBlocks()
    {
        var css = HtmlMarkup.Css(sheet => sheet
            .Media("(max-width: 600px)", m => m.Rule("body", r => r.FontSize("14px")))
            .Keyframes("fade", k => k
                .From(r => r.Opacity(0))
                .To(r => r.Opacity(1))))
            .ToString();

        await Assert.That(css).Contains("@media (max-width: 600px)");
        await Assert.That(css).Contains("@keyframes fade");
        await Assert.That(css).Contains("opacity: 0;");
    }

    [Test]
    public async Task RootVars_Grid_AndRaw()
    {
        var css = HtmlMarkup.Css(sheet => sheet
            .Root(r => r.Var("accent", "#0a7").Var("--gap", "8px"))
            .Rule(".grid", r => r.Grid().GridTemplateColumns("1fr 1fr").Gap("8px"))
            .Raw("/* comment */"))
            .ToString();

        await Assert.That(css).Contains(":root");
        await Assert.That(css).Contains("--accent: #0a7;");
        await Assert.That(css).Contains("display: grid;");
        await Assert.That(css).Contains("/* comment */");
    }

    [Test]
    public async Task EnumDisplay_ToKebab()
    {
        var css = new CssRule(".x").Display(CssDisplay.InlineBlock).Position(CssPosition.Absolute).ToString();

        await Assert.That(css).Contains("display: inline-block;");
        await Assert.That(css).Contains("position: absolute;");
    }
}

public class JsBuilderTests
{
    [Test]
    public async Task FunctionAndClass_EmitDeclarations()
    {
        var js = HtmlMarkup.Js(script => script
            .Function("add", ["a", "b"], body => body.Return("a + b"))
            .Class("Counter", c => c
                .Field("count", "0")
                .Constructor(["initial = 0"], "this.count = initial;")
                .Method("inc", [], "this.count += 1; return this.count;")
                .Getter("value", "return this.count;")
                .StaticMethod("create", [], "return new Counter();")))
            .ToString();

        await Assert.That(js).Contains("function add(a, b)");
        await Assert.That(js).Contains("return a + b;");
        await Assert.That(js).Contains("class Counter");
        await Assert.That(js).Contains("constructor(initial = 0)");
        await Assert.That(js).Contains("inc()");
        await Assert.That(js).Contains("get value()");
        await Assert.That(js).Contains("static create()");
    }

    [Test]
    public async Task ConstLet_AndClassExtends()
    {
        var js = HtmlMarkup.Js(script => script
            .Const("PI", "3.14")
            .Let("n", "0")
            .Class("Child", c => c
                .Extends("Parent")
                .Setter("value", "v", "this._v = v;")
                .AsyncMethod("load", [], "return 1;")
                .StaticField("kind", "'x'")
                .Raw("tag = true;")))
            .ToString();

        await Assert.That(js).Contains("const PI = 3.14;");
        await Assert.That(js).Contains("let n = 0;");
        await Assert.That(js).Contains("extends Parent");
        await Assert.That(js).Contains("set value(v)");
        await Assert.That(js).Contains("async load()");
        await Assert.That(js).Contains("static kind = 'x';");
    }

    [Test]
    public async Task JsBody_IfAndLine()
    {
        var body = new JsBody()
            .Const("x", "1")
            .If("x > 0", b => b.Line("console.log(x)"))
            .Return("x");

        var text = body.ToString();
        await Assert.That(text).Contains("const x = 1;");
        await Assert.That(text).Contains("if (x > 0)");
        await Assert.That(text).Contains("console.log(x);");
        await Assert.That(text).Contains("return x;");
    }
}

public class SvgBuilderTests
{
    [Test]
    public async Task Svg_SelfClosesEmptyShapes()
    {
        var svg = HtmlMarkup.Svg(root => root
            .Width(100)
            .Height(100)
            .ViewBox(0, 0, 100, 100)
            .Circle(50, 50, 40, c => c.Attr("fill", "#0a7"))
            .Rect(10, 10, 20, 20))
            .ToString();

        await Assert.That(svg).Contains("xmlns=\"http://www.w3.org/2000/svg\"");
        await Assert.That(svg).Contains("<circle cx=\"50\" cy=\"50\" r=\"40\" fill=\"#0a7\" />");
        await Assert.That(svg).Contains("<rect x=\"10\" y=\"10\" width=\"20\" height=\"20\" />");
        await Assert.That(svg).Contains("</svg>");
    }

    [Test]
    public async Task Svg_PathsLinesTextAndGradients()
    {
        var svg = HtmlMarkup.Svg(root => root
            .Width("100%")
            .Height("2rem")
            .ViewBox("0 0 10 10")
            .Fill("none")
            .Stroke("#000")
            .SvgTitle("Demo")
            .Desc("A demo")
            .Defs(_ => { })
            .LinearGradient("g1", g =>
            {
                SvgRoot.Stop(g, "0%", "#000");
                SvgRoot.Stop(g, "100%", "#fff", 0.5);
            })
            .Path("M0 0 L10 10")
            .Line(0, 0, 10, 10)
            .Polyline("0,0 5,5")
            .Polygon("0,0 10,0 5,10")
            .Ellipse(5, 5, 2, 1)
            .Text(1, 2, "Hi")
            .Use("#g1")
            .G(g => g.Attr("opacity", "0.5")))
            .ToString();

        await Assert.That(svg).Contains("<title>Demo</title>");
        await Assert.That(svg).Contains("<path d=\"M0 0 L10 10\" />");
        await Assert.That(svg).Contains("<linearGradient");
        await Assert.That(svg).Contains("stop-opacity=\"0.5\"");
        await Assert.That(svg).Contains("<text x=\"1\" y=\"2\">Hi</text>");
    }

    [Test]
    public async Task Document_CanEmbedSvgCssAndJs()
    {
        var html = HtmlMarkup.Document(doc => doc
            .Title("Mixed")
            .StyleSheet(css => css.Rule("body", r => r.Margin(0)))
            .WithBody(body => body
                .Svg(svg => svg.Width(10).Height(10).Circle(5, 5, 4)))
            .Script(js => js.Function("noop", [], "")))
            .ToString();

        await Assert.That(html).Contains("<style>");
        await Assert.That(html).Contains("<svg");
        await Assert.That(html).Contains("<script>");
        await Assert.That(html).Contains("function noop()");
    }
}
