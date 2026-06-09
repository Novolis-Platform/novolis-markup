namespace Novolis.Markup.Markdown.Rendering;

/// <summary>Built-in HTML preview themes for rendered Markdown.</summary>
public enum MarkdownHtmlTheme
{
    /// <summary>Dark studio theme tuned for Avalonia HtmlRenderer previews.</summary>
    StudioDark,

    /// <summary>GitHub-flavored light theme with embedded github-markdown-css.</summary>
    GitHubLight,

    /// <summary>GitHub-flavored dark theme with embedded github-markdown-css dark overrides.</summary>
    GitHubDark,
}
