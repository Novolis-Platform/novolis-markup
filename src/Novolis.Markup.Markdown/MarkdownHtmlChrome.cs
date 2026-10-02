using Novolis.Markup.Html;

namespace Novolis.Markup.Markdown;

/// <summary>CSP-safe copy and fullscreen chrome around code, mermaid, and images.</summary>
public static class MarkdownHtmlChrome
{
    /// <summary>Wraps a fenced code block with a Copy control.</summary>
    public static HtmlElement CodeBlock(string code, string? language, MarkdownHtmlActionSink actions)
    {
        ArgumentNullException.ThrowIfNull(actions);
        var href = actions.AddCopy(code);
        return HtmlMarkup.Div(div => div
            .Class("code-block")
            .Child(HtmlMarkup.Div(bar => bar
                .Class("code-block-bar")
                .Child(HtmlMarkup.Span(string.IsNullOrWhiteSpace(language) ? "code" : language).Class("code-block-lang"))
                .Child(HtmlMarkup.A(href, "Copy").Class("code-block-copy"))))
            .Child(HtmlMarkup.PreCode(code, language)));
    }

    /// <summary>Wraps an embedded image with a fullscreen control.</summary>
    public static HtmlElement Media(string dataUri, string caption, MarkdownHtmlActionSink actions, string extraClass)
    {
        ArgumentNullException.ThrowIfNull(actions);
        var href = actions.AddPreview(dataUri, caption);
        return HtmlMarkup.Div(div => div
            .Class("media-preview", extraClass)
            .Child(HtmlMarkup.A(href, "Fullscreen").Class("media-preview-open"))
            .Img(dataUri, caption));
    }
}
