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
