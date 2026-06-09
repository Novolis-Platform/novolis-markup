namespace Novolis.Markup.Markdown.Rendering;

/// <summary>Wraps HTML body fragments in a complete document with theme styles.</summary>
public static class MarkdownHtmlDocument
{
    /// <summary>Wraps a body HTML fragment in a full HTML document using the given theme.</summary>
    /// <param name="bodyHtml">HTML fragment from Markdig or another renderer.</param>
    /// <param name="theme">Visual theme for the document.</param>
    /// <param name="title">Optional document title.</param>
    /// <returns>Complete HTML document string.</returns>
    public static string Wrap(string bodyHtml, MarkdownHtmlTheme theme = MarkdownHtmlTheme.StudioDark, string? title = null)
    {
        var css = theme switch
        {
            MarkdownHtmlTheme.GitHubLight => GithubMarkdownCss.Default,
            MarkdownHtmlTheme.GitHubDark => GithubMarkdownCss.Default + GithubMarkdownCss.Other,
            _ => StudioMarkdownCss.Default,
        };

        var bodyClass = theme is MarkdownHtmlTheme.GitHubLight or MarkdownHtmlTheme.GitHubDark
            ? "markdown-body"
            : "markdown-body studio";

        var titleTag = string.IsNullOrWhiteSpace(title)
            ? string.Empty
            : $"<title>{System.Net.WebUtility.HtmlEncode(title)}</title>";

        return $"""
                <!DOCTYPE html>
                <html lang="en">
                <head>
                <meta charset="utf-8" />
                {titleTag}
                <style>{css}</style>
                </head>
                <body class="{bodyClass}">
                {bodyHtml}
                </body>
                </html>
                """;
    }

    /// <summary>Converts Markdown source to a complete themed HTML document.</summary>
    /// <param name="markdown">Markdown source.</param>
    /// <param name="theme">Visual theme.</param>
    /// <param name="title">Optional document title.</param>
    /// <returns>Complete HTML document.</returns>
    public static string FromMarkdown(string markdown, MarkdownHtmlTheme theme = MarkdownHtmlTheme.StudioDark, string? title = null)
    {
        var body = MarkdigMarkdownRenderer.ToHtml(markdown);
        return Wrap(body, theme, title);
    }
}
