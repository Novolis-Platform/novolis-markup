namespace Novolis.Markup.Mermaid.Tests;

public sealed class FlowchartCoverageTests
{
    [Test]
    public async Task Flowchart_EmitsMermaidSyntax()
    {
        var flowchart = new Flowchart(Direction.LeftToRight);
        var a = new Node("A", Shape.Circle);
        var b = new Node("B", Shape.Diamond);
        flowchart.AddNode(a);
        flowchart.AddNode(b);
        flowchart.AddLink(new Link(a, b, "go"));

        var text = flowchart.GetBuilder().ToString();
        await Assert.That(text).Contains("flowchart LR");
        await Assert.That(text).Contains("A");
        await Assert.That(text).Contains("B");
        await Assert.That(text).Contains("-->");
    }

    [Test]
    public async Task Subgraph_NestedLinksRender()
    {
        var flowchart = new Flowchart();
        var subgraph = new Subgraph("Inner", Direction.TopToBottom);
        var n1 = new Node("One");
        var n2 = new Node("Two", Shape.Hexagon);
        subgraph.AddNode(n1);
        subgraph.AddNode(n2);
        subgraph.AddLink(new Link(n1, n2));
        flowchart.AddSubgraph(subgraph);

        var outside = new Node("Outside");
        flowchart.AddNode(outside);
        flowchart.AddLink(new Link(outside, n1, "enter"));

        var text = flowchart.GetBuilder().ToString();
        await Assert.That(text).Contains("subgraph");
        await Assert.That(text).Contains("Inner");
        await Assert.That(text).Contains("{{Two}}");
    }

    [Test]
    public async Task LinkStyles_AndDirections()
    {
        var flowchart = new Flowchart(Direction.BottomToTop);
        var x = new Node("X");
        var y = new Node("Y");
        flowchart.AddNode(x);
        flowchart.AddNode(y);
        var link = new Link(x, y, "up");
        link.SetLineStyle(new Line(LineStyle.DottedWithArrow, 3));
        flowchart.AddLink(link);

        var text = flowchart.GetBuilder().ToString();
        await Assert.That(text).Contains("BT");
        await Assert.That(text.Contains('.') && text.Contains('>')).IsTrue();
    }
}
