using Markdig;

namespace Novolis.Markup.Markdown.Rendering;

/// <summary>Converts raw Markdown source to HTML fragments via Markdig.</summary>
public static class MarkdigMarkdownRenderer
{
    /// <summary>Converts Markdown to an HTML body fragment (no document wrapper).</summary>
    /// <param name="markdown">Markdown source text.</param>
    /// <param name="pipeline">Optional Markdig pipeline; uses <see cref="MarkdownRenderPipeline.Default"/> when null.</param>
    /// <returns>HTML fragment suitable for wrapping in a full document.</returns>
    public static string ToHtml(string markdown, MarkdownPipeline? pipeline = null)
    {
        if (string.IsNullOrEmpty(markdown))
            return "<p></p>";

        return Markdig.Markdown.ToHtml(markdown, pipeline ?? MarkdownRenderPipeline.Default);
    }
}
