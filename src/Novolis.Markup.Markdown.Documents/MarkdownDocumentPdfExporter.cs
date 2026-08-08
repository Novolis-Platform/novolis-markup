using Novolis.Documents;
using Novolis.Documents.Skia;
using Novolis.Markup.Markdown;

namespace Novolis.Markup.Markdown.Documents;

/// <summary>Exports Novolis Markdown to PDF via <see cref="DocumentPdf"/> (Skia).</summary>
public static class MarkdownDocumentPdfExporter
{
    /// <summary>Maps a fluent Markdown document and writes a PDF file.</summary>
    public static void ExportToFile(IMarkdownDocument document, string outputPath, MarkdownPagedExportOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);
        DocumentPdf.Write(MarkdownPagedDocumentMapper.FromDocument(document, options), outputPath);
    }

    /// <summary>Maps a fluent Markdown document and returns PDF bytes.</summary>
    public static byte[] ExportToBytes(IMarkdownDocument document, MarkdownPagedExportOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(document);
        return DocumentPdf.ToBytes(MarkdownPagedDocumentMapper.FromDocument(document, options));
    }

    /// <summary>
    /// Maps raw Markdown via <see cref="MarkdownDocument.Parse"/> and writes a PDF file.
    /// Prefer the <see cref="IMarkdownDocument"/> overloads for fluent documents.
    /// </summary>
    public static void ExportToFile(string markdown, string outputPath, MarkdownPagedExportOptions? options = null) =>
        ExportToFile(MarkdownDocument.Parse(markdown ?? string.Empty), outputPath, options);

    /// <summary>
    /// Maps raw Markdown via <see cref="MarkdownDocument.Parse"/> and returns PDF bytes.
    /// Prefer the <see cref="IMarkdownDocument"/> overloads for fluent documents.
    /// </summary>
    public static byte[] ExportToBytes(string markdown, MarkdownPagedExportOptions? options = null) =>
        ExportToBytes(MarkdownDocument.Parse(markdown ?? string.Empty), options);

    /// <summary>Builds a <see cref="PagedDocument"/> without writing PDF.</summary>
    public static PagedDocument ToPagedDocument(IMarkdownDocument document, MarkdownPagedExportOptions? options = null) =>
        MarkdownPagedDocumentMapper.FromDocument(document, options);

    /// <summary>Builds a <see cref="PagedDocument"/> from raw Markdown via <see cref="MarkdownDocument.Parse"/>.</summary>
    public static PagedDocument ToPagedDocument(string markdown, MarkdownPagedExportOptions? options = null) =>
        MarkdownPagedDocumentMapper.FromMarkdown(markdown, options);
}
