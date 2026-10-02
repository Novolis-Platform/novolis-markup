namespace Novolis.Markup.Markdown;

/// <summary>Host-handled URIs for Markdown chrome that must not run script.</summary>
public static class MarkdownHtmlActionUris
{
    /// <summary>Custom scheme cancelled in the WebView and handled by the host.</summary>
    public const string Scheme = "novolis-md";

    /// <summary>Builds a copy-action URI for a code-block index.</summary>
    public static string Copy(int index) => $"{Scheme}://copy/{index}";

    /// <summary>Builds a fullscreen-preview URI for a media index.</summary>
    public static string Preview(int index) => $"{Scheme}://preview/{index}";

    /// <summary>Tries to read a copy or preview index from a host URI.</summary>
    public static bool TryRead(Uri uri, out MarkdownHtmlActionKind kind, out int index)
    {
        kind = default;
        index = -1;
        if (!string.Equals(uri.Scheme, Scheme, StringComparison.OrdinalIgnoreCase))
            return false;

        if (!int.TryParse(uri.AbsolutePath.Trim('/'), out index) || index < 0)
            return false;

        kind = uri.Host.ToLowerInvariant() switch
        {
            "copy" => MarkdownHtmlActionKind.Copy,
            "preview" => MarkdownHtmlActionKind.Preview,
            _ => default,
        };
        return kind is not default(MarkdownHtmlActionKind);
    }
}
