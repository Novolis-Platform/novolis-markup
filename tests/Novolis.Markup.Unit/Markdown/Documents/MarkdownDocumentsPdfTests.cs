using Novolis.Documents;
using Novolis.Markup.Markdown;
using Novolis.Markup.Markdown.Documents;
using TUnit.Core;

namespace Novolis.Markup.Unit.Markdown.Documents;

public sealed class MarkdownDocumentsPdfTests
{
    static IMarkdownDocument SampleDocument() => new MarkdownDocument()
        .WithHeader("Duckville Harbor")
        .With(new MarkdownParagraph().WithText("Freight bells marked the hour across the strait."))
        .WithHeader("Quay-side", 2)
        .With(new MarkdownParagraph().WithText("Spray bit the quay stones while fog settled low."))
        .With(new MarkdownHorizontalRule())
        .WithHeader("Lien notes", 3)
        .WithUnorderedList("Bonded cargo waited in the shed", "The chart was folded twice")
        .WithOrderedList("Check the manifest", "Cast off at first light");

    [Test]
    public async Task Mapper_maps_headings_scene_break_and_lists()
    {
        var doc = MarkdownPagedDocumentMapper.FromDocument(SampleDocument(), new MarkdownPagedExportOptions
        {
            Title = "Duckville Harbor",
            Author = "Novolis",
            IncludeCover = true,
            IncludeToc = true,
        });

        await Assert.That(doc.Meta.Title).IsEqualTo("Duckville Harbor");
        await Assert.That(doc.Body.OfType<HeadingBlock>().Count()).IsEqualTo(3);
        await Assert.That(doc.Body.OfType<SceneBreakBlock>().Count()).IsEqualTo(1);
        await Assert.That(doc.Body.OfType<ParagraphBlock>().Any(p => p.Text.StartsWith('•'))).IsTrue();
        await Assert.That(doc.Body.OfType<ParagraphBlock>().Any(p => p.Text.StartsWith("1."))).IsTrue();
    }

    [Test]
    public async Task Mapper_groups_chapter_callouts_into_dateline_table()
    {
        var md = """
            # Chapter 144 - Dress Uniform
            > [!date] 2497.110
            > [!time] 17:40
            > [!system] System Y982283
            > [!location] Earth Fleet battlecruiser, Quartermaster's office

            The door to the quartermaster's office slid open.
            """;

        var doc = MarkdownPagedDocumentMapper.FromMarkdown(md, new MarkdownPagedExportOptions
        {
            Title = "Calypso",
            IncludeCover = false,
            IncludeToc = false,
        });

        await Assert.That(doc.Header).IsNotNull();
        await Assert.That(doc.Header!.UseChapterTitle).IsTrue();

        var box = doc.Body.OfType<TextBoxBlock>().FirstOrDefault();
        await Assert.That(box).IsNotNull();
        await Assert.That(box!.BorderStrokePt).IsGreaterThan(0f);
        await Assert.That(box.Lines.ToArray())
            .IsEquivalentTo(["2497.110 17:40", "System Y982283", "Earth Fleet battlecruiser, Quartermaster's office"]);
    }

    [Test]
    public async Task Mapper_groups_plain_public_quotes_into_textbox()
    {
        var md = """
            # Chapter 112 - The Wizard
            > 2496.349
            > Centralis Omnis System
            > The Hub, Maintenance Corridor E-17, bulkhead 3

            Ryn rounded the corner.
            """;

        var doc = MarkdownPagedDocumentMapper.FromMarkdown(md, new MarkdownPagedExportOptions
        {
            Title = "Calypso",
            IncludeCover = false,
            IncludeToc = false,
        });

        var box = doc.Body.OfType<TextBoxBlock>().FirstOrDefault();
        await Assert.That(box).IsNotNull();
        await Assert.That(box!.Lines.ToArray())
            .IsEquivalentTo([
                "2496.349",
                "Centralis Omnis System",
                "The Hub, Maintenance Corridor E-17, bulkhead 3"]);
    }

    [Test]
    public async Task Mapper_textbook_warning_becomes_colored_textbox()
    {
        var md = """
            # Chapter 2 - Git
            > **Warning**: `git add .` stages everything.

            Prose continues.
            """;

        var textbook = MarkdownPagedDocumentMapper.FromMarkdown(md, new MarkdownPagedExportOptions
        {
            Title = "Intro",
            UseTextbookChrome = true,
            EnableChapterDatelineBoxes = false,
        });
        var warning = textbook.Body.OfType<TextBoxBlock>().Single();
        await Assert.That(warning.Lines[0].StartsWith("Warning —", StringComparison.Ordinal)).IsTrue();
        await Assert.That(warning.AccentColor).IsEqualTo(DocumentColor.Parse("#e67e22"));
        await Assert.That(warning.Background).IsEqualTo(DocumentColor.Parse("#fef5e7"));

        var fiction = MarkdownPagedDocumentMapper.FromMarkdown(md, new MarkdownPagedExportOptions
        {
            Title = "Intro",
            UseTextbookChrome = false,
        });
        await Assert.That(fiction.Body.OfType<TextBoxBlock>().Any()).IsFalse();
        await Assert.That(fiction.Body.OfType<ParagraphBlock>().Any(p =>
            p.Text.Contains("Warning", StringComparison.OrdinalIgnoreCase))).IsTrue();
    }

    [Test]
    public async Task Mapper_textbook_code_uses_accent_chrome_and_h4()
    {
        var md = """
            # Chapter
            #### Nested topic
            ```csharp
            Console.WriteLine("hi");
            ```
            """;

        var doc = MarkdownPagedDocumentMapper.FromMarkdown(md, new MarkdownPagedExportOptions
        {
            UseTextbookChrome = true,
            EnableChapterDatelineBoxes = false,
            ShowCodeLineNumbers = true,
            HighlightCode = true,
            Typography = new Typography { CodeFontFamily = "Consolas" },
        });

        await Assert.That(doc.Body.OfType<HeadingBlock>().Any(h => h.Level == 4)).IsTrue();
        var code = doc.Body.OfType<CodeBlock>().Single();
        await Assert.That(code.AccentBorderLeftPt).IsEqualTo(3f);
        await Assert.That(code.AccentColor).IsEqualTo(DocumentColor.Parse("#4a90e2"));
        await Assert.That(code.Background).IsEqualTo(DocumentColor.Parse("#f8f8f8"));
        await Assert.That(code.ShowLineNumbers).IsTrue();
        await Assert.That(code.StyledLines).IsNotNull();
        await Assert.That(code.StyledLines!.Count).IsEqualTo(1);
        await Assert.That(code.StyledLines[0].Spans.Any(s =>
            s.Text.Contains("Console", StringComparison.Ordinal)
            || s.Color == DocumentColor.Parse("#267f99")
            || s.Color == DocumentColor.Parse("#0550ae"))).IsTrue();
    }

    [Test]
    public async Task Highlighter_colors_csharp_keywords_strings_and_comments()
    {
        var lines = CodeSyntaxHighlighter.Highlight(
            """
            // note
            public class Foo { string s = "hi"; }
            """,
            "csharp");

        await Assert.That(lines.Count).IsEqualTo(2);
        await Assert.That(lines[0].Spans[0].Color).IsEqualTo(DocumentColor.Parse("#6a737d"));
        var joined = string.Concat(lines[1].Spans.Select(s => s.Text));
        await Assert.That(joined).Contains("public class Foo");
        await Assert.That(lines[1].Spans.Any(s => s.Text == "public" && s.Color == DocumentColor.Parse("#0550ae"))).IsTrue();
        await Assert.That(lines[1].Spans.Any(s => s.Text.Contains("\"hi\"", StringComparison.Ordinal) && s.Color == DocumentColor.Parse("#a31515"))).IsTrue();
    }

    [Test]
    public async Task Exporter_writes_multipage_pdf_bytes()
    {
        var longDoc = new MarkdownDocument()
            .WithHeader("Chapter 1 - Arrival")
            .With(new MarkdownParagraph().WithText(string.Join(' ', Enumerable.Repeat("harbor", 400))))
            .WithHeader("Chapter 2 - Manifest")
            .With(new MarkdownParagraph().WithText(string.Join(' ', Enumerable.Repeat("freight", 400))))
            .WithHeader("Chapter 3 - Departure")
            .With(new MarkdownParagraph().WithText(string.Join(' ', Enumerable.Repeat("tide", 400))));

        var bytes = MarkdownDocumentPdfExporter.ExportToBytes(longDoc, new MarkdownPagedExportOptions
        {
            Title = "Duckville Harbor",
            Subtitle = "A NearSol Freighter Chronicle",
            Series = "Calypso Tramp Stories",
            Author = "Novolis",
            IncludeCover = true,
            IncludeToc = true,
        });

        await Assert.That(bytes.Length).IsGreaterThan(1000);
        await Assert.That(bytes.Length).IsLessThan(80_000);
        await Assert.That(bytes[0]).IsEqualTo((byte)'%');
        await Assert.That(bytes[1]).IsEqualTo((byte)'P');
    }
}
