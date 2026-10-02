namespace Novolis.Markup.Markdown;

/// <summary>Embeds local Markdown images as <c>data:</c> URIs so CSP <c>img-src data:</c> can show them.</summary>
public static class MarkdownLocalImage
{
    private const int MaximumBytes = 2 * 1024 * 1024;

    /// <summary>Resolves a Markdown image URL to a <c>data:</c> URI when the file is a local image.</summary>
    public static string? TryEmbed(string? url, string? sourceDirectory)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;
        if (url.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            return url;
        if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri))
            return null;
        if (uri.IsAbsoluteUri && uri.Scheme is "http" or "https" or "mailto")
            return null;

        var path = uri.IsAbsoluteUri && uri.IsFile
            ? uri.LocalPath
            : string.IsNullOrWhiteSpace(sourceDirectory)
                ? null
                : Path.GetFullPath(Path.Combine(sourceDirectory, url.Replace('/', Path.DirectorySeparatorChar)));
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            return null;
        if (!IsUnderSource(path, sourceDirectory) && !(uri.IsAbsoluteUri && uri.IsFile))
            return null;

        var mime = Mime(path);
        if (mime is null)
            return null;

        var info = new FileInfo(path);
        if (info.Length is <= 0 or > MaximumBytes)
            return null;

        var bytes = File.ReadAllBytes(path);
        return $"data:{mime};base64,{Convert.ToBase64String(bytes)}";
    }

    private static bool IsUnderSource(string path, string? sourceDirectory)
    {
        if (string.IsNullOrWhiteSpace(sourceDirectory))
            return false;
        var root = Path.GetFullPath(sourceDirectory)
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var full = Path.GetFullPath(path);
        return full.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)
            || string.Equals(full, root, StringComparison.OrdinalIgnoreCase);
    }

    private static string? Mime(string path) => Path.GetExtension(path).ToLowerInvariant() switch
    {
        ".png" => "image/png",
        ".jpg" or ".jpeg" => "image/jpeg",
        ".gif" => "image/gif",
        ".webp" => "image/webp",
        ".svg" => "image/svg+xml",
        ".bmp" => "image/bmp",
        _ => null,
    };
}
