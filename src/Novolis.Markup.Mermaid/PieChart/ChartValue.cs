namespace Novolis.Markup.Mermaid;

/// <summary>Represents ChartValue.</summary>
public class ChartValue(string name, double value)
{
    /// <summary>Name.</summary>
    public string Name { get; } = name;
    /// <summary>Value.</summary>
    public double Value { get; } = value;
}
