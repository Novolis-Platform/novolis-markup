namespace Novolis.Markup.Mermaid;

/// <summary>Represents PieChart.</summary>
public class PieChart(string title, bool showData = true) : IMermaidable
{
    /// <summary>ShowData.</summary>
    public bool ShowData { get; } = showData;
/// <summary>Values.</summary>

    public string Title { get; } = title;

    /// <summary>Adds an item.</summary>
    public List<ChartValue> Values { get; } = new();
    /// <summary>Adds an item.</summary>
    
    public void AddValue(string name, double value) => Values.Add(new ChartValue(name, value));
    /// <summary>Adds an item.</summary>
    
    public void AddValue(ChartValue value) => Values.Add(value);
    
    /// <summary>Adds an item.</summary>
    public void AddValues(IEnumerable<ChartValue> values) => Values.AddRange(values);
    
    /// <summary>Adds an item.</summary>
    public void AddValues(params KeyValuePair<string, double>[] values) => Values.AddRange(values.Select(pair => new ChartValue(pair.Key, pair.Value)));

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.Write("pie {0}", ShowData ? "showData\n" : string.Empty);
        writer.WriteLine("title {0}", Title);
        writer.IncreaseIndent();
        foreach (var value in Values)
        {
            writer.WriteLine("\"{0}\" : {1}", value.Name, value.Value);
        }
        writer.DecreaseIndent();
        return writer;
    }
    
    /*
    pie showData
    title Key elements in Product X
    "Calcium" : 42.96
    "Potassium" : 50.05
    "Magnesium" : 10.01
    "Iron" :  5
     */
}
