<!-- novolis-pkg-brand:start -->
<p align="center">
  <a href="https://github.com/Novolis-Platform/novolis-markup">
    <img src="https://raw.githubusercontent.com/Novolis-Platform/.github/main/brand/logo-icon.svg" width="72" alt="Novolis"/>
  </a>
</p>
<!-- novolis-pkg-brand:end -->

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
| `XyChart` | `xychart` |
| `BlockDiagram` | `block-beta` |
| `ArchitectureDiagram` | `architecture-beta` |
| `C4Diagram` | `C4Context` / `C4Container` / … |
| `PacketDiagram` | `packet-beta` |
| `RadarChart` | `radar-beta` |
| `Treemap` | `treemap-beta` |
| `Kanban` | `kanban` |
| `VennDiagram` | `venn-beta` |
| `TreeView` | `treeView` |
| `IshikawaDiagram` | `ishikawa-beta` |
| `UseCaseDiagram` | `usecase-beta` |
| `WardleyMap` | `wardley-beta` |
| `CynefinDiagram` | `cynefin-beta` |
| `RailroadDiagram` | `railroad-ebnf-beta` / `railroad-abnf-beta` / `railroad-peg-beta` / `railroad-beta` |
| `EventModelingDiagram` | `eventmodeling` |

See `MermaidDiagramKind` for the catalog enum.

## Serialize and parse

Mermaid source is the canonical form. `MermaidDocument` reconstructs a typed builder (or a lossless `RawMermaidDiagram` fallback). `MermaidJson` wraps the same source in a JSON envelope.

```csharp
using Novolis.Markup.Mermaid;

var source = """
    flowchart LR
        A[Start] --> B([Done])
    """;

MermaidDocument doc = MermaidDocument.Parse(source);
string mermaid = doc.ToMermaidString();
string json = MermaidJson.Serialize(doc);
MermaidDocument again = MermaidJson.Deserialize(json);
```

Front matter (`title`, `theme`, `look`, `layout`) is preserved when present. `MermaidKindDetector.TryDetect` maps a header token (`xychart`, `ishikawa-beta`, `C4Context`, …) onto `MermaidDiagramKind`.

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

Paste the result into any Mermaid renderer. For headless SVG/PNG export, use `Novolis.Markup.Mermaid.Rendering` (`ToSvg` / `ToPng`). For Avalonia UI, use `Novolis.Avalonia.Mermaid` (`MermaidControl`).

## Related packages

| Package | When to use |
|---------|-------------|
| `Novolis.Markup.Markdown` | GFM documents, tables, alerts, HTML export |
| `Novolis.Markup.Mermaid.Rendering` | Headless SVG/PNG export for `IMermaidable` |
| `Novolis.Avalonia.Mermaid` | Avalonia control that renders Mermaid source to SVG |
| `Novolis.CodeGen.Reflection.ClassDiagram` | Class diagrams from .NET types (codegen repo) |

## More documentation

- [Getting started](https://github.com/Novolis-Platform/novolis-markup/blob/main/docs/getting-started.md)
- [Design](https://github.com/Novolis-Platform/novolis-markup/blob/main/docs/design.md)
- [Novolis.Markup.Markdown README](https://github.com/Novolis-Platform/novolis-markup/blob/main/src/Novolis.Markup.Markdown/README.md)

## Support

Pre-release platform package; API may evolve with Novolis governance releases.

