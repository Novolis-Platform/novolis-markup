# novolis-markup

Programmatic Markdown and Mermaid text builders for .NET, migrated from [Frank.Markdown](https://github.com/frankhaugen/Frank.Markdown) and [Frank.Mermaid](https://github.com/frankhaugen/Frank.Mermaid) (wave 10).

## Packages

| Package | Description |
|---------|-------------|
| `Novolis.Markup.Markdown` | Fluent GitHub-flavored Markdown document builder |
| `Novolis.Markup.Mermaid` | Fluent Mermaid diagram syntax builder (flowchart, sequence, gantt, …) |

Reflection-based class diagrams remain in `Novolis.CodeGen.Reflection.Mermaid` ([novolis-codegen](https://github.com/Novolis-Platform/novolis-codegen)).

## Build

```bash
dotnet build Novolis.Markup.slnx
dotnet test Novolis.Markup.slnx
```
