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
    public async Task Parse_keeps_callouts_immediately_under_h1()
    {
        var md = """
            # Chapter 144 - Dress Uniform
            > [!date] 2497.110
            > [!time] 17:40
            > [!system] System Y982283
            > [!location] Earth Fleet battlecruiser, Quartermaster's office

            The door to the quartermaster's office slid open.
            """;
        var doc = MarkdownDocument.Parse(md);
        var text = doc.ToString();

        await Assert.That(text).Contains("# Chapter 144 - Dress Uniform");
        await Assert.That(text).Contains("> [!date] 2497.110");
        await Assert.That(text).Contains("> [!location] Earth Fleet battlecruiser, Quartermaster's office");
        await Assert.That(text).Contains("quartermaster's office slid open");
    }

    [Test]
    public async Task Parse_MergesConsecutiveQuoteLines()
    {
        var doc = MarkdownDocument.Parse("> **Target Service:** `DocFlow`\n> **Topic:** Pipeline");
        var quotes = doc.OfType<IMarkdownQuote>().ToArray();
        await Assert.That(quotes.Length).IsEqualTo(1);
        var lines = quotes[0].Text.ToArray();
        await Assert.That(lines.Length).IsEqualTo(2);
        await Assert.That(lines[0]).Contains("Target Service");
        await Assert.That(lines[1]).Contains("Topic");
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
    public async Task Parse_NestedUnorderedList_PreservesIndent()
    {
        var md = "- parent\n  - child\n    - grandchild";
        var doc = MarkdownDocument.Parse(md);
        var text = doc.ToString();

        await Assert.That(text).Contains("- parent");
        await Assert.That(text).Contains("  - child");
        await Assert.That(text).Contains("    - grandchild");
    }

    [Test]
    public async Task Parse_FencedCodeAndThematicBreak()
    {
        var md = "```csharp\nvar x = 1;\n```\n\n---\n\nAfter.";
        var doc = MarkdownDocument.Parse(md);
        var text = doc.ToString();

        await Assert.That(text).Contains("```csharp");
        await Assert.That(text).Contains("var x = 1;");
        await Assert.That(text).Contains("---");
        await Assert.That(text).Contains("After.");
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
