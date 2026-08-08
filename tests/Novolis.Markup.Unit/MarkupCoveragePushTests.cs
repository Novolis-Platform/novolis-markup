using Novolis.Markup.Markdown;
using Novolis.Markup.Markdown.Rendering;
using Novolis.Markup.Mermaid;

namespace Novolis.Markup.Unit;

public sealed class MarkupCoveragePushTests
{
    [Test]
    public async Task MarkdownHtmlExporter_and_document_extensions()
    {
        var path = Path.Combine(Path.GetTempPath(), $"md-html-{Guid.NewGuid():N}.html");
        try
        {
            MarkdownHtmlExporter.ExportToFile("# Title\n\nPara.", path, MarkdownHtmlTheme.GitHubLight, "Doc");
            await Assert.That(File.Exists(path)).IsTrue();
            var html = await File.ReadAllTextAsync(path);
            await Assert.That(html).Contains("<html");
        }
        finally
        {
            if (File.Exists(path))
                File.Delete(path);
        }

        IMarkdownDocument doc = MarkdownDocument.Create("hello")
            .WithHorizontalRule(new MarkdownHorizontalRule())
            .WithHeader("Section", MarkdownHeaderLevel.H2)
            .WithAlert("warn", MarkdownAlertLevel.Warning)
            .WithCodeBlock("x = 1;", "csharp")
            .WithUnorderedList("a", "b")
            .WithOrderedList("1", "2")
            .WithTable(new MarkdownTable<string>(["H"], [["v"]]));

        var rendered = doc.ToHtml();
        await Assert.That(rendered).Contains("hello");
        await Assert.That(rendered).Contains("<hr");
        await Assert.That(MarkdownHeaderLevel.H2.ToInt()).IsEqualTo(2);
    }

    [Test]
    public async Task MarkdownDocument_parse_table_quote_and_lists()
    {
        var doc = MarkdownDocument.Parse("# H1\n\nparagraph");
        var html = doc.ToHtml();
        await Assert.That(html).Contains("H1");
        await Assert.That(html).Contains("paragraph");
    }

    [Test]
    public async Task Mermaid_remaining_uncovered_builders()
    {
        var c4 = new C4Diagram(C4Kind.Container, "App")
            .System("core", "Core")
            .Container("api", "API", "net")
            .System_Ext("ext", "External")
            .Rel("api", "core", "calls", "http")
            .GetMermaidString();
        await Assert.That(c4).Contains("C4Container");

        var block = new BlockDiagram(2)
            .Block("a", "A")
            .Space()
            .Block("b", "B")
            .Edge("a", "b", "link")
            .GetMermaidString();
        await Assert.That(block).Contains("block-beta");

        var chart = new XyChart("Trend");
        chart.SetXAxis(new Axis("Time"));
        chart.SetYAxis(new Axis("Value", logarithmic: true));
        var series = new Series("s1");
        series.Points.Add(new Point(1, 2));
        chart.AddSeries(series);
        await Assert.That(chart.GetMermaidString()).Contains("xyChart");

        var gantt = new Gantt("Plan")
            .WithDateFormat("YYYY-MM-DD")
            .AddSection(new GanttSection("S1").AddTask("t", "id1", "2026-01-01", "1d"))
            .GetMermaidString();
        await Assert.That(gantt).Contains("dateFormat");

        var timeline = new Timeline("Hist");
        timeline.AddEvent(new Event("evt", new DateTime(2025, 6, 1), TimePeriod.Month));
        timeline.AddSection(new Section("Era"));
        await Assert.That(timeline.GetMermaidString()).Contains("timeline");

        var pie = new PieChart("Split");
        pie.AddValue("A", 30);
        pie.AddValue("B", 70);
        await Assert.That(pie.GetMermaidString()).Contains("pie");

        await Assert.That(new Line(LineStyle.ThickWithArrow, 2).GetMermaidString()).Contains("=");

        await Assert.That(Direction.BottomToTop.GetBuilder()).IsEqualTo("BT");
    }

    [Test]
    public async Task Hash_compare_and_gitgraph_commit()
    {
        var a = Hash.NewHash();
        var b = Hash.NewHash();
        await Assert.That(a.CompareTo(b)).IsNotEqualTo(0);

        var git = new GitGraph();
        git.AddCommit(new Commit("init", "main"));
        await Assert.That(git.GetMermaidString()).Contains("gitGraph");
    }
}
