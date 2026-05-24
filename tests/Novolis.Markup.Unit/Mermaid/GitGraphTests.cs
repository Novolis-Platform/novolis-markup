namespace Novolis.Markup.Mermaid.Tests;

public class GitGraphTests
{
    [Test]
    public async Task Test1()
    {
        var graph = new GitGraph();
        var writer = graph.GetBuilder();
        await Assert.That(writer.ToString()).IsNotNullOrWhiteSpace();
    }
}
