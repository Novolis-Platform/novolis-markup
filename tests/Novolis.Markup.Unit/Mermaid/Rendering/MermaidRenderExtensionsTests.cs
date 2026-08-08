using Novolis.Markup.Mermaid;
using Novolis.Markup.Mermaid.Rendering;

namespace Novolis.Markup.Mermaid.Tests.Rendering;

public sealed class MermaidRenderExtensionsTests
{
    [Test]
    public async Task ToSvg_Flowchart_ReturnsSvg()
    {
        var chart = CreateFlowchart();
        var svg = chart.ToSvg(MermaidRenderTheme.StudioDark);

        await Assert.That(svg).IsNotNull();
        await Assert.That(svg!).Contains("<svg");
    }

    [Test]
    public async Task ToSvg_SequenceDiagram_ReturnsSvg()
    {
        var diagram = new SequenceDiagram()
            .AddParticipant("A", "Alice")
            .AddParticipant("B", "Bob")
            .Message("A", "B", "Hello");

        var svg = diagram.ToSvg();

        await Assert.That(svg).IsNotNull();
        await Assert.That(svg!).Contains("<svg");
    }

    [Test]
    public async Task ToSvg_InvalidSource_ReturnsNull()
    {
        await Assert.That("this is not mermaid".ToSvg()).IsNull();
        await Assert.That("   ".ToSvg()).IsNull();
    }

    [Test]
    public async Task ToPng_Flowchart_HasPngSignature()
    {
        var png = CreateFlowchart().ToPng(MermaidRenderTheme.GitHubLight, scale: 1f);

        await Assert.That(png).IsNotNull();
        await Assert.That(png!.Length).IsGreaterThan(8);
        await Assert.That(png[0]).IsEqualTo((byte)0x89);
        await Assert.That(png[1]).IsEqualTo((byte)0x50);
        await Assert.That(png[2]).IsEqualTo((byte)0x4E);
        await Assert.That(png[3]).IsEqualTo((byte)0x47);
    }

    [Test]
    public async Task OptionsFor_Themes_SetDistinctBackgrounds()
    {
        var dark = MermaidSvgRenderer.OptionsFor(MermaidRenderTheme.StudioDark);
        var light = MermaidSvgRenderer.OptionsFor(MermaidRenderTheme.GitHubLight);

        await Assert.That(dark.Bg).IsEqualTo("#1e1e1e");
        await Assert.That(dark.Accent).IsEqualTo("#6eb5ff");
        await Assert.That(light.Bg).IsEqualTo("#ffffff");
        await Assert.That(light.Accent).IsEqualTo("#0969da");
    }

    [Test]
    public async Task ExportSvg_And_ExportPng_WriteFiles()
    {
        var chart = CreateFlowchart();
        var dir = Path.Combine(Path.GetTempPath(), "novolis-mermaid-render-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        try
        {
            var svgPath = Path.Combine(dir, "diagram.svg");
            var pngPath = Path.Combine(dir, "diagram.png");

            await Assert.That(chart.ExportSvg(svgPath)).IsTrue();
            await Assert.That(chart.ExportPng(pngPath)).IsTrue();
            await Assert.That(File.Exists(svgPath)).IsTrue();
            await Assert.That(File.Exists(pngPath)).IsTrue();
            await Assert.That(File.ReadAllText(svgPath)).Contains("<svg");
            await Assert.That(File.ReadAllBytes(pngPath)[0]).IsEqualTo((byte)0x89);
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }

    [Test]
    public async Task TryRenderSvg_NullOrBlank_ReturnsNull()
    {
        await Assert.That(MermaidSvgRenderer.TryRenderSvg(null)).IsNull();
        await Assert.That(MermaidSvgRenderer.TryRenderSvg("   ")).IsNull();
    }

    private static Flowchart CreateFlowchart()
    {
        var chart = new Flowchart(Direction.TopToBottom);
        var start = new Node("Start", Shape.Rounded);
        var end = new Node("Done");
        chart.AddNode(start);
        chart.AddNode(end);
        chart.AddLink(new Link(start, end, "next"));
        return chart;
    }
}
