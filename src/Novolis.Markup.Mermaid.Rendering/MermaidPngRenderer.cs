using System.Text;
using SkiaSharp;
using Svg.Skia;

namespace Novolis.Markup.Mermaid.Rendering;

/// <summary>Rasters Mermaid SVG documents to PNG using Svg.Skia.</summary>
public static class MermaidPngRenderer
{
    /// <summary>
    /// Renders Mermaid source to PNG bytes, or <c>null</c> on failure.
    /// </summary>
    /// <param name="mermaid">Mermaid diagram source.</param>
    /// <param name="theme">Color theme.</param>
    /// <param name="scale">Raster scale factor (1 = native SVG size).</param>
    public static byte[]? TryRenderPng(
        string? mermaid,
        MermaidRenderTheme theme = MermaidRenderTheme.StudioDark,
        float scale = 1f)
    {
        var svg = MermaidSvgRenderer.TryRenderSvg(mermaid, theme);
        return svg is null ? null : TryEncodePng(svg, scale);
    }

    /// <summary>Encodes an SVG document string to PNG bytes, or <c>null</c> on failure.</summary>
    public static byte[]? TryEncodePng(string? svg, float scale = 1f)
    {
        if (string.IsNullOrWhiteSpace(svg))
            return null;

        if (scale <= 0f || float.IsNaN(scale) || float.IsInfinity(scale))
            return null;

        try
        {
            using var skSvg = new SKSvg();
            using var loadStream = new MemoryStream(Encoding.UTF8.GetBytes(svg));
            if (skSvg.Load(loadStream) is null || skSvg.Picture is null)
                return null;

            using var pngStream = new MemoryStream();
            skSvg.Picture.ToImage(
                pngStream,
                SKColors.Empty,
                SKEncodedImageFormat.Png,
                quality: 100,
                scaleX: scale,
                scaleY: scale,
                skColorType: SKColorType.Rgba8888,
                skAlphaType: SKAlphaType.Premul,
                skColorSpace: SKColorSpace.CreateSrgb());

            return pngStream.Length == 0 ? null : pngStream.ToArray();
        }
        catch
        {
            return null;
        }
    }
}
