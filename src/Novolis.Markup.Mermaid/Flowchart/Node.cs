namespace Novolis.Markup.Mermaid;

/// <summary>Represents Node.</summary>
public class Node(Hash id, string label, Shape shape) : IMermaidable
{
    /// <summary>Node operation.</summary>
    public Node(string label) : this(Hash.NewHash(), label, Shape.Rectangle)
    {
    }
    /// <summary>Node operation.</summary>
    
    public Node(string label, Shape shape) : this(Hash.NewHash(), label, shape)
    {
    }

    /// <inheritdoc />
    public Hash Id { get; } = id;
/// <summary>Shape.</summary>

    public string Label { get; } = label;
    /// <summary>Shape.</summary>
    public Shape Shape { get; } = shape;

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var label = Shape switch
        {
            Shape.Circle => $"(({Label}))",
            Shape.Subroutine => $"[[{Label}]]",
            Shape.Rounded => $"({Label})",
            Shape.Hexagon => $"{{{{{Label}}}}}",
            Shape.Database => $"[({Label})]",
            Shape.Rectangle => $"[{Label}]", // Default case
            Shape.Diamond => $"{{{Label}}}",
            Shape.DoubleCircle => $"((({Label})))",
            
            _ => $"[{Label}]" // Default case
        };
        
        var writer = new IndentedStringBuilder();
        writer.WriteLine("{0}{1}", this.GetId(), label);
        return writer;
    }
    
}
