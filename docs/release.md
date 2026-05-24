# Release

Packages publish from this repo to GitHub Packages on merge to `main`.

See the [Novolis release policy](https://github.com/Novolis-Platform/novolis-governance/blob/main/docs/release-policy.md) for versioning (`2026.1.*`), feed configuration, and consumer `nuget.config` setup.

## Packages

| Package | Project |
|---------|---------|
| `Novolis.Markup.Markdown` | `src/Novolis.Markup.Markdown` |
| `Novolis.Markup.Mermaid` | `src/Novolis.Markup.Mermaid` |

## Local validation

```bash
dotnet build Novolis.Markup.slnx
dotnet test Novolis.Markup.slnx
```

Documentation completeness is gated by `build/.novolis-documentation-complete` and strict XML doc builds in CI.
