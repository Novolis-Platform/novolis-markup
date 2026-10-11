<!-- novolis-pkg-brand:start -->
[![Novolis](https://raw.githubusercontent.com/Novolis-Platform/.github/main/brand/logo-icon.png)](https://novolis-platform.github.io/.github/novolis-markup/)

[Novolis](https://github.com/Novolis-Platform) · [Docs](https://novolis-platform.github.io/.github/novolis-markup/) · [Source](https://github.com/Novolis-Platform/novolis-markup)
<!-- novolis-pkg-brand:end -->

# Novolis.Markup.Svg

Type-safe, fluent SVG document and shape builders for .NET. The package is headless and BCL-only: it creates SVG markup but does not render or rasterize it.

## Install

```bash
dotnet add package Novolis.Markup.Svg
```

**Prerequisites:** [.NET 10 SDK](https://dotnet.microsoft.com/download) (`net10.0`).

## Quick start

```csharp
using Novolis.Markup.Svg;

var svg = new SvgDocument()
    .Width(SvgLength.Pixels(120))
    .Height(SvgLength.Pixels(120))
    .ViewBox(new SvgViewBox(0, 0, 120, 120))
    .Title("Example")
    .Circle(new SvgPoint(60, 60), 48, circle => circle
        .Fill(SvgPaint.Color("#1a9b8e"))
        .Stroke(SvgPaint.Color("#0b3440"))
        .StrokeWidth(3))
    .Path(
        new SvgPathData()
            .MoveTo(new SvgPoint(12, 96))
            .LineTo(new SvgPoint(60, 24))
            .LineTo(new SvgPoint(108, 96))
            .Close(),
        path => path
            .Fill(SvgPaint.None)
            .Stroke(SvgPaint.CurrentColor)
            .StrokeWidth(2)
            .StrokeLineJoin(SvgLineJoin.Round))
    .ToString();
```

The builder validates geometry and uses invariant numeric formatting. Text and attribute values are XML-escaped during serialization.

## Building blocks

| API | Role |
|-----|------|
| `SvgMarkup` / `SvgDocument` | Root document, viewport, dimensions, groups, definitions, and shapes |
| `SvgLength`, `SvgPoint`, `SvgRect`, `SvgViewBox` | Typed geometry and dimensions |
| `SvgPaint` | Paint values such as colors, `none`, `currentColor`, and URL references |
| `SvgPathData` | Fluent path commands without hand-written command strings |
| `SvgTransformList` | Typed translate, scale, rotate, and skew transforms |
| `SvgCircle`, `SvgRectangle`, `SvgPathElement`, and related elements | Typed SVG nodes |

`Novolis.Markup.Svg` is an authoring package. Mermaid generation remains in `Novolis.Markup.Mermaid.Rendering`, and SVG rasterization remains in packages that use `Svg.Skia`.
