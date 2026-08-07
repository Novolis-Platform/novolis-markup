<!-- novolis-pkg-brand:start -->
<p align="center">
  <a href="https://github.com/Novolis-Platform/novolis-markup">
    <img src="https://raw.githubusercontent.com/Novolis-Platform/.github/main/brand/logo-icon.svg" width="72" alt="Novolis"/>
  </a>
</p>
<!-- novolis-pkg-brand:end -->

# Novolis.Markup.Manuscript.LegacyBooks

Isolated adapter for the pre-NMP layout (`content/series`, `content/books`, lowercase `chapters`, `references`/`reference`, callouts, `chapter_order_from_heading`). Returns the same `ManuscriptSnapshot` as NMP/1 without changing Protocol rules.

## Mapping notes

- Fiction series under `content/series/<id>` are placed under a synthetic universe id `legacy` (title `Legacy`). Protocol itself still requires real `universe.yaml` for NMP/1 trees.
- Standalone books under `content/books/<id>` become NonFiction under synthetic subject id `legacy` (title `Legacy`).
- Renderer flags such as `debug_mode` are ignored for the protocol catalog.

## Quick start

```csharp
using Novolis.Markup.Manuscript.LegacyBooks;

var snapshot = new LegacyBooksCatalogReader().Read(root);
```
