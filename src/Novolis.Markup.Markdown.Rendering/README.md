<!-- novolis-pkg-brand:start -->
<p align="center">
  <a href="https://github.com/Novolis-Platform/novolis-markup">
    <img src="https://raw.githubusercontent.com/Novolis-Platform/.github/main/brand/logo-icon.svg" width="72" alt="Novolis"/>
  </a>
</p>
<!-- novolis-pkg-brand:end -->

# Novolis.Markup.Markdown.Rendering

Themed HTML document export for Markdown source via `Novolis.Markup.Markdown` (no Markdig).

## Install

```bash
dotnet add package Novolis.Markup.Markdown.Rendering
```

## Quick start

```csharp
using Novolis.Markup.Markdown.Rendering;

MarkdownHtmlExporter.ExportToFile(markdown, "readme.html", MarkdownHtmlTheme.GitHubLight, title: "Readme");

var html = MarkdownHtmlDocument.FromMarkdown(markdown, MarkdownHtmlTheme.StudioDark, title: "Doc");
var fragment = NovolisMarkdownRenderer.ToHtml(markdown);
```

For PDF use `Novolis.Markup.Markdown.Documents` / `novolis-mdpdf` (Documents.Skia), not this package.

| Type | Role |
| --- | --- |
| `NovolisMarkdownRenderer` | `ToHtml(markdown)` body fragment |
| `MarkdownHtmlDocument` | Wrap / FromMarkdown / FromDocument |
| `MarkdownHtmlExporter` | Write standalone HTML files |
| `MarkdownHtmlTheme` | StudioDark / GitHubLight / GitHubDark |

## Support

- Docs: [novolis-markup](https://github.com/Novolis-Platform/novolis-markup)
