namespace Novolis.Markup.Markdown.Tests;

public sealed class MarkdownHtmlActionUrisTests
{
    [Test]
    public async Task TryRead_CopyAndPreview()
    {
        await Assert.That(MarkdownHtmlActionUris.TryRead(
            new Uri("novolis-md://copy/3"),
            out var copyKind,
            out var copyIndex)).IsTrue();
        await Assert.That(copyKind).IsEqualTo(MarkdownHtmlActionKind.Copy);
        await Assert.That(copyIndex).IsEqualTo(3);

        await Assert.That(MarkdownHtmlActionUris.TryRead(
            new Uri("novolis-md://preview/0"),
            out var previewKind,
            out var previewIndex)).IsTrue();
        await Assert.That(previewKind).IsEqualTo(MarkdownHtmlActionKind.Preview);
        await Assert.That(previewIndex).IsEqualTo(0);

        await Assert.That(MarkdownHtmlActionUris.TryRead(
            MarkdownHtmlActionUris.Preview(4),
            out var aboutKind,
            out var aboutIndex)).IsTrue();
        await Assert.That(aboutKind).IsEqualTo(MarkdownHtmlActionKind.Preview);
        await Assert.That(aboutIndex).IsEqualTo(4);
    }

    [Test]
    public async Task TryRead_RejectsOtherSchemes()
    {
        await Assert.That(MarkdownHtmlActionUris.TryRead(new Uri("https://example.com/copy/0"), out _, out _))
            .IsFalse();
    }
}
