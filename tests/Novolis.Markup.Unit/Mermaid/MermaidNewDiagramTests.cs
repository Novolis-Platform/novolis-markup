namespace Novolis.Markup.Mermaid.Tests;

public sealed class MermaidNewDiagramTests
{
    [Test]
    public async Task Ishikawa_EmitsProblemAndCauses()
    {
        var diagram = new IshikawaDiagram("Blurry Photo");
        diagram.AddCause("Process").AddCause("Out of focus");
        diagram.AddCause("User").AddCause("Shaky hands");

        var text = diagram.GetMermaidString();
        await Assert.That(text).Contains("ishikawa-beta");
        await Assert.That(text).Contains("Blurry Photo");
        await Assert.That(text).Contains("Out of focus");
    }

    [Test]
    public async Task UseCase_EmitsActorsAndInclude()
    {
        var text = new UseCaseDiagram()
            .Direction(Direction.LeftToRight)
            .Actor("Customer")
            .SystemBoundary("Storefront")
            .UseCase("Checkout", "Place order")
            .UseCase("Browse", "Browse catalogue")
            .End()
            .Rel("Customer", "Checkout")
            .Rel("Checkout", "Browse", UseCaseRelation.Include)
            .GetMermaidString();

        await Assert.That(text).Contains("usecase-beta");
        await Assert.That(text).Contains("actor Customer");
        await Assert.That(text).Contains("systemBoundary");
        await Assert.That(text).Contains("..> : include Browse");
    }

    [Test]
    public async Task Wardley_EmitsComponentsAndLinks()
    {
        var text = new WardleyMap()
            .Title("Cup of tea")
            .Anchor("User", 0.95, 0.2)
            .Component("Tea", 0.6, 0.4)
            .Component("Kettle", 0.4, 0.7, "inertia")
            .Link("User", "Tea")
            .Evolve("Kettle", 0.85)
            .GetMermaidString();

        await Assert.That(text).Contains("wardley-beta");
        await Assert.That(text).Contains("component Tea");
        await Assert.That(text).Contains("User -> Tea");
        await Assert.That(text).Contains("evolve Kettle");
    }

    [Test]
    public async Task Cynefin_EmitsDomainsAndTransition()
    {
        var text = new CynefinDiagram("Incident")
            .AddItem(CynefinDomain.Complex, "Investigate")
            .AddItem(CynefinDomain.Clear, "Restart")
            .Transition(CynefinDomain.Complex, CynefinDomain.Complicated, "Pattern")
            .GetMermaidString();

        await Assert.That(text).Contains("cynefin-beta");
        await Assert.That(text).Contains("complex");
        await Assert.That(text).Contains("\"Investigate\"");
        await Assert.That(text).Contains("complex --> complicated");
    }

    [Test]
    public async Task Railroad_EmitsEbnfRule()
    {
        var text = new RailroadDiagram(RailroadNotation.Ebnf, "Digit")
            .Rule("digit = \"0\" | \"1\"")
            .GetMermaidString();

        await Assert.That(text).Contains("railroad-ebnf-beta");
        await Assert.That(text).Contains("title Digit");
        await Assert.That(text).Contains("digit = \"0\" | \"1\" ;");
    }

    [Test]
    public async Task EventModeling_EmitsTimeFrames()
    {
        var text = new EventModelingDiagram()
            .TimeFrame("01", EventModelingEntityType.Ui, "CartUI")
            .TimeFrame("02", EventModelingEntityType.Command, "AddItem")
            .TimeFrame("03", EventModelingEntityType.Event, "ItemAdded")
            .ResetFrame("10")
            .GetMermaidString();

        await Assert.That(text).Contains("eventmodeling");
        await Assert.That(text).Contains("tf 01 ui CartUI");
        await Assert.That(text).Contains("tf 03 evt ItemAdded");
        await Assert.That(text).Contains("rf 10");
    }

    [Test]
    public async Task Flowchart_NamedShapesAndStadium()
    {
        var a = Node.Named("Start", "Go", Shape.Stadium);
        var b = Node.NamedShape("Work", "docs", "Write");
        var chart = new Flowchart(Direction.LeftToRight);
        chart.AddNode(a);
        chart.AddNode(b);
        chart.AddLink(new Link(a, b, "next"));

        var text = chart.GetMermaidString();
        await Assert.That(text).Contains("Start([Go])");
        await Assert.That(text).Contains("Work@{ shape: docs, label: \"Write\" }");
        await Assert.That(text).Contains("|next|");
    }

    [Test]
    public async Task Sequence_AutonumberAndBox()
    {
        var text = new SequenceDiagram()
            .Autonumber()
            .Box("Client")
            .AddParticipant("A")
            .End()
            .Message("A", "B", "Hi")
            .GetMermaidString();

        await Assert.That(text).Contains("autonumber");
        await Assert.That(text).Contains("box Client");
    }

    [Test]
    public async Task GitGraph_FluentBranchMerge()
    {
        var text = new GitGraph()
            .Commit("Alpha")
            .Branch("develop")
            .Commit("Beta", CommitType.Highlight, "v1")
            .Checkout("main")
            .Merge("develop")
            .GetMermaidString();

        await Assert.That(text).Contains("commit id: \"Alpha\"");
        await Assert.That(text).Contains("branch develop");
        await Assert.That(text).Contains("type: HIGHLIGHT");
        await Assert.That(text).Contains("merge develop");
    }

    [Test]
    public async Task XyChart_BarAndLine()
    {
        var text = new XyChart("Sales")
            .WithXAxis(null, "jan", "feb")
            .WithYAxis("Revenue", 0, 100)
            .Bar("q1", 10, 20)
            .Line("trend", 12, 18)
            .GetMermaidString();

        await Assert.That(text).Contains("xychart");
        await Assert.That(text).Contains("bar \"q1\" [10, 20]");
        await Assert.That(text).Contains("line \"trend\" [12, 18]");
    }
}
