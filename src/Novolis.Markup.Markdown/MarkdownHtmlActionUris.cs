namespace Novolis.Markup.Markdown;

/// <summary>Host-handled URIs for Markdown chrome that must not run script.</summary>
public static class MarkdownHtmlActionUris
{
    /// <summary>Legacy custom scheme still accepted from older HTML.</summary>
    public const string Scheme = "novolis-md";

    /// <summary>
    /// <c>about:</c> actions navigate for real (so WebView2 raises Navigating)
    /// without touching the network or an unregistered protocol.
    /// </summary>
    public const string AboutPrefix = "about:novolis-md/";

    /// <summary>Builds a copy-action URI for a code-block index.</summary>
    public static string Copy(int index) => $"{AboutPrefix}copy/{index}";

    /// <summary>Builds a fullscreen-preview URI for a media index.</summary>
    public static string Preview(int index) => $"{AboutPrefix}preview/{index}";

    /// <summary>Tries to read a copy or preview index from a navigation URL.</summary>
    public static bool TryRead(string? url, out MarkdownHtmlActionKind kind, out int index)
    {
        kind = default;
        index = -1;
        if (string.IsNullOrWhiteSpace(url))
            return false;
        if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return TryRead(uri, out kind, out index);

        return TryReadText(url, out kind, out index);
    }

    /// <summary>Tries to read a copy or preview index from a host URI.</summary>
    public static bool TryRead(Uri uri, out MarkdownHtmlActionKind kind, out int index)
    {
        kind = default;
        index = -1;
        if (string.Equals(uri.Scheme, Scheme, StringComparison.OrdinalIgnoreCase))
            return TryReadPath($"{uri.Host}{uri.AbsolutePath}", out kind, out index);

        if (string.Equals(uri.Host, "novolis.md", StringComparison.OrdinalIgnoreCase))
            return TryReadPath(uri.AbsolutePath, out kind, out index);

        if (TryReadText(uri.Fragment, out kind, out index))
            return true;

        return TryReadText(uri.OriginalString, out kind, out index);
    }

    private static bool TryReadText(string text, out MarkdownHtmlActionKind kind, out int index)
    {
        kind = default;
        index = -1;
        if (string.IsNullOrWhiteSpace(text))
            return false;

        var markers = new[] { AboutPrefix, "novolis-md://", "#novolis-md/", "https://novolis.md/" };
        foreach (var marker in markers)
        {
            var start = text.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (start < 0)
                continue;
            return TryReadPath(text[(start + marker.Length)..], out kind, out index);
        }

        return false;
    }

    private static bool TryReadPath(string path, out MarkdownHtmlActionKind kind, out int index)
    {
        kind = default;
        index = -1;
        var parts = path.Trim().TrimStart('#', '/').Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2 || !int.TryParse(parts[1].Split('?', '#')[0], out index) || index < 0)
            return false;

        kind = parts[0].ToLowerInvariant() switch
        {
            "copy" => MarkdownHtmlActionKind.Copy,
            "preview" => MarkdownHtmlActionKind.Preview,
            _ => default,
        };
        return kind is not default(MarkdownHtmlActionKind);
    }
}
