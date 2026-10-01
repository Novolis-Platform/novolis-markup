<!-- novolis-pkg-brand:start -->
[![Novolis](https://raw.githubusercontent.com/Novolis-Platform/.github/main/brand/logo-icon.png)](https://novolis-platform.github.io/.github/novolis-markup/)

[Novolis](https://github.com/Novolis-Platform) · [Docs](https://novolis-platform.github.io/.github/novolis-markup/) · [Source](https://github.com/Novolis-Platform/novolis-markup)
<!-- novolis-pkg-brand:end -->

# Novolis.Markup.Markdown.Mermaid.Rendering

Headless Mermaid rendering for Markdown HTML output.

Fenced `mermaid` code blocks are parsed through `Novolis.Markup.Markdown`, rendered to SVG through `Novolis.Markup.Mermaid.Rendering`, and embedded as an SVG data image in the generated HTML. No browser runtime or Mermaid JavaScript is required.

## Install

```xml
<PackageReference Include="Novolis.Markup.Markdown.Mermaid.Rendering" />
```

## Quick start

```csharp
var html = MermaidMarkdownHtmlRenderer.ToHtmlDocument(
    markdown,
    MarkdownHtmlTheme.GitHubDark,
    MermaidRenderTheme.StudioDark,
    title: "Architecture");
```

Ordinary fenced code blocks retain the standard Markdown HTML representation. If a Mermaid block cannot be rendered, it falls back to a normal fenced code block instead of producing broken markup.

