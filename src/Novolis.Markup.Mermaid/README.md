# Novolis.Markup.Mermaid

Fluent Mermaid diagram syntax builder for .NET. Covers the major Mermaid diagram families as first-class builders that emit Mermaid source.

## Install

```bash
dotnet add package Novolis.Markup.Mermaid
```

**Prerequisites:** [.NET 10 SDK](https://dotnet.microsoft.com/download) (`net10.0`).

## Diagram kinds

| Builder | Mermaid header |
|---------|----------------|
| `Flowchart` | `flowchart` |
| `SequenceDiagram` | `sequenceDiagram` |
| `ClassDiagram` | `classDiagram` |
| `StateDiagram` | `stateDiagram-v2` |
| `ErDiagram` | `erDiagram` |
| `Journey` | `journey` |
| `Gantt` | `gantt` |
| `PieChart` | `pie` |
| `QuadrantChart` | `quadrantChart` |
| `RequirementDiagram` | `requirementDiagram` |
| `GitGraph` | `gitGraph` |
| `Mindmap` | `mindmap` |
| `Timeline` | `timeline` |
| `Sankey` | `sankey-beta` |
| `XyChart` | `xychart-beta` |
| `BlockDiagram` | `block-beta` |
| `ArchitectureDiagram` | `architecture-beta` |
| `C4Diagram` | `C4Context` / `C4Container` / … |
| `PacketDiagram` | `packet-beta` |
| `RadarChart` | `radar-beta` |
| `Treemap` | `treemap-beta` |
| `Kanban` | `kanban` |
| `VennDiagram` | `venn-beta` |
| `TreeView` | `treeView` |

See `MermaidDiagramKind` for the catalog enum.

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

```csharp
var seq = new SequenceDiagram()
    .AddParticipant("A", "Alice")
    .AddParticipant("B", "Bob")
    .Message("A", "B", "Hello");
```

Paste the result into any Mermaid renderer. For Avalonia UI rendering, use `Novolis.Avalonia.Mermaid` (`MermaidControl`).

## Related packages

| Package | When to use |
|---------|-------------|
| `Novolis.Markup.Markdown` | GFM documents, tables, alerts, HTML export |
| `Novolis.Avalonia.Mermaid` | Avalonia control that renders Mermaid source to SVG |
| `Novolis.CodeGen.Reflection.ClassDiagram` | Class diagrams from .NET types (codegen repo) |

## More documentation

- [Getting started](https://github.com/Novolis-Platform/novolis-markup/blob/main/docs/getting-started.md)
- [Design](https://github.com/Novolis-Platform/novolis-markup/blob/main/docs/design.md)
- [Novolis.Markup.Markdown README](https://github.com/Novolis-Platform/novolis-markup/blob/main/src/Novolis.Markup.Markdown/README.md)

## Support

Pre-release platform package; API may evolve with Novolis governance releases.
