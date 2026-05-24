# Novolis.Markup.Markdown

Fluent GitHub-flavored Markdown document builder for .NET.

## Install

```bash
dotnet add package Novolis.Markup.Markdown
```

**Prerequisites:** [.NET 10 SDK](https://dotnet.microsoft.com/download) (`net10.0`).

## Quick start

```csharp
using Novolis.Markup.Markdown;

var doc = new MarkdownDocument()
    .WithHeader("Release notes", MarkdownHeaderLevel.H1)
    .WithAlert("Breaking change in API.", MarkdownAlertLevel.Warning)
    .WithCodeBlock("var x = 1;", "csharp");

string markdown = doc.ToString();
string html = doc.ToHtml();
```

## Related packages

| Package | When to use |
|---------|-------------|
| `Novolis.Markup.Mermaid` | Diagram syntax (flowcharts, timelines, pie charts) |
| `Novolis.CodeGen.Reflection.ClassDiagram` | UML/class diagrams from reflection (codegen repo) |

## More documentation

- [Getting started](https://github.com/Novolis-Platform/novolis-markup/blob/main/docs/getting-started.md)
- [Design](https://github.com/Novolis-Platform/novolis-markup/blob/main/docs/design.md)
- [Novolis.Markup.Mermaid README](https://github.com/Novolis-Platform/novolis-markup/blob/main/src/Novolis.Markup.Mermaid/README.md)

## Support

Pre-release platform package; API may evolve with Novolis governance releases.
