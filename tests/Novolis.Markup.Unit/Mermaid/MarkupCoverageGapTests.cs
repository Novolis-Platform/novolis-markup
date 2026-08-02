namespace Novolis.Markup.Mermaid.Tests;

public sealed class MarkupCoverageGapTests
{
    [Test]
    public async Task Flowchart_CleanDuplicates_RemovesRepeatedNodesLinksAndSubgraphs()
    {
        var flowchart = new Flowchart();
        var a = new Node("A");
        var b = new Node("B");
        flowchart.AddNode(a);
        flowchart.AddNode(a);
        flowchart.AddNode(b);
        var link = new Link(a, b);
        flowchart.AddLink(link);
        flowchart.AddLink(link);
        var sg = new Subgraph("Dup", Direction.TopToBottom);
        flowchart.AddSubgraph(sg);
        flowchart.AddSubgraph(sg);

        var text = flowchart.GetBuilder().ToString();
        await Assert.That(text.Split("[A]", StringSplitOptions.None).Length - 1).IsEqualTo(1);
        await Assert.That(text.Split("subgraph Dup", StringSplitOptions.None).Length - 1).IsEqualTo(1);
        await Assert.That(text.Split("--->", StringSplitOptions.None).Length - 1).IsEqualTo(1);
    }

    [Test]
    public async Task Link_CompareAndEquals_UseIdAndEndpoints()
    {
        var a = new Node("X");
        var b = new Node("Y");
        var link = new Link(a, b, "go");
        var same = new Link(a, b, "go");
        var other = new Link(b, a);

        await Assert.That(link.Equals(same)).IsFalse();
        await Assert.That(link.CompareTo(other)).IsNotEqualTo(0);
        await Assert.That(link.CompareTo(null)).IsEqualTo(1);
        await Assert.That(link.Equals(link)).IsTrue();
    }

    [Test]
    public async Task Link_InvalidSource_Throws()
    {
        await Assert.That(() => new Link(new FakeMermaid(), new Node("A")))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task SequenceDiagram_EmitsAllArrowStylesAndBlocks()
    {
        var text = new SequenceDiagram()
            .AddParticipant("A", "Alice", asActor: true)
            .AddParticipant("B")
            .Message("A", "B", "solid", SequenceArrow.Solid)
            .Message("A", "B", "dotted", SequenceArrow.Dotted)
            .Message("A", "B", "open", SequenceArrow.SolidOpen)
            .Message("A", "B", "dopen", SequenceArrow.DottedOpen)
            .Message("A", "B", "cross", SequenceArrow.SolidCross)
            .Message("A", "B", "dcross", SequenceArrow.DottedCross)
            .Message("A", "B", "async", SequenceArrow.SolidAsync)
            .Message("A", "B", "dasync", SequenceArrow.DottedAsync)
            .BeginBlock("loop", "retry")
            .Else("alt branch")
            .End()
            .NoteOver("A", "note")
            .Note("right of", "B", "side")
            .Activate("A")
            .Deactivate("A")
            .GetBuilder()
            .ToString();

        await Assert.That(text).Contains("sequenceDiagram");
        await Assert.That(text).Contains("actor A as Alice");
        await Assert.That(text).Contains("A->>B: solid");
        await Assert.That(text).Contains("A-->>B: dotted");
        await Assert.That(text).Contains("A-)B: async");
        await Assert.That(text).Contains("loop retry");
        await Assert.That(text).Contains("else alt branch");
        await Assert.That(text).Contains("Note over A");
        await Assert.That(text).Contains("activate A");
    }

    [Test]
    public async Task StateDiagram_EmitsCompositeAndNotes()
    {
        var text = new StateDiagram()
            .Transition("[*]", "Idle")
            .State("Idle", "Waiting")
            .Transition("Idle", "Active", "go")
            .BeginComposite("Active")
            .Transition("Active", "Done")
            .EndComposite()
            .Note("right", "Done", "finished")
            .GetBuilder()
            .ToString();

        await Assert.That(text).Contains("stateDiagram-v2");
        await Assert.That(text).Contains("[*] --> Idle");
        await Assert.That(text).Contains("state \"Waiting\" as Idle");
        await Assert.That(text).Contains("state Active {");
        await Assert.That(text).Contains("note right of Done");
    }

    [Test]
    public async Task Timeline_EventFormatsAllPeriods()
    {
        var baseDate = new DateTime(2026, 3, 15, 14, 30, 45, 123, DateTimeKind.Utc);
        var periods = new[]
        {
            (TimePeriod.Year, "2026"),
            (TimePeriod.Month, "2026-03"),
            (TimePeriod.Day, "2026-03-15"),
            (TimePeriod.Hour, "2026-03-15 14"),
            (TimePeriod.Minute, "2026-03-15 14:30"),
            (TimePeriod.Second, "2026-03-15 14:30:45"),
            (TimePeriod.Millisecond, "2026-03-15 14:30:45.123"),
        };

        foreach (var (period, expected) in periods)
        {
            var text = new Event("evt", baseDate, period).GetBuilder().ToString();
            await Assert.That(text).StartsWith(expected);
            await Assert.That(text).Contains(": evt");
        }
    }

    [Test]
    public async Task ArchitectureDiagram_EmitsGroupsJunctionsAndIcons()
    {
        var text = new ArchitectureDiagram()
            .Group("core", "Core", "cloud")
            .Service("api", "API", "server", "core")
            .Junction("hub", "core")
            .Edge("api", "hub", "calls")
            .GetBuilder()
            .ToString();

        await Assert.That(text).Contains("group core(cloud)[Core]");
        await Assert.That(text).Contains("service api(server)[API] in core");
        await Assert.That(text).Contains("junction hub in core");
        await Assert.That(text).Contains("api:hub : calls");
    }

    [Test]
    public async Task ClassRelation_EmitsAllRelationTypes()
    {
        var types = new[]
        {
            (ClassRelationType.Inheritance, "<|--"),
            (ClassRelationType.Composition, "*--"),
            (ClassRelationType.Aggregation, "o--"),
            (ClassRelationType.Association, "-->"),
            (ClassRelationType.Link, "--"),
            (ClassRelationType.Dependency, "..>"),
            (ClassRelationType.Realization, "..|>"),
        };

        foreach (var (type, token) in types)
        {
            var labeled = new ClassRelation("A", "B", type, "uses").GetBuilder().ToString();
            var plain = new ClassRelation("A", "B", type).GetBuilder().ToString();
            await Assert.That(labeled).Contains(token);
            await Assert.That(labeled).Contains(": uses");
            await Assert.That(plain).Contains(token);
        }
    }

    [Test]
    public async Task MindmapNode_AllShapes_FormatLabels()
    {
        await Assert.That(new MindmapNode("plain").FormatLabel()).IsEqualTo("plain");
        await Assert.That(new MindmapNode("square", MindmapShape.Square).FormatLabel()).IsEqualTo("[square]");
        await Assert.That(new MindmapNode("round", MindmapShape.Rounded).FormatLabel()).IsEqualTo("(round)");
        await Assert.That(new MindmapNode("root", MindmapShape.Circle).FormatLabel()).IsEqualTo("((root))");
        await Assert.That(new MindmapNode("bang", MindmapShape.Bang).FormatLabel()).IsEqualTo(")bang(");
        await Assert.That(new MindmapNode("hex", MindmapShape.Hexagon).FormatLabel()).IsEqualTo("{{hex}}");

        var root = new MindmapNode("root", MindmapShape.Circle);
        var child = root.AddChild("leaf", MindmapShape.Hexagon);
        await Assert.That(root.FormatLabel()).IsEqualTo("((root))");
        await Assert.That(child.FormatLabel()).IsEqualTo("{{leaf}}");
    }

    sealed class FakeMermaid : IMermaidable
    {
        public Hash Id { get; } = Hash.NewHash();
        public IIndentedStringBuilder GetBuilder() => new IndentedStringBuilder();
    }
}
