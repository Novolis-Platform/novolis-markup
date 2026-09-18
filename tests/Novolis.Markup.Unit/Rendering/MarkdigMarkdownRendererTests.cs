using Novolis.Markup.Markdown.Rendering;

namespace Novolis.Markup.Markdown.Rendering.Tests;

public class NovolisMarkdownRendererTests
{
    [Test]
    public async Task ToHtml_RendersHeading()
    {
        var html = NovolisMarkdownRenderer.ToHtml("# Title");
        await Assert.That(html).Contains("<h1");
        await Assert.That(html).Contains("Title");
    }

    [Test]
    public async Task FromMarkdown_WrapsDocument()
    {
        var html = MarkdownHtmlDocument.FromMarkdown("**bold**", MarkdownHtmlTheme.StudioDark);
        await Assert.That(html).Contains("<!DOCTYPE html>");
        await Assert.That(html).Contains("<strong>bold</strong>");
    }

    [Test]
    public async Task FromMarkdown_IncludesViewportAndViewerOverlay()
    {
        var html = MarkdownHtmlDocument.FromMarkdown("# Hi", MarkdownHtmlTheme.GitHubLight);
        await Assert.That(html).Contains("name=\"viewport\"");
        await Assert.That(html).Contains("width=device-width");
        await Assert.That(html).Contains(".mermaid-diagram");
        await Assert.That(html).Contains("overflow-x: auto");
        await Assert.That(html).Contains(".mermaid-diagram img { display: block; max-width: none; height: auto; }");
        await Assert.That(html).Contains("white-space: nowrap");
        await Assert.That(html).Contains("data-theme=\"light\"");
        await Assert.That(html).Contains("color-scheme: light");
    }

    [Test]
    public async Task FromMarkdown_GitHubDark_DoesNotRelyOnPrefersColorScheme()
    {
        var html = MarkdownHtmlDocument.FromMarkdown("# Hi", MarkdownHtmlTheme.GitHubDark);
        await Assert.That(html).Contains("data-theme=\"dark\"");
        await Assert.That(html).Contains("color-scheme: dark");
        await Assert.That(html).Contains("background: #0d1117");
        await Assert.That(html).Contains("name=\"color-scheme\"");
    }

    [Test]
    public async Task ToHtml_RendersFencedCode()
    {
        var html = NovolisMarkdownRenderer.ToHtml("```csharp\nvar x = 1;\n```");
        await Assert.That(html).Contains("<pre");
        await Assert.That(html).Contains("var x = 1;");
    }
}
