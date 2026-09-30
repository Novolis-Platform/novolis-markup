using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Novolis.Markup.Mermaid;

/// <summary>
/// Parsed Mermaid document: optional YAML front matter plus a typed (or raw) diagram builder.
/// Canonical serialization is Mermaid source; JSON is a lossless envelope around that source.
/// </summary>
public sealed class MermaidDocument
{
    MermaidDocument(MermaidFrontMatter frontMatter, MermaidDiagramKind kind, IMermaidable diagram)
    {
        FrontMatter = frontMatter;
        Kind = kind;
        Diagram = diagram;
    }

    /// <summary>YAML front matter (may be empty).</summary>
    public MermaidFrontMatter FrontMatter { get; }

    /// <summary>Detected diagram family.</summary>
    public MermaidDiagramKind Kind { get; }

    /// <summary>Typed builder when reconstruction succeeded; otherwise <see cref="RawMermaidDiagram"/>.</summary>
    public IMermaidable Diagram { get; }

    /// <summary>Emits front matter (when present) plus diagram source.</summary>
    public string ToMermaidString()
    {
        var body = Diagram.GetMermaidString().Replace("\r\n", "\n", StringComparison.Ordinal).TrimEnd();
        var yaml = FrontMatter.ToYamlBlock();
        return string.IsNullOrEmpty(yaml) ? body + "\n" : yaml + body + "\n";
    }

    /// <summary>Wraps an existing builder.</summary>
    public static MermaidDocument From(IMermaidable diagram, MermaidFrontMatter? frontMatter = null)
    {
        ArgumentNullException.ThrowIfNull(diagram);
        if (!MermaidKinds.TryOf(diagram, out var kind))
            throw new ArgumentException("The value is not a root Mermaid diagram builder.", nameof(diagram));
        return new MermaidDocument(frontMatter ?? new MermaidFrontMatter(), kind, diagram);
    }

    /// <summary>Parses Mermaid source into a document. Throws when the header cannot be recognized.</summary>
    public static MermaidDocument Parse(string source)
    {
        if (!TryParse(source, out var document))
            throw new FormatException("Unrecognized Mermaid diagram source.");
        return document;
    }

    /// <summary>Tries to parse Mermaid source. Returns <see langword="false"/> when the diagram kind cannot be detected.</summary>
    public static bool TryParse(string? source, [NotNullWhen(true)] out MermaidDocument? document)
    {
        document = null;
        if (string.IsNullOrWhiteSpace(source))
            return false;
        if (!MermaidSourceReader.TryRead(source, out var frontMatter, out var header, out var body))
            return false;
        var kind = MermaidKindDetector.FromHeader(header);
        if (kind is null)
            return false;

        var diagram = MermaidTypedParser.Parse(kind.Value, header, body)
                      ?? new RawMermaidDiagram(kind.Value, Reconstruct(header, body));
        document = new MermaidDocument(frontMatter, kind.Value, diagram);
        return true;
    }

    internal static string Reconstruct(string header, IReadOnlyList<string> body)
    {
        if (body.Count == 0)
            return header;
        return header + "\n" + string.Join('\n', body);
    }
}
