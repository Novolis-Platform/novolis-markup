namespace Novolis.Markup.Markdown.Tests;

public sealed class MarkdownDocumentParseTests
{
    [Test]
    public async Task Parse_HeaderParagraphAndList()
    {
        var md = "# Title\n\nHello world.\n\n- one\n- two";
        var doc = MarkdownDocument.Parse(md);
        var text = doc.ToString();

        await Assert.That(text).Contains("# Title");
        await Assert.That(text).Contains("Hello world.");
        await Assert.That(text).Contains("- one");
        await Assert.That(text).Contains("- two");
    }

    [Test]
    public async Task Parse_QuoteAndOrderedList()
    {
        var md = "> quoted line\n\n1. first\n2. second";
        var doc = MarkdownDocument.Parse(md);
        var text = doc.ToString();

        await Assert.That(text).Contains("> quoted line");
        await Assert.That(text).Contains("1. first");
        await Assert.That(text).Contains("2. second");
    }

    [Test]
    public async Task Parse_Table()
    {
        var md = "| A | B |\n| 1 | 2 |";
        var doc = MarkdownDocument.Parse(md);
        var text = doc.ToString();

        await Assert.That(text).Contains("| A | B |");
        await Assert.That(text).Contains("| 1 | 2 |");
    }

    [Test]
    public async Task Create_FromSectionsAndStrings()
    {
        var doc = MarkdownDocument.Create("alpha", "beta");
        await Assert.That(doc.ToString()).Contains("alpha");
        await Assert.That(doc.ToString()).Contains("beta");
    }

    [Test]
    public async Task With_MultipleSections_ReturnsSameDocument()
    {
        var doc = new MarkdownDocument();
        var result = doc.With([new MarkdownHeader("H"), new MarkdownParagraph().WithText("body")]);
        await Assert.That(result).IsEqualTo(doc);
        await Assert.That(doc.ToString()).Contains("# H");
        await Assert.That(doc.ToString()).Contains("body");
    }
}
