using Novolis.Markup.Markdown;

namespace Novolis.Markup.Unit;

public sealed class MarkdownParagraphItemTests
{
    [Test]
    public async Task ToString_WrapsBoldItalicAndLinks()
    {
        await Assert.That(new MarkdownParagraphItem("x", MarkdownParagraphItemType.Bold).ToString()).IsEqualTo("**x**");
        await Assert.That(new MarkdownParagraphItem("y", MarkdownParagraphItemType.Italic).ToString()).IsEqualTo("*y*");
        await Assert.That(new MarkdownParagraphItem("z", MarkdownParagraphItemType.Strikethrough).ToString()).IsEqualTo("~~z~~");
        await Assert.That(new MarkdownParagraphItem("u", MarkdownParagraphItemType.Underline).ToString()).IsEqualTo("__u__");
        await Assert.That(new MarkdownParagraphItem("t", MarkdownParagraphItemType.Text).ToString()).IsEqualTo("t");
        await Assert.That(new MarkdownParagraphItem("c", MarkdownParagraphItemType.Code).ToString()).IsEqualTo("`c`");
    }

    [Test]
    public async Task ToString_FormatsLinkParts()
    {
        var linkText = new MarkdownParagraphItem("Docs", MarkdownParagraphItemType.LinkText);
        var linkUrl = new MarkdownParagraphItem("https://example.com", MarkdownParagraphItemType.Link);
        await Assert.That(linkText.ToString()).IsEqualTo("(Docs)");
        await Assert.That(linkUrl.ToString()).IsEqualTo("[https://example.com]");
    }

    [Test]
    public async Task Paragraph_BuilderComposesMixedInline()
    {
        var p = new MarkdownParagraph();
        p.WithBold("bold");
        p.WithText(" and ");
        p.WithItalic("em");
        p.WithLink("site", "https://x.test");
        var rendered = string.Concat(p.Items.Select(i => i.ToString()));
        await Assert.That(rendered).Contains("**bold**");
        await Assert.That(rendered).Contains("*em*");
        await Assert.That(rendered).Contains("[https://x.test]");
    }
}
