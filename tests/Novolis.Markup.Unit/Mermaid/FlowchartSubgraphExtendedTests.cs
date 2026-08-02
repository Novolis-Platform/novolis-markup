namespace Novolis.Markup.Mermaid.Tests;

public sealed class FlowchartSubgraphExtendedTests
{
    [Test]
    public async Task NestedSubgraph_RendersDirectionAndEnd()
    {
        var outer = new Subgraph("Outer", Direction.LeftToRight);
        var inner = new Subgraph("Inner", Direction.TopToBottom);
        var a = new Node("A");
        var b = new Node("B", Shape.Rounded);
        inner.AddNode(a);
        inner.AddNode(b);
        inner.AddLink(new Link(a, b, "inner"));
        outer.AddSubgraph(inner);
        outer.AddNode(new Node("Gate", Shape.Diamond));

        var text = outer.GetBuilder().ToString();
        await Assert.That(text).Contains("subgraph Outer");
        await Assert.That(text).Contains("subgraph Inner");
        await Assert.That(text).Contains("direction TB");
        await Assert.That(text).Contains("direction LR");
        await Assert.That(text).Contains("(B)");
        await Assert.That(text.Split("end", StringSplitOptions.None).Length - 1).IsEqualTo(2);
    }

    [Test]
    public async Task Flowchart_LinksSubgraphToSubgraph_AndBatchAdds()
    {
        var flowchart = new Flowchart(Direction.RightToLeft);
        var sg1 = new Subgraph("Alpha", Direction.TopToBottom);
        var sg2 = new Subgraph("Beta", Direction.BottomToTop);
        var n1 = new Node("One");
        var n2 = new Node("Two");
        sg1.AddNodes([n1]);
        sg2.AddNodes([n2]);
        flowchart.AddSubgraphs([sg1, sg2]);
        flowchart.AddLinks([new Link(sg1, sg2, "handoff")]);

        var text = flowchart.GetBuilder().ToString();
        await Assert.That(text).Contains("flowchart RL");
        await Assert.That(text).Contains("Alpha");
        await Assert.That(text).Contains("Beta");
        await Assert.That(text).Contains("handoff");
    }

    [Test]
    public async Task Subgraph_CleanDuplicates_RemovesRepeatedNodes()
    {
        var subgraph = new Subgraph("Dup", Direction.TopToBottom);
        var shared = new Node("Shared");
        subgraph.AddNode(shared);
        subgraph.AddNode(shared);
        subgraph.AddLink(new Link(shared, shared));

        var text = subgraph.GetBuilder().ToString();
        await Assert.That(text.Split("Shared", StringSplitOptions.None).Length - 1).IsEqualTo(1);
    }

    [Test]
    public async Task Flowchart_AllNodeShapes_RenderDistinctMarkers()
    {
        var flowchart = new Flowchart();
        flowchart.AddNodes(
        [
            new Node("Rect"),
            new Node("Round", Shape.Rounded),
            new Node("Sub", Shape.Subroutine),
            new Node("Db", Shape.Database),
            new Node("Dbl", Shape.DoubleCircle),
        ]);

        var text = flowchart.GetBuilder().ToString();
        await Assert.That(text).Contains("[Rect]");
        await Assert.That(text).Contains("(Round)");
        await Assert.That(text).Contains("[[Sub]]");
        await Assert.That(text).Contains("[(Db)]");
        await Assert.That(text).Contains("(((Dbl)))");
    }
}
