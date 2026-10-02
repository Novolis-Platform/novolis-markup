using Novolis.Markup.Markdown;

namespace Novolis.Markup.Markdown.Mermaid.Rendering;

/// <summary>Themed Markdown HTML plus host payloads for copy and fullscreen preview.</summary>
public sealed record MarkdownHtmlDocumentBuild(string Document, MarkdownHtmlActionSink Actions);
