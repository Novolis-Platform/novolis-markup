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
}
