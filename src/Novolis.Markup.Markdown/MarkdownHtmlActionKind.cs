namespace Novolis.Markup.Markdown;

/// <summary>Host action encoded in a <see cref="MarkdownHtmlActionUris"/> link.</summary>
public enum MarkdownHtmlActionKind
{
    /// <summary>Copy a fenced code block.</summary>
    Copy = 1,

    /// <summary>Open mermaid or image fullscreen preview.</summary>
    Preview = 2,
}
