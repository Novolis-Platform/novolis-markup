namespace Novolis.Markup.Markdown.Rendering;

/// <summary>Converts raw Markdown source to HTML fragments via <see cref="MarkdownDocument.Parse"/>.</summary>
public static class NovolisMarkdownRenderer
{
    /// <summary>Converts Markdown to an HTML body fragment (no document wrapper).</summary>
    public static string ToHtml(string markdown)
    {
        if (string.IsNullOrEmpty(markdown))
            return "<p></p>";

        return MarkdownToHtmlConverter.Convert(MarkdownDocument.Parse(markdown));
    }
}
