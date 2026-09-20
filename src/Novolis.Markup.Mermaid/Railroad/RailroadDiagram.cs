namespace Novolis.Markup.Mermaid;

/// <summary>Grammar notation used by a railroad / syntax diagram.</summary>
public enum RailroadNotation
{
    /// <summary>Mermaid IR constructors (<c>railroad-beta</c>).</summary>
    Ir,

    /// <summary>W3C / ISO EBNF (<c>railroad-ebnf-beta</c>).</summary>
    Ebnf,

    /// <summary>RFC 5234 ABNF (<c>railroad-abnf-beta</c>).</summary>
    Abnf,

    /// <summary>Parsing Expression Grammar (<c>railroad-peg-beta</c>).</summary>
    Peg,
}

/// <summary>Fluent builder for Mermaid railroad (syntax) diagrams.</summary>
public sealed class RailroadDiagram : IMermaidable
{
    readonly List<string> _rules = [];

    /// <summary>Creates a railroad diagram in the given notation.</summary>
    public RailroadDiagram(RailroadNotation notation = RailroadNotation.Ebnf, string? title = null)
    {
        Notation = notation;
        Title = title;
    }

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <summary>Grammar notation / header keyword.</summary>
    public RailroadNotation Notation { get; }

    /// <summary>Optional title.</summary>
    public string? Title { get; }

    /// <summary>Adds a grammar rule. Include the trailing semicolon when the notation requires it.</summary>
    public RailroadDiagram Rule(string rule)
    {
        _rules.Add(rule.TrimEnd());
        return this;
    }

    /// <summary>Header token for this notation.</summary>
    public string HeaderToken => Notation switch
    {
        RailroadNotation.Ir => "railroad-beta",
        RailroadNotation.Abnf => "railroad-abnf-beta",
        RailroadNotation.Peg => "railroad-peg-beta",
        _ => "railroad-ebnf-beta",
    };

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        writer.WriteLine(HeaderToken);
        writer.IncreaseIndent();
        if (!string.IsNullOrWhiteSpace(Title))
            writer.WriteLine("title {0}", Quote(Title));
        foreach (var rule in _rules)
            writer.WriteLine(rule.EndsWith(';') ? rule : rule + " ;");
        writer.DecreaseIndent();
        return writer;
    }

    internal static RailroadNotation FromHeader(string header)
    {
        var token = header.Trim().Split(' ', 2)[0].ToLowerInvariant();
        return token switch
        {
            "railroad-beta" => RailroadNotation.Ir,
            "railroad-abnf-beta" => RailroadNotation.Abnf,
            "railroad-peg-beta" => RailroadNotation.Peg,
            _ => RailroadNotation.Ebnf,
        };
    }

    static string Quote(string value) =>
        value.Contains(' ') ? $"\"{value.Replace("\"", "#quot;", StringComparison.Ordinal)}\"" : value;
}
