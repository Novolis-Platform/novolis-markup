# Novolis.Markup.Markdown.Rendering

Markdig HTML rendering and document export for raw Markdown source.

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

## Related packages

| Package | When to use |
|---------|-------------|
| `Novolis.Markup.Markdown` | Fluent GFM document builder |
| `Novolis.Avalonia.Markdown` | Avalonia editor + live preview controls |
