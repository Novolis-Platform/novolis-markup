namespace Novolis.Markup.Mermaid;

/// <summary>Represents Flowchart.</summary>
public class Flowchart(Direction direction = Direction.TopToBottom) : IMermaidable
{
    private readonly List<Node> _nodes = new();
    private readonly List<Link> _links = new();
    /// <summary>Adds an item.</summary>
    private readonly List<Subgraph> _subgraphs = new();

    /// <summary>Adds an item.</summary>
    public void AddSubgraph(Subgraph subgraph) => _subgraphs.Add(subgraph);
    /// <summary>Adds an item.</summary>
    public void AddSubgraphs(IEnumerable<Subgraph> subgraphs) => _subgraphs.AddRange(subgraphs);
    /// <summary>Adds an item.</summary>
    
    public void AddNode(Node node) => _nodes.Add(node);
    /// <summary>Adds an item.</summary>
    public void AddNodes(IEnumerable<Node> nodes) => _nodes.AddRange(nodes);
    /// <summary>Adds an item.</summary>
    
    public void AddLink(Link link) => _links.Add(link);
    /// <summary>Adds an item.</summary>
    public void AddLinks(IEnumerable<Link> links) => _links.AddRange(links);

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        // CleanDuplicates();
        var writer = new IndentedStringBuilder();
        writer.WriteLine("flowchart {0}", direction.GetBuilder());
        writer.IncreaseIndent();
        
        foreach (var node in _nodes)
        {
            writer.WriteLine(node.GetBuilder());
        }
        
        foreach (var link in _links)
        {
            writer.WriteLine(link.GetBuilder());
        }
        
        foreach (var subgraph in _subgraphs)
        {
            writer.WriteLine(subgraph.GetBuilder());
        }
        
        writer.DecreaseIndent();
        
        return writer;
    }
    
    private void CleanDuplicates()
    {
        var nodeIds = new HashSet<Hash>();
        var linkIds = new HashSet<Hash>();
        var subgraphIds = new HashSet<Hash>();
        
        _nodes.RemoveAll(node =>
        {
            if (!nodeIds.Add(node.Id))
            {
                return true;
            }

            return false;
        });
        
        _links.RemoveAll(link =>
        {
            if (linkIds.Contains(link.Id))
            {
                return true;
            }
            linkIds.Add(link.Id);
            return false;
        });
        
        _subgraphs.RemoveAll(subgraph =>
        {
            if (subgraphIds.Contains(subgraph.Id))
            {
                return true;
            }
            subgraphIds.Add(subgraph.Id);
            return false;
        });
    }
}
