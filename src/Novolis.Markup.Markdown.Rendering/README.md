# Novolis.Markup.Markdown.Rendering

Markdig HTML rendering and document export for raw Markdown source — standalone HTML files and PDF via QuestPDF.

## Install

```bash
dotnet add package Novolis.Markup.Markdown.Rendering
```

**Prerequisites:** [.NET 10 SDK](https://dotnet.microsoft.com/download) (`net10.0`).

## Quick start

```csharp
using Novolis.Markup.Markdown.Rendering;

// HTML preview document (dark studio theme)
var html = MarkdownHtmlDocument.FromMarkdown("# Hello\n\nParagraph.");

// Export standalone HTML
MarkdownHtmlExporter.ExportToFile(markdown, "readme.html", MarkdownHtmlTheme.GitHubLight, "Readme");

// Export PDF (call once at app startup for QuestPDF Community license)
MarkdownPdfExporter.EnsureCommunityLicense();
MarkdownPdfExporter.ExportToFile(markdown, "readme.pdf", new MarkdownPdfExportOptions
{
    Title = "Readme",
    Author = "Novolis",
});
```

## API

| Type | Role |
|------|------|
| `MarkdigMarkdownRenderer` | `ToHtml(markdown, pipeline?)` |
| `MarkdownRenderPipeline` | Default Markdig pipeline |
| `MarkdownHtmlDocument` | `FromMarkdown`, `Wrap` — full HTML document |
| `MarkdownHtmlExporter` | `ExportToFile(markdown, path, theme, title)` |
| `MarkdownPdfExporter` | `EnsureCommunityLicense()`, `ExportToFile`, `ExportToBytes` |
| `MarkdownHtmlTheme` | `StudioDark`, `GitHubLight`, `GitHubDark` |
| `MarkdownPdfExportOptions` | Page size, margins, fonts, cover page |

## Related

| Package | Role |
|---------|------|
| `Novolis.Markup.Markdown` | Fluent GFM document builder |
| `Novolis.Markup.Manuscript` | Book/reference PDF export via `MarkdownPdfExporter` |
| `Novolis.Avalonia.Markdown` | Avalonia editor + live preview controls |

## More documentation

- [Getting started](https://github.com/Novolis-Platform/novolis-markup/blob/main/docs/getting-started.md)
- [Design](https://github.com/Novolis-Platform/novolis-markup/blob/main/docs/design.md)
