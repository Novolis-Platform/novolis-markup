namespace Novolis.Markup.Mermaid;

/// <summary>
/// Lossless Mermaid source holder used when a typed builder is unavailable or as the parse fallback.
/// Emits <see cref="Source"/> unchanged (aside from a trailing newline).
/// </summary>
public sealed class RawMermaidDiagram : IMermaidable
{
    /// <summary>Creates a raw diagram from already-detected kind and source body (including the header line).</summary>
    public RawMermaidDiagram(MermaidDiagramKind kind, string source)
    {
        Kind = kind;
        Source = source ?? throw new ArgumentNullException(nameof(source));
    }

    /// <inheritdoc />
    public Hash Id { get; } = Hash.NewHash();

    /// <inheritdoc />
    public MermaidDiagramKind Kind { get; }

    /// <summary>Full Mermaid source for this diagram, without YAML front matter.</summary>
    public string Source { get; }

    /// <inheritdoc />
    public IIndentedStringBuilder GetBuilder()
    {
        var writer = new IndentedStringBuilder();
        var text = Source.Replace("\r\n", "\n", StringComparison.Ordinal).TrimEnd();
        foreach (var line in text.Split('\n'))
            writer.WriteLine(line);
        return writer;
    }
}
