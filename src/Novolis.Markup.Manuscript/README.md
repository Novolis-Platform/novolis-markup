<!-- novolis-pkg-brand:start -->
<p align="center">
  <a href="https://github.com/Novolis-Platform/novolis-markup">
    <img src="https://raw.githubusercontent.com/Novolis-Platform/.github/main/brand/logo-icon.svg" width="72" alt="Novolis"/>
  </a>
</p>
<!-- novolis-pkg-brand:end -->

# Novolis.Markup.Manuscript

Chapter metadata, content catalog, structural diagnostics, and book/reference PDF export for manuscript workspaces. Used by Books Mobile and Books Writer Studio.

## Install

```bash
dotnet add package Novolis.Markup.Manuscript
```

Depends on `Novolis.IO.Paths`, `YamlDotNet`, and `Novolis.Markup.Markdown.Rendering` (PDF export).

## Quick start — open workspace

```csharp
using Novolis.Markup.Manuscript;

if (!ManuscriptWorkspace.TryOpen(startDir, out var workspace) || workspace is null)
    throw new InvalidOperationException("No manuscript workspace found.");

var series = workspace.Catalog.Load(workspace.ContentRoot);
var issues = ManuscriptDoctor.Diagnose(workspace.ContentRoot);
```

Expects `content/series` or `content/books` under the content root. Books contain `book.yaml`, `chapters/*.md`, optional `appendices/` and `references/`.

## Quick start — metadata and PDF

```csharp
var (meta, body, format) = ManuscriptMetadata.Parse(chapterMarkdown);
var words = ManuscriptMetadata.CountWords(chapterMarkdown);

var settings = ManuscriptPrintSettings.Load(settingsPath);
ManuscriptBookPdfExporter.ExportBook(bookInfo, outputPath, settings);
```

## API

| Type | Role |
|------|------|
| `ManuscriptWorkspace` | `TryOpen(startDir, out workspace)`; `ContentRoot`, `Catalog` |
| `ManuscriptCatalog` | `Load`, `LoadStandaloneBooks`, `FindBook` |
| `SeriesInfo`, `BookInfo`, `ChapterInfo`, `ReferenceSetInfo`, `ReferenceFileInfo` | Catalog records |
| `ChapterKind` | `Chapter`, `Appendix` |
| `ManuscriptDoctor` | `Diagnose(contentRoot\|series\|book)` → findings |
| `DiagnosticSeverity`, `DiagnosticFinding` | Structural diagnostics |
| `ManuscriptMetadata` | `Parse`, `GetBodyForWordCount`, `CountWords`, `ApplyCallouts` |
| `ManuscriptChapterMetadata`, `ManuscriptMetadataFormat` | Parsed front matter |
| `ChapterOrder` | Sort keys, titles, chapter ordering |
| `BookYaml` | `LoadFile`, `GetString`, `GetBool` for `series.yaml` / `book.yaml` |
| `ManuscriptBookPdfExporter` | `ExportBook`, `ExportReferenceSet` |
| `ManuscriptPrintSettings` | Layout JSON load/save; `ToPdfOptions` |

## Dogfooding / apps

Used by **BooksWriterStudio** (diagnostics + PDF export) and **BooksMobile** (catalog loading).

## Related

| Package | Role |
|---------|------|
| `Novolis.Markup.Markdown.Rendering` | `MarkdownPdfExporter` used internally |
| `Novolis.Markup.Markdown` | GFM document builder |
| `Novolis.IO.Paths` | Workspace path helpers |

## More documentation

- [Getting started](https://github.com/Novolis-Platform/novolis-markup/blob/main/docs/getting-started.md)
- [Design](https://github.com/Novolis-Platform/novolis-markup/blob/main/docs/design.md)

