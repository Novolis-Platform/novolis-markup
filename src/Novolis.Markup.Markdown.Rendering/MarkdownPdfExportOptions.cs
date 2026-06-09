namespace Novolis.Markup.Markdown.Rendering;

/// <summary>Options for PDF export of Markdown documents.</summary>
public sealed class MarkdownPdfExportOptions
{
    /// <summary>Document title shown on the cover and header.</summary>
    public string? Title { get; init; }

    /// <summary>Optional subtitle on the cover page.</summary>
    public string? Subtitle { get; init; }

    /// <summary>Optional author line on the cover page.</summary>
    public string? Author { get; init; }

    /// <summary>Whether to include a cover page.</summary>
    public bool IncludeCoverPage { get; init; } = true;

    /// <summary>Page width in inches.</summary>
    public float PageWidthInches { get; init; } = 8.5f;

    /// <summary>Page height in inches.</summary>
    public float PageHeightInches { get; init; } = 11f;

    /// <summary>Horizontal page margin in inches.</summary>
    public float MarginHorizontalInches { get; init; } = 0.75f;

    /// <summary>Vertical page margin in inches.</summary>
    public float MarginVerticalInches { get; init; } = 0.75f;

    /// <summary>Body font size in points.</summary>
    public float BodyFontSize { get; init; } = 11f;

    /// <summary>Heading font size in points.</summary>
    public float HeadingFontSize { get; init; } = 14f;

    /// <summary>Body font family.</summary>
    public string BodyFontFamily { get; init; } = "Georgia";

    /// <summary>Monospace font family for code blocks.</summary>
    public string CodeFontFamily { get; init; } = "Courier New";
}
