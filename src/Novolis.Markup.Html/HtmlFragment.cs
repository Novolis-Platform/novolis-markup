namespace Novolis.Markup.Html;

/// <summary>Ordered collection of nodes without a wrapper element.</summary>
public sealed class HtmlFragment : IHtmlNode
{
    private readonly List<IHtmlNode> _nodes = new();

    /// <summary>Child nodes.</summary>
    public IReadOnlyList<IHtmlNode> Nodes => _nodes;

    /// <summary>Appends a node when non-null.</summary>
    public HtmlFragment Child(IHtmlNode? node)
    {
        if (node is not null)
        {
            _nodes.Add(node);
        }

        return this;
    }

    /// <summary>Appends nodes.</summary>
    public HtmlFragment Add(params IHtmlNode[] nodes)
    {
        foreach (var node in nodes)
        {
            Child(node);
        }

        return this;
    }

    /// <summary>Appends escaped text.</summary>
    public HtmlFragment Text(string text) => Child(new HtmlText(text));

    /// <summary>Appends raw markup.</summary>
    public HtmlFragment Raw(string markup) => Child(new HtmlRaw(markup));

    /// <inheritdoc />
    public void WriteTo(TextWriter writer)
    {
        foreach (var node in _nodes)
        {
            node.WriteTo(writer);
        }
    }

    /// <inheritdoc />
    public override string ToString()
    {
        using var writer = new StringWriter();
        WriteTo(writer);
        return writer.ToString();
    }
}
