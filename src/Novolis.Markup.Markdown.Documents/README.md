<!-- novolis-pkg-brand:start -->
[![Novolis](https://raw.githubusercontent.com/Novolis-Platform/.github/main/brand/logo-icon.png)](https://novolis-platform.github.io/.github/novolis-markup/)

[Novolis](https://github.com/Novolis-Platform) · [Docs](https://novolis-platform.github.io/.github/novolis-markup/) · [Source](https://github.com/Novolis-Platform/novolis-markup)
<!-- novolis-pkg-brand:end -->

# Novolis.Markup.Markdown.Documents

Map `Novolis.Markup.Markdown` (`IMarkdownDocument`) into `Novolis.Documents.PagedDocument` and export PDF with `Novolis.Documents.Skia`. No Markdig or QuestPDF.

Lists, code, quotes, and tables flatten to paragraphs in v1 (Documents block set is intentionally small).

## Install

```bash
dotnet add package Novolis.Markup.Markdown.Documents
```

## Quick start

```csharp
using Novolis.Markup.Markdown;
using Novolis.Markup.Markdown.Documents;

var doc = new MarkdownDocument()
    .WithHeader("Harbor Watch")
    .With(new MarkdownParagraph().WithText("Freight bells marked the hour."))
    .With(new MarkdownHorizontalRule())
    .WithHeader("Quay-side", 2)
    .With(new MarkdownParagraph().WithText("Spray bit the stones."));

MarkdownDocumentPdfExporter.ExportToFile(doc, "harbor.pdf", new MarkdownPagedExportOptions
{
    Title = "Harbor Watch",
    Author = "Novolis",
});
```

Cover/TOC default off for short docs; pass `IncludeCover` / `IncludeToc` when you want book chrome.

## Related packages

| Package | When to use |
|---------|-------------|
| `Novolis.Markup.Markdown` | Fluent Markdown document model |
| `Novolis.Markup.Markdown.Rendering` | Themed HTML export (Novolis Markdown) |
| `Novolis.Documents.Skia` | Paint `PagedDocument` directly |

## Support

- Docs: [novolis-markup](https://github.com/Novolis-Platform/novolis-markup)
