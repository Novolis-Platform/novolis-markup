<!-- novolis-pkg-brand:start -->
<p align="center">
  <a href="https://github.com/Novolis-Platform/novolis-markup">
    <img src="https://raw.githubusercontent.com/Novolis-Platform/.github/main/brand/logo-icon.svg" width="72" alt="Novolis"/>
  </a>
</p>
<!-- novolis-pkg-brand:end -->

# Novolis.Markup.Html

Fluent HTML, CSS, JavaScript (functions and classes), and SVG builders for .NET.

## Install

```bash
dotnet add package Novolis.Markup.Html
```

**Prerequisites:** [.NET 10 SDK](https://dotnet.microsoft.com/download) (`net10.0`).

## Quick start

```csharp
using Novolis.Markup.Html;

var page = HtmlMarkup.Document(doc => doc
    .Lang("en")
    .CharsetUtf8()
    .Viewport()
    .Title("Hello")
    .StyleSheet(css => css
        .Rule("body", r => r
            .Margin(0)
            .FontFamily("Georgia, serif")
            .BackgroundColor("#0b1c2c")
            .Color("#e8f1f5"))
        .Rule(".hero", r => r
            .Flex()
            .JustifyContent("center")
            .Padding("2rem")))
    .WithBody(body => body
        .Div(d => d.Class("hero")
            .H1("Novolis")
            .P("Fluent markup.")
            .Svg(svg => svg
                .Width(120).Height(120).ViewBox(0, 0, 120, 120)
                .Circle(60, 60, 48, c => c.Attr("fill", "#1a9b8e")))))
    .Script(js => js
        .Function("greet", ["name"], body => body.Return("`Hello, ${name}`"))
        .Class("App", c => c
            .Constructor([], "this.ready = true;")
            .Method("run", [], "greet('world');"))));

File.WriteAllText("index.html", page.ToString());
```

## Surfaces

| API | Role |
|-----|------|
| `HtmlMarkup` / `HtmlDocument` / `HtmlElement` | HTML5 documents and elements |
| `HtmlFragment` | Sibling nodes without a wrapper |
| `Css.CssStylesheet` / `CssRule` | Common layout, flex/grid, type, color, border props |
| `Js.JsScript` / `JsClass` | Function and class declarations (intentionally narrow) |
| `Svg.SvgRoot` | SVG shapes, text, gradients, groups |

## Factories

```csharp
HtmlMarkup.H2("Title");
HtmlMarkup.Ul(["one", "two"]);
HtmlMarkup.Ol(["a", "b"]);
HtmlMarkup.Table(["Col"], [["cell"]]);
HtmlMarkup.PreCode("var x = 1;", "csharp");
HtmlMarkup.Alert("Warning", "Careful");
HtmlMarkup.A("https://example.com", "Example");
HtmlMarkup.Fragment(HtmlMarkup.H1("Hi"), HtmlMarkup.P("Body"));
```

`Novolis.Markup.Markdown` converts documents through these factories (`MarkdownToHtmlConverter`).

Text nodes and attributes are escaped; use `Raw` only for trusted fragments (or for CSS/JS already produced by the builders).
