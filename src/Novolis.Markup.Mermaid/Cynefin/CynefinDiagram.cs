namespace Novolis.Markup.Mermaid;

/// <summary>Domains of the Cynefin framework.</summary>
public enum CynefinDomain
{
    /// <summary>Complex (probe–sense–respond).</summary>
    Complex,

    /// <summary>Complicated (sense–analyse–respond).</summary>
    Complicated,

    /// <summary>Clear (sense–categorise–respond).</summary>
    Clear,

    /// <summary>Chaotic (act–sense–respond).</summary>
    Chaotic,

    /// <summary>Confusion / disorder.</summary>
    Confusion,
}

/// <summary>Fluent builder for Mermaid <c>cynefin-beta</c> diagrams.</summary>
public sealed class CynefinDiagram : IMermaidable
{
    readonly Dictionary<CynefinDomain, List<string>> _items = new();
    readonly List<(CynefinDomain From, CynefinDomain To, string? Label)> _transitions = [];
    readonly string? _title;

    /// <summary>Creates a Cynefin diagram with an optional title.</summary>
    public CynefinDiagram(string? title = null)
    {
        _title = title;
        foreach (var domain in Enum.GetValues<CynefinDomain>())
            _items[domain] = [];
    }

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>Adds an item badge to a domain.</summary>
    public CynefinDiagram AddItem(CynefinDomain domain, string label)
    {
        _items[domain].Add(label);
        return this;
    }

    /// <summary>Adds a transition between two domains.</summary>
    public CynefinDiagram Transition(CynefinDomain from, CynefinDomain to, string? label = null)
    {
        _transitions.Add((from, to, label));
        return this;
    }

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine("cynefin-beta");
        writer.IncreaseIndent();
        if (!string.IsNullOrWhiteSpace(_title))
            writer.WriteLine("title {0}", _title);

        foreach (var domain in Enum.GetValues<CynefinDomain>())
        {
            writer.WriteLine(Token(domain));
            writer.IncreaseIndent();
            foreach (var item in _items[domain])
                writer.WriteLine("\"{0}\"", item.Replace("\"", "#quot;", StringComparison.Ordinal));
            writer.DecreaseIndent();
        }

        foreach (var (from, to, label) in _transitions)
        {
            if (string.IsNullOrWhiteSpace(label))
                writer.WriteLine("{0} --> {1}", Token(from), Token(to));
            else
                writer.WriteLine("{0} --> {1} : \"{2}\"", Token(from), Token(to), label.Replace("\"", "#quot;", StringComparison.Ordinal));
        }

        writer.DecreaseIndent();
        return writer;
    }

    internal static string Token(CynefinDomain domain) => domain switch
    {
        CynefinDomain.Complex => "complex",
        CynefinDomain.Complicated => "complicated",
        CynefinDomain.Clear => "clear",
        CynefinDomain.Chaotic => "chaotic",
        CynefinDomain.Confusion => "confusion",
        _ => throw new ArgumentOutOfRangeException(nameof(domain), domain, null),
    };

    internal static bool TryParseDomain(string token, out CynefinDomain domain)
    {
        domain = token.ToLowerInvariant() switch
        {
            "complex" => CynefinDomain.Complex,
            "complicated" => CynefinDomain.Complicated,
            "clear" => CynefinDomain.Clear,
            "chaotic" => CynefinDomain.Chaotic,
            "confusion" => CynefinDomain.Confusion,
            _ => (CynefinDomain)(-1),
        };
        return Enum.IsDefined(domain);
    }
}
