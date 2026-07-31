# novolis-markup

Programmatic Markdown, Mermaid, and manuscript tooling for .NET — migrated from [Frank.Markdown](https://github.com/frankhaugen/Frank.Markdown) and [Frank.Mermaid](https://github.com/frankhaugen/Frank.Mermaid) (wave 10).

## Packages

| Package | Description |
|---------|-------------|
| `Novolis.Markup.Markdown` | Fluent GitHub-flavored Markdown document builder |
| `Novolis.Markup.Markdown.Rendering` | Markdig HTML rendering and export (HTML, PDF via QuestPDF) |
| `Novolis.Markup.Mermaid` | Fluent Mermaid diagram syntax builder (flowchart, sequence, gantt, …) |
| `Novolis.Markup.Manuscript` | Chapter metadata, content catalog, diagnostics, book/reference PDF export |

Avalonia editor and preview controls live in [`novolis-avalonia`](https://github.com/Novolis-Platform/novolis-avalonia) as `Novolis.Avalonia.Markdown` and `Novolis.Avalonia.Mermaid`.

Reflection-based class diagrams remain in `Novolis.CodeGen.Reflection.ClassDiagram` ([novolis-codegen](https://github.com/Novolis-Platform/novolis-codegen)).

## Install

```bash
dotnet add package Novolis.Markup.Markdown
dotnet add package Novolis.Markup.Manuscript
```

## Quick start

```csharp
using Novolis.Markup.Markdown;

var doc = new MarkdownDocument()
    .WithHeader("Release notes", MarkdownHeaderLevel.H1)
    .WithAlert("Breaking change.", MarkdownAlertLevel.Warning);

string markdown = doc.ToString();
string html = doc.ToHtml();
```

Manuscript workspace (Books Mobile / Writer Studio):

```csharp
using Novolis.Markup.Manuscript;

if (ManuscriptWorkspace.TryOpen(startDir, out var workspace) && workspace is not null)
{
    var series = workspace.Catalog.Load(workspace.ContentRoot);
    var issues = ManuscriptDoctor.Diagnose(workspace.ContentRoot);
}
```

## Build

```bash
dotnet build Novolis.Markup.slnx
dotnet test Novolis.Markup.slnx
```

## Documentation

- [Getting started](docs/getting-started.md)
- [Design](docs/design.md)

## Related repos

| Repo | Role |
|------|------|
| `novolis-avalonia` | Markdown/Mermaid UI controls |
| `novolis-apps` | Books Mobile, Books Writer Studio consumers |
| `novolis-codegen` | Reflection class diagrams |
