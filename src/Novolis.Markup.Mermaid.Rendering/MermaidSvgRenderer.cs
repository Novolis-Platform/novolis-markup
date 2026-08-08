using Mermaider;
using Mermaider.Models;

namespace Novolis.Markup.Mermaid.Rendering;

/// <summary>Renders Mermaid source to SVG using Mermaider (no browser).</summary>
public static class MermaidSvgRenderer
{
    /// <summary>Renders Mermaid source to an SVG document string, or <c>null</c> on failure.</summary>
    public static string? TryRenderSvg(string? mermaid, MermaidRenderTheme theme = MermaidRenderTheme.StudioDark)
    {
        if (string.IsNullOrWhiteSpace(mermaid))
            return null;

        try
        {
            var svg = MermaidRenderer.RenderSvg(mermaid.Trim(), OptionsFor(theme));
            return string.IsNullOrWhiteSpace(svg) ? null : svg;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>Maps a theme to Mermaider render options.</summary>
    public static RenderOptions OptionsFor(MermaidRenderTheme theme) =>
        theme switch
        {
            MermaidRenderTheme.GitHubLight => new RenderOptions
            {
                Bg = "#ffffff",
                Fg = "#24292f",
                Line = "#d8dee4",
                Accent = "#0969da",
                Muted = "#57606a",
                Surface = "#f6f8fa",
                Border = "#d8dee4",
                Font = "Segoe UI, system-ui, sans-serif",
                FontSize = "14px",
            },
            _ => new RenderOptions
            {
                Bg = "#1e1e1e",
                Fg = "#e8e8e8",
                Line = "#555555",
                Accent = "#6eb5ff",
                Muted = "#9da5ae",
                Surface = "#252526",
                Border = "#3a3a3a",
                Font = "Segoe UI, system-ui, sans-serif",
                FontSize = "14px",
            },
        };
}
