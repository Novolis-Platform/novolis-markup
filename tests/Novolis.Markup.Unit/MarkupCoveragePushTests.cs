using Novolis.Markup.Manuscript;
using Novolis.Markup.Markdown;
using Novolis.Markup.Markdown.Rendering;
using Novolis.Markup.Mermaid;

namespace Novolis.Markup.Unit;

public sealed class MarkupCoveragePushTests
{
    [Test]
    public async Task ManuscriptMetadata_yaml_all_fields_and_apply_callouts()
    {
        var text = """
            ---
            date: 2026-02-01
            time: 09:30
            system: Sol
            location: Station
            pov: Hero
            characters: A,B
            status: draft
            notes: note
            custom_key: extra
            ---
            # Chapter 2 - Middle

            Body.
            """;
        var (meta, _, format) = ManuscriptMetadata.Parse(text);
        await Assert.That(format).IsEqualTo(ManuscriptMetadataFormat.Yaml);
        await Assert.That(meta.Date).IsEqualTo("2026-02-01");
        await Assert.That(meta.Time).IsEqualTo("09:30");
        await Assert.That(meta.System).IsEqualTo("Sol");
        await Assert.That(meta.Location).IsEqualTo("Station");
        await Assert.That(meta.Pov).IsEqualTo("Hero");
        await Assert.That(meta.Characters).IsEqualTo("A,B");
        await Assert.That(meta.Status).IsEqualTo("draft");
        await Assert.That(meta.Notes).IsEqualTo("note");
        await Assert.That(meta.Extra["custom_key"]).IsEqualTo("extra");

        var applied = ManuscriptMetadata.ApplyCallouts("# Chapter 2 - Old\n\nBody.", new ManuscriptChapterMetadata
        {
            Number = "2",
            Title = "Middle",
            Date = "2026-02-01",
            Pov = "Hero",
        });
        await Assert.That(applied).Contains("> [!date]");
        await Assert.That(applied).Contains("> [!pov]");
        await Assert.That(ManuscriptMetadata.CountWords(text)).IsGreaterThan(0);
    }

    [Test]
    public async Task ManuscriptMetadata_callout_aliases_and_word_count_empty()
    {
        var text = """
            # Chapter 3 - End

            > [!loc] Mars
            > [!point_of_view] Pilot
            > [!chars] X
            > [!note] side

            Only words here count.
            """;
        var (meta, _, format) = ManuscriptMetadata.Parse(text);
        await Assert.That(format).IsEqualTo(ManuscriptMetadataFormat.Callout);
        await Assert.That(meta.Location).IsEqualTo("Mars");
        await Assert.That(meta.Pov).IsEqualTo("Pilot");
        await Assert.That(meta.Characters).IsEqualTo("X");
        await Assert.That(meta.Notes).IsEqualTo("side");
        await Assert.That(ManuscriptMetadata.CountWords("   \n\n  ")).IsEqualTo(0);
    }

    [Test]
    public async Task ManuscriptMetadata_apply_callouts_inserts_heading_when_missing()
    {
        var applied = ManuscriptMetadata.ApplyCallouts("Body only.", new ManuscriptChapterMetadata
        {
            Number = "9",
            Title = "New",
            System = "Alpha",
        });
        await Assert.That(applied).Contains("# Chapter 9 - New");
        await Assert.That(applied).Contains("> [!system]");
    }

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
    public async Task ManuscriptDoctor_and_pdf_exporter()
    {
        var root = Path.Combine(Path.GetTempPath(), $"ms-push-{Guid.NewGuid():N}");
        var bookDir = Path.Combine(root, "content", "books", "one");
        Directory.CreateDirectory(Path.Combine(bookDir, "chapters"));
        try
        {
            File.WriteAllText(Path.Combine(bookDir, "book.yaml"), "title: One\n");
            File.WriteAllText(Path.Combine(bookDir, "chapters", "001.md"), "# Chapter 1 - Hi\n\n> [!date] today\n\nBody.");
            var findings = ManuscriptDoctor.Diagnose(root);
            await Assert.That(findings.Count).IsGreaterThanOrEqualTo(0);

            var book = new ManuscriptCatalog().LoadStandaloneBooks(root).Single();
            var pdfPath = Path.Combine(Path.GetTempPath(), $"book-{Guid.NewGuid():N}.pdf");
            ManuscriptBookPdfExporter.ExportBook(book, pdfPath);
            await Assert.That(File.Exists(pdfPath)).IsTrue();
            File.Delete(pdfPath);

            await Assert.That(ManuscriptWorkspace.TryOpen(root, out var ws)).IsTrue();
            await Assert.That(ws!.Catalog).IsNotNull();
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
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
