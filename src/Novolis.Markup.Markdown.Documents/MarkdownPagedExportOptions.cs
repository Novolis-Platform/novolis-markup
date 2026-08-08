using Novolis.Documents;
using Novolis.Math.Measure;

namespace Novolis.Markup.Markdown.Documents;

/// <summary>Options for mapping Markdown into a <see cref="PagedDocument"/> / PDF.</summary>
public sealed class MarkdownPagedExportOptions
{
    /// <summary>Document title (first page + chrome).</summary>
    public string? Title { get; init; }

    /// <summary>Optional subtitle on the first page.</summary>
    public string? Subtitle { get; init; }

    /// <summary>Optional author on the first page.</summary>
    public string? Author { get; init; }

    /// <summary>Optional series line on the first page.</summary>
    public string? Series { get; init; }

    /// <summary>Optional rights line on the first page.</summary>
    public string? Rights { get; init; }

    /// <summary>Optional first-page extras (lines below author). When set, a first page is emitted.</summary>
    public FirstPage? First { get; init; }

    /// <summary>Optional last page after body.</summary>
    public LastPage? Last { get; init; }

    /// <summary>When true, emit a first/title page. Default false for short docs.</summary>
    public bool IncludeCover { get; init; }

    /// <summary>When true, emit a contents page from level-1 headings. Default false for short docs.</summary>
    public bool IncludeToc { get; init; }

    /// <summary>Page trim size.</summary>
    public Size Trim { get; init; } = TrimPresets.Inch6x9;

    /// <summary>Page margins.</summary>
    public Thickness Margin { get; init; } = TrimPresets.DefaultMargin;

    /// <summary>Typography for body flow.</summary>
    public Typography Typography { get; init; } = new();

    /// <summary>Running header template (<c>{title}</c>, <c>{page}</c>). Empty disables header.</summary>
    public string HeaderTemplate { get; init; } = "{title}";

    /// <summary>Running footer template. Empty disables footer.</summary>
    public string FooterTemplate { get; init; } = "{page}";
}
