# Novolis.Markup.Manuscript

Chapter metadata, content catalog, structural diagnostics, and book/reference PDF export for manuscript workspaces.

## Install

```bash
dotnet add package Novolis.Markup.Manuscript
```

## Quick start

```csharp
using Novolis.Markup.Manuscript;

if (!ManuscriptWorkspace.TryOpen(startDir, out var workspace) || workspace is null)
    throw new InvalidOperationException("No manuscript workspace found.");

var issues = ManuscriptDoctor.Diagnose(workspace.ContentRoot);
var series = workspace.Catalog.Load(workspace.ContentRoot);
```
