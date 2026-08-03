<!-- novolis-marketing:start -->
<p align="center">
  <a href="https://github.com/Novolis-Platform">
    <img src="https://raw.githubusercontent.com/Novolis-Platform/.github/main/brand/logo-brand-transparent.svg" width="360" alt="Novolis"/>
  </a>
</p>

<p align="center">
  <img src="https://raw.githubusercontent.com/Novolis-Platform/.github/main/brand/banners/novolis-markup.svg" width="100%" alt="novolis-markup"/>
</p>

<p align="center">
  <strong>Manuscripts, markdown, mermaid</strong><br/>
  Manuscript, markdown, and Mermaid markup pipelines.
</p>

<p align="center">
  <a href="https://github.com/Novolis-Platform/novolis-markup/actions"><img src="https://img.shields.io/github/actions/workflow/status/Novolis-Platform/novolis-markup/merge.yml?branch=main&label=merge&logo=github" alt="merge"/></a>
  <a href="https://github.com/orgs/Novolis-Platform/packages?repo_name=novolis-markup"><img src="https://img.shields.io/badge/packages-GitHub%20Packages-0a7ea3?logo=nuget" alt="packages"/></a>
  <a href="https://github.com/Novolis-Platform"><img src="https://img.shields.io/badge/org-Novolis--Platform-111827" alt="org"/></a>
</p>

<p align="center">
  <a href="https://nuget.pkg.github.com/Novolis-Platform/index.json"><code>https://nuget.pkg.github.com/Novolis-Platform/index.json</code></a>
  ·
  <a href="https://github.com/Novolis-Platform/.github/blob/main/profile/README.md">Org landing</a>
  ·
  <a href="https://github.com/Novolis-Platform/novolis-governance">Governance</a>
</p>

---
<!-- novolis-marketing:end -->
<!-- novolis-package-index:start -->
> **GitHub Packages shows this repository README on every package page** (upstream limitation).
> Open the **package README** for install and quick start — embedded in each .nupkg and linked below.

## Published packages

| Package | Install | Package README |
|---------|---------|----------------|
| `Novolis.Markup.Manuscript` | `dotnet add package Novolis.Markup.Manuscript` | [README](https://github.com/Novolis-Platform/novolis-markup/blob/main/src/Novolis.Markup.Manuscript/README.md) |
| `Novolis.Markup.Markdown` | `dotnet add package Novolis.Markup.Markdown` | [README](https://github.com/Novolis-Platform/novolis-markup/blob/main/src/Novolis.Markup.Markdown/README.md) |
| `Novolis.Markup.Markdown.Rendering` | `dotnet add package Novolis.Markup.Markdown.Rendering` | [README](https://github.com/Novolis-Platform/novolis-markup/blob/main/src/Novolis.Markup.Markdown.Rendering/README.md) |
| `Novolis.Markup.Mermaid` | `dotnet add package Novolis.Markup.Mermaid` | [README](https://github.com/Novolis-Platform/novolis-markup/blob/main/src/Novolis.Markup.Mermaid/README.md) |

For NuGet.org and Visual Studio, the **embedded** README.md inside each package is authoritative.

<!-- novolis-package-index:end -->
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

