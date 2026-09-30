using Novolis.Markup.Html;
using Novolis.Markup.Html.Css;
using Novolis.Markup.Html.Js;
using Novolis.Markup.Html.Svg;

namespace Novolis.Markup.Unit.Html;

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
