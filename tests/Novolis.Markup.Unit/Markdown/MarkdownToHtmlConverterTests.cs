namespace Novolis.Markup.Markdown.Tests;

public sealed class MarkdownToHtmlConverterTests
{
    [Test]
    public async Task Convert_HeaderParagraphListAndQuote()
    {
        var doc = MarkdownDocument.Create(
            new MarkdownHeader("Title", 2),
            new MarkdownParagraph().WithText("Hello"),
            new MarkdownUnorderedList(["a", "b"]),
            new MarkdownQuote("quoted"),
            new MarkdownHorizontalRule());

        var html = MarkdownToHtmlConverter.Convert(doc);
        await Assert.That(html).Contains("<h2>Title</h2>");
        await Assert.That(html).Contains("<p>Hello</p>");
        await Assert.That(html).Contains("<ul>");
        await Assert.That(html).Contains("<blockquote>quoted</blockquote>");
        await Assert.That(html).Contains("<hr>");
    }

    [Test]
    public async Task Convert_TableAndOrderedList()
    {
        var doc = MarkdownDocument.Create(
            new MarkdownTable<string>(["Col"], [["val"]]),
            new MarkdownOrderedList(["one", "two"]));

        var html = MarkdownToHtmlConverter.Convert(doc);
        await Assert.That(html).Contains("<table>");
        await Assert.That(html).Contains("<th>Col</th>");
        await Assert.That(html).Contains("<td>val</td>");
        await Assert.That(html).Contains("<ol>");
    }

    [Test]
    public async Task Convert_AlertAndCodeBlock()
    {
        var doc = MarkdownDocument.Create(
            new MarkdownAlert("Careful", MarkdownAlertLevel.Warning),
            new MarkdownCodeBlock("var x = 1;", "csharp"));

        var html = MarkdownToHtmlConverter.Convert(doc);
        await Assert.That(html).Contains("alert-Warning");
        await Assert.That(html).Contains("Careful");
        await Assert.That(html).Contains("<pre><code class=\"language-csharp\">var x = 1;</code></pre>");
    }

    [Test]
    public async Task Convert_InlineFormattingAndLink()
    {
        var paragraph = new MarkdownParagraph();
        paragraph.WithBold("bold");
        paragraph.WithText(" ");
        paragraph.WithItalic("ital");
        paragraph.WithText(" ");
        paragraph.WithLink("Novolis", "https://novolis.dev");
        paragraph.WithText(" ");
        paragraph.WithCode("x");

        var html = MarkdownToHtmlConverter.Convert(MarkdownDocument.Create(paragraph));
        await Assert.That(html).Contains("<strong>bold</strong>");
        await Assert.That(html).Contains("<em>ital</em>");
        await Assert.That(html).Contains("<a href=\"https://novolis.dev\">Novolis</a>");
        await Assert.That(html).Contains("<code>x</code>");
    }

    [Test]
    public async Task Convert_EscapesHtmlInText()
    {
        var doc = MarkdownDocument.Create(new MarkdownParagraph().WithText("<script>"));
        var html = MarkdownToHtmlConverter.Convert(doc);
        await Assert.That(html).Contains("&lt;script&gt;");
        await Assert.That(html).DoesNotContain("<script>");
    }

    [Test]
    public async Task ConvertNodes_ReturnsFragment()
    {
        var doc = MarkdownDocument.Create(new MarkdownHeader("Hi", 1));
        var fragment = MarkdownToHtmlConverter.ConvertNodes(doc);
        await Assert.That(fragment.ToString()).IsEqualTo("<h1>Hi</h1>");
    }
}
