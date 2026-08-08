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
    public async Task ToHtml_RendersFencedCode()
    {
        var html = NovolisMarkdownRenderer.ToHtml("```csharp\nvar x = 1;\n```");
        await Assert.That(html).Contains("<pre");
        await Assert.That(html).Contains("var x = 1;");
    }
}
