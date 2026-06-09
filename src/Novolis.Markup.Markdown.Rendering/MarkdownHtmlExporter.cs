using System.Text;

namespace Novolis.Markup.Markdown.Rendering;

/// <summary>Exports Markdown source to standalone HTML files.</summary>
public static class MarkdownHtmlExporter
{
    /// <summary>Exports Markdown to a complete HTML file.</summary>
    /// <param name="markdown">Markdown source.</param>
    /// <param name="outputPath">Destination file path.</param>
    /// <param name="theme">HTML theme.</param>
    /// <param name="title">Optional document title.</param>
    /// <param name="encoding">Text encoding; UTF-8 without BOM by default.</param>
    public static void ExportToFile(
        string markdown,
        string outputPath,
        MarkdownHtmlTheme theme = MarkdownHtmlTheme.GitHubLight,
        string? title = null,
        Encoding? encoding = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);

        var html = MarkdownHtmlDocument.FromMarkdown(markdown, theme, title);
        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllText(outputPath, html, encoding ?? new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
    }
}
