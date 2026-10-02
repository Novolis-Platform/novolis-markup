using Novolis.Markup.Markdown.Mermaid.Rendering;

namespace Novolis.Markup.Markdown.Tests;

public sealed class MermaidMarkdownHtmlRendererTests
{
    [Test]
    public async Task ToHtml_RendersMermaidFenceAsSvgDataImage()
    {
        const string markdown = """
            # Diagram

            ```mermaid
            flowchart LR
                A --> B
            ```
            """;

        var html = MermaidMarkdownHtmlRenderer.ToHtml(markdown);

        await Assert.That(html).Contains("<h1>Diagram</h1>");
        await Assert.That(html).Contains("class=\"mermaid-diagram\"");
        await Assert.That(html).Contains("data:image/svg+xml;base64,");
        await Assert.That(html).DoesNotContain("<script");
    }

    [Test]
    public async Task ToHtml_LeavesOrdinaryCodeFenceAsCode()
    {
        const string markdown = """
            ```csharp
            var answer = 42;
            ```
            """;

        var html = MermaidMarkdownHtmlRenderer.ToHtml(markdown);

        await Assert.That(html).Contains("<pre><code class=\"language-csharp\">var answer = 42;</code></pre>");
    }

    [Test]
    public async Task ToHtml_CanSkipMermaidAndKeepFenceAsCode()
    {
        const string markdown = """
            ```mermaid
            flowchart LR
                A --> B
            ```
            """;

        var html = MermaidMarkdownHtmlRenderer.ToHtml(markdown, renderMermaid: false);

        await Assert.That(html).Contains("language-mermaid");
        await Assert.That(html).DoesNotContain("data:image/svg+xml;base64,");
    }

    [Test]
    public async Task ToHtml_FallsBackToCodeWhenMermaidCannotRender()
    {
        const string markdown = """
            ```mermaid
            definitely not valid mermaid {{{
            ```
            """;

        var html = MermaidMarkdownHtmlRenderer.ToHtml(markdown);

        await Assert.That(html).Contains("language-mermaid");
        await Assert.That(html).Contains("definitely not valid mermaid");
    }

    [Test]
    public async Task ToHtml_AcceptsInfoStringAfterFenceSpace()
    {
        const string markdown = """
            ``` mermaid
            flowchart LR
                A --> B
            ```

            ``` csharp
            var answer = 42;
            ```
            """;

        var built = MermaidMarkdownHtmlRenderer.Build(markdown);

        await Assert.That(built.Document).Contains("class=\"mermaid-diagram");
        await Assert.That(built.Document).Contains("data:image/svg+xml;base64,");
        await Assert.That(built.Document).Contains("language-csharp");
        await Assert.That(built.Actions.Previews.Count).IsEqualTo(1);
    }

    [Test]
    public async Task Build_RendersPresenceLedgerSpecWhenPresent()
    {
        const string path = @"C:\Users\frank\Downloads\Presence-Ledger-Spec.md";
        if (!File.Exists(path))
            return;

        var markdown = await File.ReadAllTextAsync(path);
        var built = MermaidMarkdownHtmlRenderer.Build(markdown, title: "Presence-Ledger-Spec.md");

        await Assert.That(built.Document).Contains("<h1>Presence Ledger</h1>");
        await Assert.That(built.Document).Contains("code-block-copy");
        await Assert.That(built.Actions.Previews.Count).IsGreaterThanOrEqualTo(1);
        await Assert.That(built.Actions.CodeBlocks.Count).IsGreaterThanOrEqualTo(1);
    }

    [Test]
    public async Task Build_AddsCopyAndFullscreenChrome()
    {
        const string markdown = """
            ```csharp
            var answer = 42;
            ```

            ```mermaid
            flowchart LR
                A --> B
            ```
            """;

        var built = MermaidMarkdownHtmlRenderer.Build(markdown);

        await Assert.That(built.Document).Contains("code-block-copy");
        await Assert.That(built.Document).Contains("about:novolis-md/copy/0");
        await Assert.That(built.Document).Contains("media-preview-open");
        await Assert.That(built.Document).Contains("about:novolis-md/preview/0");
        await Assert.That(built.Actions.CodeBlocks[0]).Contains("var answer = 42;");
        await Assert.That(built.Actions.Previews[0].DataUri).StartsWith("data:image/svg+xml;base64,");
    }
}
