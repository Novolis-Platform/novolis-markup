namespace Novolis.Markup.Mermaid.Tests;

public sealed class MermaidExtendedTests
{
    [Test]
    public async Task Flowchart_EmitsNodesLinksAndSubgraph()
    {
        var start = new Node("Start");
        var end = new Node("End");
        var chart = new Flowchart(Direction.LeftToRight);
        chart.AddNode(start);
        chart.AddNode(end);
        chart.AddLink(new Link(start, end, "go"));
        var subgraph = new Subgraph("group", Direction.TopToBottom);
        subgraph.AddNode(new Node("Inner"));
        chart.AddSubgraph(subgraph);

        var text = chart.GetMermaidString();
        await Assert.That(text).Contains("flowchart LR");
        await Assert.That(text).Contains("Start");
        await Assert.That(text).Contains("subgraph group");
    }

    [Test]
    public async Task C4Diagram_EmitsPersonAndSystem()
    {
        var text = new C4Diagram(C4Kind.Context, "Overview")
            .Person("user", "User")
            .System("core", "Core")
            .Rel("user", "core", "Uses")
            .GetMermaidString();

        await Assert.That(text).Contains("C4Context");
        await Assert.That(text).Contains("Person(user");
        await Assert.That(text).Contains("System(core");
    }

    [Test]
    public async Task ArchitectureDiagram_EmitsServiceEdges()
    {
        var text = new ArchitectureDiagram()
            .Service("api", "API")
            .Service("db", "Database")
            .Edge("api", "db", "reads")
            .GetMermaidString();

        await Assert.That(text).Contains("architecture-beta");
        await Assert.That(text).Contains("service api");
        await Assert.That(text).Contains("api:db");
    }

    [Test]
    public async Task GitGraph_EmitsCommits()
    {
        var graph = new GitGraph();
        graph.AddCommit(new Commit("init", "main"));
        graph.AddCommit(new Commit("work", "feature"));
        var text = graph.GetMermaidString();

        await Assert.That(text).Contains("gitGraph");
        await Assert.That(text).Contains("message init");
        await Assert.That(text).Contains("branch feature");
    }

    [Test]
    public async Task QuadrantChart_EmitsPoints()
    {
        var text = new QuadrantChart("Strategy")
            .WithAxes("Reach", "Impact")
            .AddPoint("A", 0.2, 0.8)
            .GetMermaidString();

        await Assert.That(text).Contains("quadrantChart");
        await Assert.That(text).Contains("A:");
        await Assert.That(text).Contains("0,8");
    }

    [Test]
    public async Task RequirementDiagram_EmitsNodeAndLink()
    {
        var text = new RequirementDiagram()
            .AddRequirement(new RequirementNode("R1", "1", "Must work"))
            .AddElement(new RequirementElement("E1", "module"))
            .AddRelation(new RequirementRelation("R1", RequirementRelationType.Satisfies, "E1"))
            .GetMermaidString();

        await Assert.That(text).Contains("requirementDiagram");
        await Assert.That(text).Contains("requirement R1");
        await Assert.That(text).Contains("element E1");
    }
}
