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
        await Assert.That(html).Contains("<hr />");
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
        await Assert.That(html).Contains("Careful");
        await Assert.That(html).Contains("var x = 1;");
    }
}
