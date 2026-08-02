using Novolis.Markup.Markdown.Rendering;

namespace Novolis.Markup.Unit;

public sealed class MarkdownPdfExporterTests
{
    [Test]
    public async Task ExportToBytes_RendersHeadingsListsAndCode()
    {
        var markdown = """
            # Title

            Intro paragraph.

            > Quoted wisdom

            - item one
            - item two

            1. first
            2. second

            ```csharp
            var x = 1;
            ```

            ---

            **Bold** tail.
            """;

        var bytes = MarkdownPdfExporter.ExportToBytes(markdown, new MarkdownPdfExportOptions
        {
            Title = "Test Doc",
            Subtitle = "Sub",
            Author = "Author",
            IncludeCoverPage = true,
        });

        await Assert.That(bytes.Length).IsGreaterThan(500);
        await Assert.That(bytes[0]).IsEqualTo((byte)'%');
    }

    [Test]
    public async Task ExportToFile_WritesPdf()
    {
        var path = Path.Combine(Path.GetTempPath(), $"md-pdf-{Guid.NewGuid():N}.pdf");
        try
        {
            MarkdownPdfExporter.ExportToFile("# Hello\n\nBody.", path);
            await Assert.That(File.Exists(path)).IsTrue();
            await Assert.That(new FileInfo(path).Length).IsGreaterThan(100);
        }
        finally
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }

    [Test]
    public async Task EnsureCommunityLicense_AllowsSubsequentExport()
    {
        MarkdownPdfExporter.EnsureCommunityLicense();
        var bytes = MarkdownPdfExporter.ExportToBytes("# ok");
        await Assert.That(bytes.Length).IsGreaterThan(50);
    }
}
