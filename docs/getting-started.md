# Getting started

**novolis-markup** provides fluent builders for GitHub-flavored Markdown and Mermaid diagram syntax on .NET 10.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

## Install

Pick one or both packages:

```bash
dotnet add package Novolis.Markup.Markdown
dotnet add package Novolis.Markup.Mermaid
```

## Markdown quick start

```csharp
using Novolis.Markup.Markdown;

var doc = new MarkdownDocument()
    .WithHeader("Hello", MarkdownHeaderLevel.H1)
    .WithParagraph(new MarkdownParagraph().WithText("Build docs in code."))
    .WithUnorderedList("First", "Second");

string markdown = doc.ToString();
string html = doc.ToHtml();
```

## Mermaid quick start

```csharp
using Novolis.Markup.Mermaid;

var flowchart = new Flowchart(Direction.LeftToRight);
var a = new Node("Start");
var b = new Node("End");
flowchart.AddNode(a);
flowchart.AddNode(b);
flowchart.AddLink(new Link(a, b));

string mermaid = flowchart.GetMermaidString();
```

Render the returned text with [Mermaid](https://mermaid.js.org/) in a browser, wiki, or documentation site.

## See also

- [Design](design.md)
- [Release](release.md)
- [Novolis.Markup.Markdown README](../src/Novolis.Markup.Markdown/README.md)
- [Novolis.Markup.Mermaid README](../src/Novolis.Markup.Mermaid/README.md)
