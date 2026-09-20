namespace Novolis.Markup.Mermaid;

/// <summary>A flowchart node with a shape and optional explicit Mermaid id.</summary>
public class Node : IMermaidable
{
    /// <summary>Creates a rectangle node whose id is a generated <see cref="Hash"/>.</summary>
    public Node(string label) : this(Hash.NewHash(), label, Shape.Rectangle)
    {
    }

    /// <summary>Creates a node with a generated id and the given shape.</summary>
    public Node(string label, Shape shape) : this(Hash.NewHash(), label, shape)
    {
    }

    /// <summary>Creates a node with an explicit hash id (also used as the Mermaid node id).</summary>
    public Node(Hash id, string label, Shape shape)
    {
        Id = id;
        NodeId = id.ToString();
        Label = label;
        Shape = shape;
    }

    /// <summary>Creates a node with a caller-chosen Mermaid id (e.g. <c>Start</c>).</summary>
    public static Node Named(string nodeId, string label, Shape shape = Shape.Rectangle) =>
        new(Hash.NewHash(), label, shape) { NodeId = nodeId };

    /// <summary>Creates a node using Mermaid's named-shape syntax (<c>A@{ shape: hex, label: "Hi" }</c>).</summary>
    public static Node NamedShape(string nodeId, string shapeName, string label) =>
        new(Hash.NewHash(), label, Shape.Rectangle) { NodeId = nodeId, ShapeName = shapeName };

    /// <inheritdoc />
    public Hash Id { get; }

    /// <summary>Identifier written into Mermaid source.</summary>
    public string NodeId { get; init; }

    /// <summary>Display label.</summary>
    public string Label { get; }

    /// <summary>Delimiter shape used when <see cref="ShapeName"/> is unset.</summary>
    public Shape Shape { get; }

    /// <summary>Optional named shape (<c>hex</c>, <c>docs</c>, <c>notch-rect</c>, …).</summary>
    public string? ShapeName { get; init; }

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        if (!string.IsNullOrWhiteSpace(ShapeName))
        {
            writer.WriteLine("{0}@{{ shape: {1}, label: \"{2}\" }}", NodeId, ShapeName, Label);
            return writer;
        }

        writer.WriteLine("{0}{1}", NodeId, Shape.Wrap(Label));
        return writer;
    }
}
