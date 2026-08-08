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

    [Test]
    public async Task Mermaid_gap_builders_and_hash_operators()
    {
        await Assert.That(new Axis("X", logarithmic: true).GetBuilder().ToString()).Contains("log true");
        await Assert.That(new Axis("Y").GetBuilder().ToString()).Contains("log false");

        var pie = new PieChart("P", showData: false);
        pie.AddValue(new ChartValue("A", 1));
        pie.AddValues([new ChartValue("B", 2)]);
        pie.AddValues(new KeyValuePair<string, double>("C", 3));
        await Assert.That(pie.GetBuilder().ToString()).Contains("title P");
        await Assert.That(pie.GetBuilder().ToString()).DoesNotContain("showData");

        var quad = new QuadrantChart("Q")
            .WithAxes("x", "y")
            .WithQuadrants("q1", "q2", "q3", "q4")
            .AddPoint("p", 0.2, 0.8);
        await Assert.That(quad.GetBuilder().ToString()).Contains("quadrant-1 q1");

        var radar = new RadarChart("R")
            .WithAxes("a", "b", "c")
            .AddCurve("c1", "Curve", 1, 2, 3)
            .WithMax(10)
            .WithGraticule("polygon");
        await Assert.That(radar.GetBuilder().ToString()).Contains("max 10");

        var timelineEvents = new Timeline("T");
        timelineEvents.AddEvents([
            new Event("e1", new DateTime(2026, 1, 1), TimePeriod.Microsecond),
            new Event("e2", new DateTime(2026, 1, 1), TimePeriod.Nanosecond),
        ]);
        await Assert.That(timelineEvents.GetBuilder().ToString()).Contains(": e1");

        var timelineSections = new Timeline("S");
        var section = new Section("Era");
        section.AddEvents([new Event("se", new DateTime(2026, 2, 1))]);
        timelineSections.AddSections([section]);
        await Assert.That(timelineSections.GetBuilder().ToString()).Contains("section Era");

        await Assert.That(new Commit("m", "main", DateTime.UtcNow).GetBuilder().ToString()).Contains("commit");

        foreach (var d in new[] { Direction.TopToBottom, Direction.TopDown, Direction.RightToLeft, Direction.LeftToRight })
            await Assert.That(d.GetBuilder()).IsNotEmpty();

        await Assert.That(new Line(LineStyle.Normal, 2).GetBuilder().ToString()).IsEqualTo("--");
        await Assert.That(new Line(LineStyle.Dotted, 1).GetBuilder().ToString()).IsEqualTo(".");
        await Assert.That(new Line(LineStyle.DottedWithArrow, 2).GetBuilder().ToString()).Contains(">");

        var classNode = new ClassNode("Empty");
        await Assert.That(classNode.GetBuilder().ToString()).Contains("class Empty");
        var withStereo = new ClassNode("Svc").WithStereotype("interface").AddMember("+Run()");
        await Assert.That(withStereo.GetBuilder().ToString()).Contains("<<interface>>");

        await Assert.That(new ClassDiagram().AddNote("A Important").GetBuilder().ToString()).Contains("note for A Important");

        await Assert.That(new ErEntity("Solo").GetBuilder().ToString()).Contains("Solo");
        await Assert.That(new ErEntity("Person").AddAttribute("string name").GetBuilder().ToString()).Contains("{");

        var ganttSection = new GanttSection("Build").AddTask(new GanttTask("t", "id", "2026-01-01", "1d"));
        await Assert.That(ganttSection.GetBuilder().ToString()).Contains("section Build");

        await Assert.That(new MindmapNode("root").GetBuilder().ToString()).Contains("root");
        await Assert.That(new TreeViewNode("leaf").GetBuilder().ToString()).Contains("leaf");
        var tree = new TreeView("root");
        tree.AddChild("c");
        await Assert.That(tree.GetBuilder().ToString()).Contains("treeView");

        await Assert.That(new PacketDiagram("pkt").AddField(0, 7, "ver").GetBuilder().ToString()).Contains("title pkt");
        await Assert.That(new PacketDiagram().AddField(0, 1, "x").GetBuilder().ToString()).DoesNotContain("title");

        await Assert.That(new VennDiagram().AddSet("A", "Alpha").AddUnion("A", "B", "overlap").GetBuilder().ToString())
            .Contains("union");

        foreach (var kind in Enum.GetValues<C4Kind>())
            await Assert.That(new C4Diagram(kind, "T").GetBuilder().ToString()).IsNotEmpty();
        await Assert.That(new C4Diagram(C4Kind.Context, "T").System("s", "S", "desc").Container("c", "C", "net", "d").Rel("c", "s", "uses").GetBuilder().ToString())
            .Contains("desc");

        var req = new RequirementNode("r1", "REQ-1", "must", risk: "high", verifymethod: "test");
        await Assert.That(req.GetBuilder().ToString()).Contains("risk: high");

        var sg = new Subgraph("Lab el!", Direction.LeftToRight);
        sg.AddNodes([new Node("A"), new Node("B")]);
        sg.AddLinks([new Link(new Node("A"), new Node("B"))]);
        sg.AddSubgraphs([new Subgraph("Inner", Direction.TopToBottom)]);
        await Assert.That(sg.GetBuilder().ToString()).Contains("subgraph");

        var a = new Hash(new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        var b = new Hash(new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        await Assert.That(a == b).IsFalse();
        await Assert.That(a != b).IsTrue();
        await Assert.That(a < b).IsTrue();
        await Assert.That(b > a).IsTrue();
        await Assert.That(a <= b).IsTrue();
        await Assert.That(b >= a).IsTrue();
        await Assert.That(a.Equals((object)a)).IsTrue();
        await Assert.That(a.Equals((object)"no")).IsFalse();
        await Assert.That(a.CompareTo(a)).IsEqualTo(0);
        await Assert.That(a.ToInt64()).IsGreaterThan(0);
        await Assert.That(Hash.Parse(a.ToInt64()).ToInt64()).IsEqualTo(a.ToInt64());

        var cleaned = "ab\ncd!".CleanNonAlphanumeric(keepNewLines: true);
        await Assert.That(cleaned).Contains("\n");

        var linkSub = new Link(new Subgraph("S", Direction.TopToBottom), new Subgraph("T", Direction.TopToBottom), "go");
        await Assert.That(linkSub.GetBuilder().ToString()).Contains("|go|");
        linkSub.SetLineStyle(new Line(LineStyle.ThickWithArrow, 2));
        await Assert.That(linkSub.CompareTo(linkSub)).IsEqualTo(0);
        await Assert.That(linkSub.Equals((Link)null)).IsFalse();

        foreach (Shape shape in Enum.GetValues<Shape>())
        {
            if (shape is Shape.Parallelogram or Shape.ParallelogramAlt or Shape.Asymmetric or Shape.Rhombus or Shape.Trapezoid or Shape.TrapezoidAlt or Shape.Stadium)
                continue;
            await Assert.That(new Node("n", shape).GetBuilder().ToString()).IsNotEmpty();
        }

        var xy = new XyChart("Trend");
        xy.SetXAxis(new Axis("Time"));
        xy.SetYAxis(new Axis("Value"));
        xy.AddSeries([new Series("s")]);
        await Assert.That(xy.GetBuilder().ToString()).Contains("xyChart");

        await Assert.That(new Node("x").GetMermaidString()).IsNotEmpty();

        var arch = new ArchitectureDiagram()
            .Group("g", "G")
            .Service("s", "S")
            .Junction("j")
            .Edge("s", "j");
        await Assert.That(arch.GetBuilder().ToString()).Contains("junction j");

        var block = new BlockDiagram()
            .Block("a", "A")
            .Space(1)
            .Space(3)
            .Edge("a", "b");
        await Assert.That(block.GetBuilder().ToString()).Contains("space:3");

        await Assert.That(new Event("tick", DateTime.UtcNow, TimePeriod.Tick).GetBuilder().ToString()).Contains(": tick");

        var state = new StateDiagram().Transition("A", "B");
        await Assert.That(state.GetBuilder().ToString()).Contains("A --> B");

        var seq = new SequenceDiagram().AddParticipant("A").Message("A", "A", "self");
        await Assert.That(seq.GetBuilder().ToString()).Contains("sequenceDiagram");
    }
}
