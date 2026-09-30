using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Novolis.Markup.Mermaid;

/// <summary>JSON envelope for a <see cref="MermaidDocument"/> (kind + source + optional front matter).</summary>
public static class MermaidJson
{
    static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
    };

    /// <summary>Serializes a document to JSON.</summary>
    public static string Serialize(MermaidDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        var payload = new MermaidJsonPayload
        {
            Kind = document.Kind,
            Source = document.ToMermaidString(),
            Title = document.FrontMatter.Title,
            Theme = document.FrontMatter.Theme,
            Look = document.FrontMatter.Look,
            Layout = document.FrontMatter.Layout,
        };
        return JsonSerializer.Serialize(payload, Options);
    }

    /// <summary>Serializes a builder (no extra front matter unless the source already contains it).</summary>
    public static string Serialize(IMermaidable diagram) => Serialize(MermaidDocument.From(diagram));

    /// <summary>Deserializes a JSON envelope produced by <see cref="Serialize(MermaidDocument)"/>.</summary>
    public static MermaidDocument Deserialize(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);
        var payload = JsonSerializer.Deserialize<MermaidJsonPayload>(json, Options)
                      ?? throw new FormatException("Mermaid JSON payload was empty.");
        if (string.IsNullOrWhiteSpace(payload.Source))
            throw new FormatException("Mermaid JSON payload is missing source.");
        return MermaidDocument.Parse(payload.Source);
    }

    sealed class MermaidJsonPayload
    {
        public MermaidDiagramKind Kind { get; init; }
        public string Source { get; init; } = string.Empty;
        public string? Title { get; init; }
        public string? Theme { get; init; }
        public string? Look { get; init; }
        public string? Layout { get; init; }
    }
}
