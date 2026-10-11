using Novolis.Documents;
using Novolis.Markup.Markdown;

namespace Novolis.Markup.Markdown.Documents;

/// <summary>Maps Markdown into the semantic <see cref="Document"/> model.</summary>
public static class MarkdownDocumentMapper
{
    /// <summary>Maps a parsed Markdown document.</summary>
    public static Document FromDocument(
        IMarkdownDocument document,
        MarkdownPagedExportOptions? options = null) =>
        MarkdownPagedDocumentMapper.FromDocument(document, options);

    /// <summary>Parses and maps Markdown text.</summary>
    public static Document FromMarkdown(
        string markdown,
        MarkdownPagedExportOptions? options = null) =>
        MarkdownPagedDocumentMapper.FromMarkdown(markdown, options);
}
