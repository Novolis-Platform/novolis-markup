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
