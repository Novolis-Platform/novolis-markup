namespace Novolis.Markup.Mermaid.Tests;

public class PieChartTests
{
    [Test]
    public async Task Test1()
    {
        var chart = new PieChart("Chart 1");
        chart.AddValue("Slice 1", 10);
        chart.AddValue("Slice 2", 20);
        chart.AddValue("Slice 3", 30);
        
        var writer = chart.GetBuilder();
        var result = writer.ToString();
    }
    
    [Test]
    public async Task Test2()
    {
        var chart = new PieChart("Chart 1", false);
        chart.AddValue("Slice 1", 10);
        chart.AddValue("Slice 2", 20);
        chart.AddValue("Slice 3", 30);
        
        var writer = chart.GetBuilder();
        var result = writer.ToString();
    }
    
    [Test]
    public async Task Test3()
    {
        var mermaidPieChart = new PieChart("MyPieChart");
        mermaidPieChart.AddValue("A", 999);
        mermaidPieChart.AddValue("B", 666);
        mermaidPieChart.AddValue("C", 420);
        mermaidPieChart.AddValue("D", 69);

        await Assert.That(mermaidPieChart.GetBuilder().ToString()).IsNotNullOrWhiteSpace();
    }
}