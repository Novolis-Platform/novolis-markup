using Novolis.Markup.Html;

// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

/// <summary>Allows callers to render selected Markdown sections before the built-in HTML conversion handles them.</summary>
public delegate IHtmlNode? MarkdownHtmlSectionRenderer(IMarkdownSection section);
