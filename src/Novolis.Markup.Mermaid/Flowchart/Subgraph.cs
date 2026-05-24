namespace Novolis.Markup.Mermaid;
// Define the line styles using an enumeration

/// <summary>Represents Subgraph.</summary>
public class Subgraph(string label, Direction direction) : IMermaidable
{
    /// <summary>Adds an item.</summary>
    public void AddNode(Node node) => Nodes.Add(node);
    /// <summary>Adds an item.</summary>
    public void AddNodes(IEnumerable<Node> nodes) => Nodes.AddRange(nodes);
/// <summary>Adds an item.</summary>

    public void AddLink(Link link) => Links.Add(link);
    /// <summary>Adds an item.</summary>
    public void AddLinks(IEnumerable<Link> links) => Links.AddRange(links);
/// <summary>Adds an item.</summary>

    public void AddSubgraph(Subgraph subgraph) => _subgraphs.Add(subgraph);
    /// <summary>Label.</summary>
    public void AddSubgraphs(IEnumerable<Subgraph> subgraphs) => _subgraphs.AddRange(subgraphs);

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();
    
    /// <summary>Direction.</summary>
    public string Label { get; } = label.CleanNonAlphanumeric();
/// <summary>Nodes.</summary>

    public Direction Direction { get; } = direction;
/// <summary>Nodes.</summary>

    public List<Node> Nodes { get; } = new();
    
    private List<Link> Links { get; } = new();
    
    
    
    private readonly List<Subgraph> _subgraphs = new();
    
    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        CleanDuplicates();
        var writer = new IndentedStringBuilder();
        writer.WriteLine("subgraph {0}", Label);
        writer.IncreaseIndent();
        
        writer.WriteLine("direction {0}", Direction.GetBuilder());
        
        foreach (var node in Nodes)
        {
            writer.WriteLine(node.GetBuilder());
        }

        foreach (var link in Links)
        {
            writer.WriteLine(link.GetBuilder());
        }

        foreach (var subgraph in _subgraphs)
        {
            writer.WriteLine(subgraph.GetBuilder());
        }
        

        writer.DecreaseIndent();
        writer.WriteLine("end");
        return writer;
    }
    
    private void CleanDuplicates()
    {
        var nodeIds = new HashSet<Hash>();
        var linkIds = new HashSet<Hash>();
        var subgraphIds = new HashSet<Hash>();
        
        Nodes.RemoveAll(node =>
        {
            if (!nodeIds.Add(node.Id))
            {
                return true;
            }
            return false;
        });
        
        Links.RemoveAll(link =>
        {
            if (!linkIds.Add(link.Id))
            {
                return true;
            }
            return false;
        });
        
        _subgraphs.RemoveAll(subgraph =>
        {
            if (!subgraphIds.Add(subgraph.Id))
            {
                return true;
            }
            return false;
        });
    }
}
