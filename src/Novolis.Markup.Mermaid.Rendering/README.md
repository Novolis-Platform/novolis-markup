<!-- novolis-pkg-brand:start -->
<p align="center">
  <a href="https://github.com/Novolis-Platform/novolis-markup">
    <img src="https://raw.githubusercontent.com/Novolis-Platform/.github/main/brand/logo-icon.svg" width="72" alt="Novolis"/>
  </a>
</p>
<!-- novolis-pkg-brand:end -->

# Novolis.Markup.Mermaid.Rendering

Headless Mermaid diagram export to SVG (via [Mermaider](https://www.nuget.org/packages/Mermaider)) and PNG (via Svg.Skia) for `Novolis.Markup.Mermaid` builders — no browser, no Avalonia.

## Install

```bash
dotnet add package Novolis.Markup.Mermaid.Rendering
```

**Prerequisites:** [.NET 10 SDK](https://dotnet.microsoft.com/download) (`net10.0`).

## Quick start

```csharp
using Novolis.Markup.Mermaid;
using Novolis.Markup.Mermaid.Rendering;

var chart = new Flowchart(Direction.TopToBottom);
var start = new Node("Start", Shape.Rounded);
var end = new Node("Done");
chart.AddNode(start);
chart.AddNode(end);
chart.AddLink(new Link(start, end, "next"));

string? svg = chart.ToSvg();
byte[]? png = chart.ToPng(scale: 2f);

chart.ExportSvg(@"d:\out\diagram.svg");
chart.ExportPng(@"d:\out\diagram.png", MermaidRenderTheme.GitHubLight);
```

Raw Mermaid source works the same via string extensions:

```csharp
string? svg = """
    sequenceDiagram
      A->>B: Hello
    """.ToSvg();
```

## API

| Type | Role |
|------|------|
| `MermaidRenderExtensions` | `ToSvg` / `ToPng` / `ExportSvg` / `ExportPng` on `IMermaidable` and `string` |
| `MermaidSvgRenderer` | `TryRenderSvg`, `OptionsFor` |
| `MermaidPngRenderer` | `TryRenderPng`, `TryEncodePng` |
| `MermaidRenderTheme` | `StudioDark`, `GitHubLight` |

Soft-fail: `To*` / `Try*` return `null` on blank or invalid input. `Export*` returns `false` on failure.

## Related

| Package | Role |
|---------|------|
| `Novolis.Markup.Mermaid` | Fluent Mermaid syntax builders |
| `Novolis.Avalonia.Mermaid` | Avalonia control + HTML preview helpers |

## More documentation

- [Getting started](https://github.com/Novolis-Platform/novolis-markup/blob/main/docs/getting-started.md)
- [Design](https://github.com/Novolis-Platform/novolis-markup/blob/main/docs/design.md)
