using System.Text;
using Novolis.Markup.Html;
using Novolis.Markup.Markdown.Rendering;
using Novolis.Markup.Mermaid.Rendering;

namespace Novolis.Markup.Markdown.Mermaid.Rendering;

/// <summary>Renders fenced <c>mermaid</c> code blocks as headless SVG images.</summary>
public static class MermaidMarkdownHtmlRenderer
{
    /// <summary>Converts Markdown to an HTML body fragment with Mermaid diagrams rendered to SVG.</summary>
    public static string ToHtml(string markdown, MermaidRenderTheme theme = MermaidRenderTheme.StudioDark)
    {
        if (string.IsNullOrEmpty(markdown))
            return "<p></p>";

        return MarkdownToHtmlConverter.Convert(
            MarkdownDocument.Parse(markdown),
            section => RenderSection(section, theme));
    }

    /// <summary>Creates a full themed HTML document with Mermaid diagrams rendered to SVG.</summary>
    public static string ToHtmlDocument(
        string markdown,
        MarkdownHtmlTheme htmlTheme = MarkdownHtmlTheme.StudioDark,
        MermaidRenderTheme mermaidTheme = MermaidRenderTheme.StudioDark,
        string? title = null)
    {
        var body = ToHtml(markdown, mermaidTheme);
        return MarkdownHtmlDocument.Wrap(body, htmlTheme, title);
    }

    private static IHtmlNode? RenderSection(IMarkdownSection section, MermaidRenderTheme theme)
    {
        if (section is not IMarkdownCodeBlock code
            || !string.Equals(code.Language, "mermaid", StringComparison.OrdinalIgnoreCase))
            return null;

        var svg = MermaidSvgRenderer.TryRenderSvg(code.Code, theme);
        if (svg is null)
            return null;

        var bytes = Encoding.UTF8.GetBytes(svg);
        var source = $"data:image/svg+xml;base64,{Convert.ToBase64String(bytes)}";

        return HtmlMarkup.Div(div => div
            .Class("mermaid-diagram")
            .Img(source, "Mermaid diagram"));
    }
}
