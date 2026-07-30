namespace Novolis.Markup.Mermaid.Tests;

public class DiagramCoverageTests
{
    [Test]
    public async Task SequenceDiagram_EmitsParticipantsAndMessages()
    {
        var diagram = new SequenceDiagram()
            .AddParticipant("A", "Alice")
            .AddParticipant("B", "Bob")
            .Message("A", "B", "Hello")
            .Message("B", "A", "Hi", SequenceArrow.Dotted);

        var text = diagram.GetMermaidString();
        await Assert.That(text).Contains("sequenceDiagram");
        await Assert.That(text).Contains("participant A as Alice");
        await Assert.That(text).Contains("A->>B: Hello");
        await Assert.That(text).Contains("B-->>A: Hi");
    }

    [Test]
    public async Task ClassDiagram_EmitsInheritance()
    {
        var animal = new ClassNode("Animal").WithStereotype("abstract").AddMember("+name: string");
        var dog = new ClassNode("Dog").AddMember("+bark()");
        var diagram = new ClassDiagram()
            .AddClass(animal)
            .AddClass(dog)
            .AddRelation(new ClassRelation("Animal", "Dog", ClassRelationType.Inheritance));

        var text = diagram.GetMermaidString();
        await Assert.That(text).Contains("classDiagram");
        await Assert.That(text).Contains("class Animal");
        await Assert.That(text).Contains("Animal <|-- Dog");
    }

    [Test]
    public async Task StateDiagram_EmitsTransitions()
    {
        var text = new StateDiagram()
            .Transition("[*]", "Idle")
            .Transition("Idle", "Done", "finish")
            .Transition("Done", "[*]")
            .GetMermaidString();

        await Assert.That(text).Contains("stateDiagram-v2");
        await Assert.That(text).Contains("[*] --> Idle");
        await Assert.That(text).Contains("Idle --> Done : finish");
    }

    [Test]
    public async Task ErDiagram_EmitsRelationshipAndEntity()
    {
        var customer = new ErEntity("CUSTOMER").AddAttribute("string name PK");
        var diagram = new ErDiagram()
            .AddRelationship(new ErRelationship("CUSTOMER", ErCardinality.ExactlyOne, ErCardinality.ZeroOrMore, "ORDER", "places"))
            .AddEntity(customer);

        var text = diagram.GetMermaidString();
        await Assert.That(text).Contains("erDiagram");
        await Assert.That(text).Contains("CUSTOMER ||--}o ORDER : places");
        await Assert.That(text).Contains("string name PK");
    }

    [Test]
    public async Task Mindmap_EmitsHierarchy()
    {
        var map = new Mindmap("Root");
        map.AddChild("Child").AddChild("Leaf");

        var text = map.GetMermaidString();
        await Assert.That(text).Contains("mindmap");
        await Assert.That(text).Contains("((Root))");
        await Assert.That(text).Contains("Child");
        await Assert.That(text).Contains("Leaf");
    }

    [Test]
    public async Task Gantt_EmitsSectionAndTask()
    {
        var section = new GanttSection("Build").AddTask("Compile", "c1", "2026-01-01", "3d");
        var text = new Gantt("Ship").AddSection(section).GetMermaidString();

        await Assert.That(text).Contains("gantt");
        await Assert.That(text).Contains("title Ship");
        await Assert.That(text).Contains("section Build");
        await Assert.That(text).Contains("Compile :c1, 2026-01-01, 3d");
    }

    [Test]
    public async Task Journey_EmitsScoredTask()
    {
        var section = new JourneySection("Morning").AddTask("Coffee", 5, "Me");
        var text = new Journey("Day").AddSection(section).GetMermaidString();

        await Assert.That(text).Contains("journey");
        await Assert.That(text).Contains("Coffee: 5: Me");
    }

    [Test]
    public async Task QuadrantRadarSankey_EmitHeaders()
    {
        var q = new QuadrantChart("Priority").AddPoint("A", 0.8, 0.9).GetMermaidString();
        var r = new RadarChart("Skills").WithAxes("A", "B").AddCurve("c1", "Team", 1, 2).GetMermaidString();
        var s = new Sankey().AddLink("A", "B", 10).GetMermaidString();

        await Assert.That(q).Contains("quadrantChart");
        await Assert.That(r).Contains("radar-beta");
        await Assert.That(s).Contains("sankey-beta");
    }

    [Test]
    public async Task NewerDiagramKinds_EmitHeaders()
    {
        await Assert.That(new ArchitectureDiagram().Service("api", "API").GetMermaidString()).Contains("architecture-beta");
        await Assert.That(new BlockDiagram(2).Block("a", "A").GetMermaidString()).Contains("block-beta");
        await Assert.That(new C4Diagram(C4Kind.Context, "Sys").Person("u", "User").GetMermaidString()).Contains("C4Context");
        await Assert.That(new PacketDiagram().AddField(0, 15, "Version").GetMermaidString()).Contains("packet-beta");
        await Assert.That(new Kanban().AddColumn(new KanbanColumn("todo", "Todo").AddTicket("item")).GetMermaidString()).Contains("kanban");
        await Assert.That(new Treemap().AddLeaf("Eng", 50).GetMermaidString()).Contains("treemap-beta");
        await Assert.That(new VennDiagram().AddSet("A", "Alpha").GetMermaidString()).Contains("venn-beta");
        var tree = new TreeView("Root");
        tree.AddChild("Child");
        await Assert.That(tree.GetMermaidString()).Contains("treeView");
        await Assert.That(new RequirementDiagram()
            .AddRequirement(new RequirementNode("req1", "1", "Must work"))
            .GetMermaidString()).Contains("requirementDiagram");
    }
}
