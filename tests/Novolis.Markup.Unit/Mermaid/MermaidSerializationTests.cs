namespace Novolis.Markup.Mermaid.Tests;

public sealed class MermaidSerializationTests
{
    [Test]
    public async Task Parse_FlowchartRoundTrip_PreservesKindAndNodes()
    {
        var source = """
            flowchart LR
                A[Start]
                B([Done])
                A -->|go| B
            """;

        var doc = MermaidDocument.Parse(source);
        await Assert.That(doc.Kind).IsEqualTo(MermaidDiagramKind.Flowchart);
        await Assert.That(doc.Diagram).IsTypeOf<Flowchart>();
        var text = doc.ToMermaidString();
        await Assert.That(text).Contains("flowchart LR");
        await Assert.That(text).Contains("A[Start]");
        await Assert.That(text).Contains("B([Done])");
    }

    [Test]
    public async Task Parse_FrontMatter_RoundTripsTheme()
    {
        var source = """
            ---
            title: Demo
            config:
              theme: forest
              look: classic
              layout: elk
            ---
            pie showData
            title Keys
            "A" : 1
            """;

        var doc = MermaidDocument.Parse(source);
        await Assert.That(doc.FrontMatter.Title).IsEqualTo("Demo");
        await Assert.That(doc.FrontMatter.Theme).IsEqualTo("forest");
        await Assert.That(doc.FrontMatter.Look).IsEqualTo("classic");
        await Assert.That(doc.FrontMatter.Layout).IsEqualTo("elk");
        await Assert.That(doc.Kind).IsEqualTo(MermaidDiagramKind.Pie);
        var json = MermaidJson.Serialize(doc);
        var again = MermaidJson.Deserialize(json);
        await Assert.That(again.Kind).IsEqualTo(MermaidDiagramKind.Pie);
        await Assert.That(again.ToMermaidString()).Contains("forest");
        await Assert.That(again.ToMermaidString()).Contains("\"A\" : 1");
    }

    [Test]
    public async Task Parse_DetectsNewKinds()
    {
        await Assert.That(MermaidKindDetector.TryDetect("ishikawa-beta\n  P\n")).IsEqualTo(MermaidDiagramKind.Ishikawa);
        await Assert.That(MermaidKindDetector.TryDetect("usecase-beta\nactor A\n")).IsEqualTo(MermaidDiagramKind.UseCase);
        await Assert.That(MermaidKindDetector.TryDetect("wardley-beta\n")).IsEqualTo(MermaidDiagramKind.Wardley);
        await Assert.That(MermaidKindDetector.TryDetect("cynefin-beta\n")).IsEqualTo(MermaidDiagramKind.Cynefin);
        await Assert.That(MermaidKindDetector.TryDetect("railroad-ebnf-beta\n")).IsEqualTo(MermaidDiagramKind.Railroad);
        await Assert.That(MermaidKindDetector.TryDetect("eventmodeling\n")).IsEqualTo(MermaidDiagramKind.EventModeling);
        await Assert.That(MermaidKindDetector.TryDetect("xychart\nline [1]\n")).IsEqualTo(MermaidDiagramKind.XyChart);
        await Assert.That(MermaidKindDetector.TryDetect("sankey-beta\nA,B,1\n")).IsEqualTo(MermaidDiagramKind.Sankey);
    }

    [Test]
    public async Task Json_SerializeBuilder_RoundTripsIshikawa()
    {
        var diagram = new IshikawaDiagram("Outage");
        diagram.AddCause("People").AddCause("No runbook");
        var json = MermaidJson.Serialize(diagram);
        var parsed = MermaidJson.Deserialize(json);
        await Assert.That(parsed.Kind).IsEqualTo(MermaidDiagramKind.Ishikawa);
        await Assert.That(parsed.Diagram).IsTypeOf<IshikawaDiagram>();
        await Assert.That(parsed.ToMermaidString()).Contains("Outage");
        await Assert.That(parsed.ToMermaidString()).Contains("No runbook");
    }

    [Test]
    public async Task Parse_UnknownHeader_Fails()
    {
        await Assert.That(MermaidDocument.TryParse("notADiagram\nfoo", out _)).IsFalse();
    }

    [Test]
    public async Task From_Builder_EmitsSameKind()
    {
        var doc = MermaidDocument.From(new SequenceDiagram().AddParticipant("A").Message("A", "B", "Hi"));
        await Assert.That(doc.Kind).IsEqualTo(MermaidDiagramKind.Sequence);
        await Assert.That(doc.ToMermaidString()).Contains("A->>B: Hi");
    }
}
