using Novolis.Markup.Html;

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

        return HtmlMarkup.Document(doc =>
        {
            doc.Lang("en").CharsetUtf8();
            if (!string.IsNullOrWhiteSpace(title))
            {
                doc.Title(title);
            }

            doc.WithHead(head => head.StyleSheet(css));
            doc.WithBody(body => body.Class(bodyClass).Raw(bodyHtml));
        }).ToString();
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

    /// <summary>Converts a fluent Markdown document to a complete themed HTML document.</summary>
    public static string FromDocument(
        IMarkdownDocument document,
        MarkdownHtmlTheme theme = MarkdownHtmlTheme.StudioDark,
        string? title = null)
    {
        ArgumentNullException.ThrowIfNull(document);
        return Wrap(MarkdownToHtmlConverter.Convert(document), theme, title);
    }
}
