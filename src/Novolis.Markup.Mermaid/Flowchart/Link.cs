namespace Novolis.Markup.Mermaid;

/// <summary>A directed edge between two Mermaid flowchart nodes or subgraphs.</summary>
public class Link : IMermaidable, IComparable<Link>, IEquatable<Link>
{
    /// <summary>Gets the optional link label.</summary>
    public string? Label { get; }

    /// <summary>Gets the target node or subgraph identifier.</summary>
    public string Target { get; }

    /// <summary>Gets the source node or subgraph identifier.</summary>
    public string Source { get; }

    /// <summary>Gets the line style used when rendering this link.</summary>
    public Line Line { get; private set; } = new Line(LineStyle.NormalWithArrow, 3);

    /// <summary>Creates a link between two diagram elements.</summary>
    /// <param name="source">The source node or subgraph.</param>
    /// <param name="target">The target node or subgraph.</param>
    /// <param name="label">An optional edge label.</param>
    public Link(IMermaidable source, IMermaidable target, string? label = null)
        : this(IdOf(source, nameof(source)), IdOf(target, nameof(target)), label)
    {
    }

    /// <summary>Creates a link between two Mermaid node ids.</summary>
    public Link(string source, string target, string? label = null)
    {
        Source = source;
        Target = target;
        Label = label;
    }

    static string IdOf(IMermaidable element, string paramName) => element switch
    {
        Node node => node.NodeId,
        Subgraph subgraph => subgraph.Label,
        _ => throw new ArgumentException($"{paramName} must be a Node or Subgraph", paramName),
    };

    /// <summary>Assigns the line style for this link.</summary>
    /// <param name="line">The line style configuration.</param>
    public void SetLineStyle(Line line)
    {
        Line = line;
    }

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();

        writer.Write("{0} {1}{2} {3}", Source, Line.GetBuilder(), GetLabel(), Target);

        writer.WriteLine();
        return writer;
    }

    private string GetLabel() => !string.IsNullOrWhiteSpace(Label) ? $"|{Label}|" : string.Empty;

    /// <inheritdoc />
    public int CompareTo(Link? other)
    {
        if (other == null) return 1;
        if (ReferenceEquals(this, other)) return 0;
        if (Id == other.Id) return 0;
        if (Source == other.Source && Target == other.Target) return 0;
        return string.CompareOrdinal(Id.ToString(), other.Id.ToString());
    }

    /// <inheritdoc />
    public bool Equals(Link? other)
    {
        if (other == null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id && Source == other.Source && Target == other.Target;
    }
}
