# Novolis.Markup.Mermaid

Fluent Mermaid diagram syntax builder for .NET (flowchart, git graph, pie chart, timeline, XY chart).

## Install

```bash
dotnet add package Novolis.Markup.Mermaid
```

**Prerequisites:** [.NET 10 SDK](https://dotnet.microsoft.com/download) (`net10.0`).

## Quick start

```csharp
using Novolis.Markup.Mermaid;

var chart = new Flowchart(Direction.TopToBottom);
var start = new Node("Start", Shape.Rounded);
var end = new Node("Done");
chart.AddNode(start);
chart.AddNode(end);
chart.AddLink(new Link(start, end, "next"));

string mermaid = chart.GetMermaidString();
```

Paste the result into any Mermaid renderer (docs site, wiki, IDE preview).

## Related packages

| Package | When to use |
|---------|-------------|
| `Novolis.Markup.Markdown` | GFM documents, tables, alerts, HTML export |
| `Novolis.CodeGen.Reflection.ClassDiagram` | Class diagrams from .NET types (codegen repo) |

## More documentation

- [Getting started](https://github.com/Novolis-Platform/novolis-markup/blob/main/docs/getting-started.md)
- [Design](https://github.com/Novolis-Platform/novolis-markup/blob/main/docs/design.md)
- [Novolis.Markup.Markdown README](https://github.com/Novolis-Platform/novolis-markup/blob/main/src/Novolis.Markup.Markdown/README.md)

## Support

Pre-release platform package; API may evolve with Novolis governance releases.
