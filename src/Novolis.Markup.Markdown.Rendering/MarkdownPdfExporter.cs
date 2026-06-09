using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Novolis.Markup.Markdown.Rendering;

/// <summary>Exports Markdown source to PDF via QuestPDF.</summary>
public static class MarkdownPdfExporter
{
    /// <summary>Ensures QuestPDF community license is configured. Call once at app startup when using PDF export.</summary>
    public static void EnsureCommunityLicense() =>
        QuestPDF.Settings.License = LicenseType.Community;

    /// <summary>Exports Markdown to a PDF file.</summary>
    /// <param name="markdown">Markdown source.</param>
    /// <param name="outputPath">Destination PDF path.</param>
    /// <param name="options">Optional layout and metadata options.</param>
    public static void ExportToFile(string markdown, string outputPath, MarkdownPdfExportOptions? options = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);

        var bytes = ExportToBytes(markdown, options);
        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllBytes(outputPath, bytes);
    }

    /// <summary>Exports Markdown to PDF bytes.</summary>
    /// <param name="markdown">Markdown source.</param>
    /// <param name="options">Optional layout and metadata options.</param>
    /// <returns>PDF file content.</returns>
    public static byte[] ExportToBytes(string markdown, MarkdownPdfExportOptions? options = null)
    {
        EnsureCommunityLicense();
        options ??= new MarkdownPdfExportOptions();

        var blocks = ExtractBlocks(markdown);
        var title = options.Title ?? "Document";

        return Document.Create(container =>
        {
            if (options.IncludeCoverPage)
            {
                container.Page(page =>
                {
                    ConfigurePage(page, options);
                    page.Content().PaddingVertical(40).Column(col =>
                    {
                        col.Spacing(10);
                        col.Item().Text(title).FontSize(24).SemiBold().FontFamily(options.BodyFontFamily);
                        if (!string.IsNullOrWhiteSpace(options.Subtitle))
                            col.Item().Text(options.Subtitle).FontSize(13).FontFamily(options.BodyFontFamily);
                        if (!string.IsNullOrWhiteSpace(options.Author))
                            col.Item().PaddingTop(16).Text(options.Author).FontSize(11).FontFamily(options.BodyFontFamily);
                    });
                });
            }

            container.Page(page =>
            {
                ConfigurePage(page, options);
                page.Header().AlignCenter().Text(title).SemiBold().FontSize(9).FontFamily(options.BodyFontFamily);
                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Page ");
                    text.CurrentPageNumber();
                });

                page.Content().Column(col =>
                {
                    col.Spacing(10);
                    foreach (var block in blocks)
                        RenderBlock(col, block, options);
                });
            });
        }).GeneratePdf();
    }

    private static void ConfigurePage(PageDescriptor page, MarkdownPdfExportOptions options)
    {
        page.Size(options.PageWidthInches, options.PageHeightInches, Unit.Inch);
        page.MarginHorizontal(options.MarginHorizontalInches, Unit.Inch);
        page.MarginVertical(options.MarginVerticalInches, Unit.Inch);
        page.DefaultTextStyle(x => x
            .FontSize(options.BodyFontSize)
            .LineHeight(1.35f)
            .FontFamily(options.BodyFontFamily));
    }

    private static void RenderBlock(ColumnDescriptor col, PdfBlock block, MarkdownPdfExportOptions options)
    {
        switch (block.Kind)
        {
            case PdfBlockKind.Heading:
                col.Item().PaddingTop(6).Text(block.Text).FontSize(options.HeadingFontSize).SemiBold();
                break;
            case PdfBlockKind.Code:
                col.Item().Background(Colors.Grey.Lighten4).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10)
                    .Text(block.Text).FontFamily(options.CodeFontFamily).FontSize(options.BodyFontSize - 1);
                break;
            case PdfBlockKind.Quote:
                col.Item().BorderLeft(3).BorderColor(Colors.Grey.Medium).PaddingLeft(10)
                    .Text(block.Text).Italic().FontColor(Colors.Grey.Darken2);
                break;
            case PdfBlockKind.ListItem:
                col.Item().Row(row =>
                {
                    row.ConstantItem(14).Text("•");
                    row.RelativeItem().Text(block.Text);
                });
                break;
            case PdfBlockKind.OrderedListItem:
                col.Item().Row(row =>
                {
                    row.ConstantItem(20).Text($"{block.Order}.");
                    row.RelativeItem().Text(block.Text);
                });
                break;
            case PdfBlockKind.Paragraph:
            default:
                if (!string.IsNullOrWhiteSpace(block.Text))
                    col.Item().Text(block.Text);
                break;
        }
    }

    private static List<PdfBlock> ExtractBlocks(string markdown)
    {
        var document = Markdig.Markdown.Parse(markdown, MarkdownRenderPipeline.Default);
        var blocks = new List<PdfBlock>();

        foreach (var block in document)
        {
            switch (block)
            {
                case HeadingBlock heading:
                    blocks.Add(new PdfBlock(PdfBlockKind.Heading, InlineCollector.Collect(heading.Inline)));
                    break;
                case ParagraphBlock paragraph:
                    blocks.Add(new PdfBlock(PdfBlockKind.Paragraph, InlineCollector.Collect(paragraph.Inline)));
                    break;
                case CodeBlock code:
                    blocks.Add(new PdfBlock(PdfBlockKind.Code, code.Lines.ToString().TrimEnd()));
                    break;
                case QuoteBlock quote:
                    foreach (var quoteChild in quote)
                    {
                        if (quoteChild is ParagraphBlock quoteParagraph)
                            blocks.Add(new PdfBlock(PdfBlockKind.Quote, InlineCollector.Collect(quoteParagraph.Inline)));
                    }
                    break;
                case ListBlock list:
                    var order = 1;
                    foreach (var item in list)
                    {
                        if (item is not ListItemBlock listItem)
                            continue;

                        foreach (var listChild in listItem)
                        {
                            if (listChild is not ParagraphBlock listParagraph)
                                continue;

                            var text = InlineCollector.Collect(listParagraph.Inline);
                            blocks.Add(list.IsOrdered
                                ? new PdfBlock(PdfBlockKind.OrderedListItem, text, order++)
                                : new PdfBlock(PdfBlockKind.ListItem, text));
                        }
                    }
                    break;
                case ThematicBreakBlock:
                    blocks.Add(new PdfBlock(PdfBlockKind.Paragraph, "—"));
                    break;
            }
        }

        return blocks;
    }

    private enum PdfBlockKind
    {
        Paragraph,
        Heading,
        Code,
        Quote,
        ListItem,
        OrderedListItem,
    }

    private readonly record struct PdfBlock(PdfBlockKind Kind, string Text, int Order = 0);

    private static class InlineCollector
    {
        public static string Collect(ContainerInline? inline)
        {
            if (inline is null)
                return string.Empty;

            var builder = new System.Text.StringBuilder();
            foreach (var child in inline)
                AppendInline(builder, child);

            return builder.ToString().Trim();
        }

        private static void AppendInline(System.Text.StringBuilder builder, Inline inline)
        {
            switch (inline)
            {
                case LiteralInline literal:
                    builder.Append(literal.Content.ToString());
                    break;
                case LineBreakInline:
                    builder.Append(' ');
                    break;
                case EmphasisInline emphasis:
                    foreach (var child in emphasis)
                        AppendInline(builder, child);
                    break;
                case LinkInline link:
                    foreach (var child in link)
                        AppendInline(builder, child);
                    break;
                case CodeInline code:
                    builder.Append(code.Content);
                    break;
            }
        }
    }
}
