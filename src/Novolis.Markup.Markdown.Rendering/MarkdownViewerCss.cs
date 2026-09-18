namespace Novolis.Markup.Markdown.Rendering;

/// <summary>Host-agnostic overlay for Markdown HTML documents (phones, WebViews, and desktop panes).</summary>
internal static class MarkdownViewerCss
{
    public const string Overlay = """
        html { -webkit-text-size-adjust: 100%; }
        img { max-width: 100%; height: auto; }
        table { display: block; overflow-x: auto; -webkit-overflow-scrolling: touch; }
        th, td { white-space: nowrap; word-break: normal; }
        .mermaid-diagram { margin: 0 0 1.2em; overflow-x: auto; -webkit-overflow-scrolling: touch; }
        .mermaid-diagram img { display: block; max-width: none; height: auto; }
        """;
}
