namespace Novolis.Markup.Mermaid.Tests;

public class GitGraphTests
{
    [Test]
    public void Test1()
    {
        var graph = new GitGraph();
        var writer = graph.GetBuilder();
        writer.ToString().Should().NotBeNullOrEmpty();
    }
}
