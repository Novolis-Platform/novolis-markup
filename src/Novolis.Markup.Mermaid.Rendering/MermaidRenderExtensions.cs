using System.Text;
using Novolis.Markup.Mermaid;

namespace Novolis.Markup.Mermaid.Rendering;

/// <summary>Extension methods that export <see cref="IMermaidable"/> diagrams to SVG and PNG.</summary>
public static class MermaidRenderExtensions
{
    /// <summary>Renders the diagram to an SVG document string, or <c>null</c> on failure.</summary>
    public static string? ToSvg(
        this IMermaidable diagram,
        MermaidRenderTheme theme = MermaidRenderTheme.StudioDark)
    {
        ArgumentNullException.ThrowIfNull(diagram);
        return MermaidSvgRenderer.TryRenderSvg(diagram.GetMermaidString(), theme);
    }

    /// <summary>Renders the diagram to PNG bytes, or <c>null</c> on failure.</summary>
    public static byte[]? ToPng(
        this IMermaidable diagram,
        MermaidRenderTheme theme = MermaidRenderTheme.StudioDark,
        float scale = 1f)
    {
        ArgumentNullException.ThrowIfNull(diagram);
        return MermaidPngRenderer.TryRenderPng(diagram.GetMermaidString(), theme, scale);
    }

    /// <summary>Writes the diagram SVG to <paramref name="path"/>. Returns <c>false</c> on failure.</summary>
    public static bool ExportSvg(
        this IMermaidable diagram,
        string path,
        MermaidRenderTheme theme = MermaidRenderTheme.StudioDark,
        Encoding? encoding = null)
    {
        ArgumentNullException.ThrowIfNull(diagram);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        var svg = diagram.ToSvg(theme);
        if (svg is null)
            return false;

        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllText(path, svg, encoding ?? new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        return true;
    }

    /// <summary>Writes the diagram PNG to <paramref name="path"/>. Returns <c>false</c> on failure.</summary>
    public static bool ExportPng(
        this IMermaidable diagram,
        string path,
        MermaidRenderTheme theme = MermaidRenderTheme.StudioDark,
        float scale = 1f)
    {
        ArgumentNullException.ThrowIfNull(diagram);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        var png = diagram.ToPng(theme, scale);
        if (png is null)
            return false;

        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllBytes(path, png);
        return true;
    }

    /// <summary>Renders raw Mermaid source to SVG, or <c>null</c> on failure.</summary>
    public static string? ToSvg(
        this string mermaid,
        MermaidRenderTheme theme = MermaidRenderTheme.StudioDark) =>
        MermaidSvgRenderer.TryRenderSvg(mermaid, theme);

    /// <summary>Renders raw Mermaid source to PNG bytes, or <c>null</c> on failure.</summary>
    public static byte[]? ToPng(
        this string mermaid,
        MermaidRenderTheme theme = MermaidRenderTheme.StudioDark,
        float scale = 1f) =>
        MermaidPngRenderer.TryRenderPng(mermaid, theme, scale);
}
