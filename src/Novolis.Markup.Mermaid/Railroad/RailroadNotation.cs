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
