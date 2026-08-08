# Novolis.Markup.Markdown.Rendering

Themed HTML document export for Markdown source via `Novolis.Markup.Markdown` (no Markdig).

```bash
dotnet add package Novolis.Markup.Markdown.Rendering
```

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
