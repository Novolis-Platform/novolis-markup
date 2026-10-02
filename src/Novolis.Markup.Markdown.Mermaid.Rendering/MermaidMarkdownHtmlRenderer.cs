using System.Text;
using Novolis.Markup.Html;
using Novolis.Markup.Markdown;
using Novolis.Markup.Markdown.Rendering;
using Novolis.Markup.Mermaid.Rendering;

namespace Novolis.Markup.Markdown.Mermaid.Rendering;

/// <summary>Renders fenced <c>mermaid</c> code blocks as headless SVG images.</summary>
public static class MermaidMarkdownHtmlRenderer
{
    /// <summary>Converts Markdown to an HTML body fragment with Mermaid diagrams rendered to SVG.</summary>
    public static string ToHtml(
        string markdown,
        MermaidRenderTheme theme = MermaidRenderTheme.StudioDark,
        MarkdownHtmlActionSink? actions = null,
        bool renderMermaid = true)
    {
        if (string.IsNullOrEmpty(markdown))
            return "<p></p>";

        return MarkdownToHtmlConverter.Convert(
            MarkdownDocument.Parse(markdown),
            renderMermaid ? section => RenderSection(section, theme, actions) : null,
            actions);
    }

    /// <summary>Creates a full themed HTML document with Mermaid diagrams rendered to SVG.</summary>
    public static string ToHtmlDocument(
        string markdown,
        MarkdownHtmlTheme htmlTheme = MarkdownHtmlTheme.StudioDark,
        MermaidRenderTheme mermaidTheme = MermaidRenderTheme.StudioDark,
        string? title = null) =>
        Build(markdown, htmlTheme, mermaidTheme, title).Document;

    /// <summary>Creates themed HTML plus copy/preview payloads for a host viewer.</summary>
    public static MarkdownHtmlDocumentBuild Build(
        string markdown,
        MarkdownHtmlTheme htmlTheme = MarkdownHtmlTheme.StudioDark,
        MermaidRenderTheme mermaidTheme = MermaidRenderTheme.StudioDark,
        string? title = null,
        string? sourceDirectory = null,
        bool renderMermaid = true)
    {
        var actions = new MarkdownHtmlActionSink { SourceDirectory = sourceDirectory };
        var body = ToHtml(markdown, mermaidTheme, actions, renderMermaid);
        return new MarkdownHtmlDocumentBuild(MarkdownHtmlDocument.Wrap(body, htmlTheme, title), actions);
    }

    private static IHtmlNode? RenderSection(
        IMarkdownSection section,
        MermaidRenderTheme theme,
        MarkdownHtmlActionSink? actions)
    {
        if (section is not IMarkdownCodeBlock code
            || !string.Equals(code.Language, "mermaid", StringComparison.OrdinalIgnoreCase))
            return null;

        var svg = MermaidSvgRenderer.TryRenderSvg(code.Code, theme);
        if (svg is null)
            return null;

        var bytes = Encoding.UTF8.GetBytes(svg);
        var source = $"data:image/svg+xml;base64,{Convert.ToBase64String(bytes)}";
        if (actions is not null)
            return MarkdownHtmlChrome.Media(source, "Mermaid diagram", actions, "mermaid-diagram");

        return HtmlMarkup.Div(div => div
            .Class("mermaid-diagram")
            .Img(source, "Mermaid diagram"));
    }
}
