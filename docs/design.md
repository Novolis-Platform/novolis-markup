# Design

## Purpose

Programmatic construction of **Markdown** and **Mermaid** text without templates or string concatenation scattered through application code. The libraries migrated from [Frank.Markdown](https://github.com/frankhaugen/Frank.Markdown) and [Frank.Mermaid](https://github.com/frankhaugen/Frank.Mermaid) (wave 10).

## Packages

| Package | Responsibility |
|---------|----------------|
| `Novolis.Markup.Markdown` | Section-based GFM documents (headers, lists, tables, alerts, code blocks) and optional HTML export |
| `Novolis.Markup.Mermaid` | Diagram builders (flowchart, git graph, pie chart, timeline, XY chart) emitting Mermaid source |

There is no shared runtime dependency between the two packages; reference only what you need.

## Markdown model

- **`IMarkdownDocument`** — ordered collection of **`IMarkdownSection`** instances.
- **`MarkdownDocument`** — mutable builder with `With(...)` chaining and `Parse` for simple imports.
- **Extension methods** on `IMarkdownDocument` — convenience `WithHeader`, `WithTable<T>`, etc.
- **`EnumerableExtensions.ToMarkdownTable<T>`** — reflects public properties into pipe tables.

HTML output uses embedded GitHub-flavored CSS constants (`GithubMarkdownCss`).

## Mermaid model

- **`IMermaidable`** — stable `Hash` id plus `GetBuilder()` / `GetMermaidString()`.
- **`IndentedStringBuilder`** — indentation-aware emission shared by diagram types.
- **Flowchart** — nodes, links, subgraphs, shapes, and direction.
- Other diagram types follow the same builder pattern under their namespaces.

Reflection-based **class diagrams** live in `Novolis.CodeGen.Reflection.ClassDiagram` ([novolis-codegen](https://github.com/Novolis-Platform/novolis-codegen)), not in this repo.

## Documentation policy

Public API is documented with XML comments (`GenerateDocumentationFile`, strict CS1591). Package READMEs ship on NuGet via `PackageReadmeFile`.
