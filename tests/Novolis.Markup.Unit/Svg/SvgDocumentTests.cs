using Novolis.Markup.Html;
using Novolis.Markup.Svg;

namespace Novolis.Markup.Unit.Svg;

public sealed class SvgDocumentTests
{
    [Test]
    public async Task Document_UsesTypedGeometryAndEscapesText()
    {
        var svg = new SvgDocument()
            .Width(SvgLength.Pixels(120))
            .Height(SvgLength.Pixels(120))
            .ViewBox(new SvgViewBox(0, 0, 120, 120))
            .Title("A & <title>")
            .Circle(new SvgPoint(60, 60), 48, circle => circle
                .Fill(SvgPaint.Color("#1a9b8e"))
                .Stroke(SvgPaint.Color("#0b3440"))
                .StrokeWidth(3))
            .Path(
                new SvgPathData()
                    .MoveTo(new SvgPoint(12, 96))
                    .LineTo(new SvgPoint(60, 24))
                    .LineTo(new SvgPoint(108, 96))
                    .Close(),
                path => path
                    .Fill(SvgPaint.None)
                    .Stroke(SvgPaint.CurrentColor)
                    .StrokeWidth(2)
                    .StrokeLineJoin(SvgLineJoin.Round))
            .Text(new SvgPoint(60, 112), "<label>", text => text
                .Fill(SvgPaint.CurrentColor)
                .TextAnchor(SvgTextAnchor.Middle));

        var output = svg.ToString();

        await Assert.That(output).Contains("xmlns=\"http://www.w3.org/2000/svg\"");
        await Assert.That(output).Contains("width=\"120px\"");
        await Assert.That(output).Contains("viewBox=\"0 0 120 120\"");
        await Assert.That(output).Contains("<title>A &amp; &lt;title&gt;</title>");
        await Assert.That(output).Contains("<path d=\"M 12 96 L 60 24 L 108 96 Z\"");
        await Assert.That(output).Contains("&lt;label&gt;");
    }

    [Test]
    public async Task DefinitionsAndTransforms_UseTypedReferences()
    {
        var output = new SvgDocument()
            .ViewBox(0, 0, 100, 100)
            .Defs(defs => defs
                .LinearGradient("accent", gradient => gradient
                    .X1(SvgLength.Percent(0))
                    .X2(SvgLength.Percent(100))
                    .Stop(SvgLength.Percent(0), SvgPaint.Color("#0a7"))
                    .Stop(SvgLength.Percent(100), SvgPaint.Color("#07a"), 0.5))
                .Marker("arrow", marker => marker
                    .MarkerWidth(10)
                    .MarkerHeight(7)
                    .RefX(9)
                    .RefY(3.5)
                    .OrientAuto()
                    .Path(
                        new SvgPathData()
                            .MoveTo(new SvgPoint(0, 0))
                            .LineTo(new SvgPoint(10, 3.5))
                            .LineTo(new SvgPoint(0, 7))
                            .Close(),
                        path => path.Fill(SvgPaint.CurrentColor))))
            .Group(group => group
                .Transform(new SvgTransformList().Translate(5, 6).Scale(2))
                .Rect(new SvgRect(0, 0, 20, 20), rectangle => rectangle
                    .CornerRadius(4)
                    .Fill(SvgPaint.Url("accent")))
                .Line(new SvgPoint(0, 0), new SvgPoint(20, 20), line => line
                    .Stroke(SvgPaint.CurrentColor)
                    .MarkerEnd(SvgReference.To("arrow"))))
            .ToString();

        await Assert.That(output).Contains("linearGradient");
        await Assert.That(output).Contains("stop-opacity=\"0.5\"");
        await Assert.That(output).Contains("transform=\"translate(5 6) scale(2)\"");
        await Assert.That(output).Contains("fill=\"url(#accent)\"");
        await Assert.That(output).Contains("marker-end=\"url(#arrow)\"");
    }

    [Test]
    public async Task Html_CanEmbedTypedDocument()
    {
        var html = HtmlMarkup.Document(document => document
            .WithBody(body => body.SvgDocument(svg => svg
                .Width(16)
                .Height(16)
                .Circle(8, 8, 6, circle => circle.Fill(SvgPaint.Color("#0a7"))))))
            .ToString();

        await Assert.That(html).Contains("<svg");
        await Assert.That(html).Contains("<circle");
        await Assert.That(html).Contains("fill=\"#0a7\"");
    }

    [Test]
    public async Task InvalidGeometry_IsRejected()
    {
        await Assert.That(() => new SvgCircle(new SvgPoint(0, 0), -1))
            .Throws<ArgumentOutOfRangeException>();
        await Assert.That(() => new SvgPathElement(new SvgPathData()))
            .Throws<ArgumentException>();
        await Assert.That(() => new SvgPolygon([new SvgPoint(0, 0), new SvgPoint(1, 1)]))
            .Throws<ArgumentException>();
    }
}
