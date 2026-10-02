namespace Novolis.Markup.Markdown;

/// <summary>A mermaid diagram or document image that can open in fullscreen preview.</summary>
public sealed record MarkdownPreviewAsset(string DataUri, string Caption);
