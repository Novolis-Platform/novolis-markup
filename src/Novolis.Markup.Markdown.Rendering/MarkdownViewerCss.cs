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

    /// <summary>
    /// Theme colors that must not depend on <c>prefers-color-scheme</c>
    /// (Android/iOS WebViews often report light even when the host is dark).
    /// </summary>
    public static string ForTheme(MarkdownHtmlTheme theme) => Overlay + theme switch
    {
        MarkdownHtmlTheme.GitHubLight => """
            html { color-scheme: light; background: #ffffff; }
            body { color-scheme: light; background: #ffffff; margin: 0; }
            """,
        MarkdownHtmlTheme.GitHubDark => """
            html { color-scheme: dark; background: #0d1117; }
            body { color-scheme: dark; background: #0d1117; margin: 0; }
            .markdown-body { color-scheme: dark; background-color: #0d1117; color: #e6edf3; }
            .markdown-body table tr { background-color: #0d1117; }
            .markdown-body table tr:nth-child(2n) { background-color: #161b22; }
            .markdown-body table td,
            .markdown-body table th { border-color: #30363d; }
            .markdown-body img { background-color: transparent; }
            .markdown-body pre,
            .markdown-body .highlight pre { background-color: #161b22; }
            .markdown-body code { background-color: #21262d; color: #e6edf3; }
            .markdown-body blockquote { color: #848d97; border-left-color: #30363d; }
            .markdown-body h1,
            .markdown-body h2 { border-bottom-color: #21262d; }
            .markdown-body hr { background-color: #21262d; }
            .markdown-body a { color: #2f81f7; }
            """,
        _ => """
            html { color-scheme: dark; }
            """,
    };
}
