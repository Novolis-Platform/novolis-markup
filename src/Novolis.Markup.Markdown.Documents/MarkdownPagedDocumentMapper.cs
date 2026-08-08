using System.Text;
using Novolis.Documents;
using Novolis.Markup.Markdown;
using DocHeading = Novolis.Documents.HeadingBlock;
using DocIBlock = Novolis.Documents.IBlock;
using DocParagraph = Novolis.Documents.ParagraphBlock;

namespace Novolis.Markup.Markdown.Documents;

/// <summary>Maps <see cref="IMarkdownDocument"/> into a <see cref="PagedDocument"/>.</summary>
public static class MarkdownPagedDocumentMapper
{
    /// <summary>Maps a fluent Markdown document to a paged document model.</summary>
    public static PagedDocument FromDocument(IMarkdownDocument document, MarkdownPagedExportOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(document);
        options ??= new MarkdownPagedExportOptions();
        var title = options.Title ?? InferTitle(document) ?? "Document";
        var body = MapBody(document);

        return new PagedDocument
        {
            Meta = new DocumentMeta
            {
                Title = title,
                Subtitle = options.Subtitle,
                Series = options.Series,
                Author = options.Author,
                Rights = options.Rights,
            },
            Setup = new PageSetup
            {
                Trim = options.Trim,
                Margin = options.Margin,
            },
            Typography = options.Typography,
            IncludeCover = options.IncludeCover,
            IncludeToc = options.IncludeToc,
            First = options.First,
            Last = options.Last,
            SuppressHeaderOnLevel1Open = true,
            Header = string.IsNullOrWhiteSpace(options.HeaderTemplate)
                ? null
                : new RunningChrome { Template = options.HeaderTemplate },
            Footer = string.IsNullOrWhiteSpace(options.FooterTemplate)
                ? null
                : new RunningChrome { Template = options.FooterTemplate },
            Body = body,
        };
    }

    /// <summary>
    /// Maps raw Markdown via <see cref="MarkdownDocument.Parse"/> (Novolis Markdown, not Markdig).
    /// Prefer <see cref="FromDocument"/> for fluent / faithful documents.
    /// </summary>
    public static PagedDocument FromMarkdown(string markdown, MarkdownPagedExportOptions? options = null) =>
        FromDocument(MarkdownDocument.Parse(markdown ?? string.Empty), options);

    static IReadOnlyList<DocIBlock> MapBody(IMarkdownDocument document)
    {
        var blocks = new List<DocIBlock>();
        foreach (var section in document)
            AppendSection(blocks, section);
        return blocks;
    }

    static void AppendSection(List<DocIBlock> blocks, IMarkdownSection section)
    {
        switch (section)
        {
            case IMarkdownHeader header:
                var level = System.Math.Clamp(header.Level, 1, 3);
                if (!string.IsNullOrWhiteSpace(header.Text))
                    blocks.Add(new DocHeading { Level = level, Text = header.Text.Trim() });
                break;

            case IMarkdownParagraph paragraph:
                var para = FlattenParagraph(paragraph);
                if (!string.IsNullOrWhiteSpace(para))
                    blocks.Add(new DocParagraph { Text = para });
                break;

            case IMarkdownHorizontalRule:
                blocks.Add(new SceneBreakBlock());
                break;

            case IMarkdownCodeBlock code:
                if (!string.IsNullOrWhiteSpace(code.Code))
                    blocks.Add(new DocParagraph { Text = code.Code.TrimEnd() });
                break;

            case IMarkdownAlert alert:
                var alertText = string.Join(" ", alert.Text).Trim();
                if (!string.IsNullOrWhiteSpace(alertText))
                    blocks.Add(new DocParagraph { Text = alertText });
                break;

            case IMarkdownQuote quote:
                var quoteText = string.Join(" ", quote.Text).Trim();
                if (!string.IsNullOrWhiteSpace(quoteText))
                    blocks.Add(new DocParagraph { Text = quoteText });
                break;

            case IMarkdownUnorderedList list:
                foreach (var item in list.Items)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                        blocks.Add(new DocParagraph { Text = "• " + item.Trim() });
                }
                break;

            case IMarkdownOrderedList list:
                var order = 1;
                foreach (var item in list.Items)
                {
                    if (string.IsNullOrWhiteSpace(item))
                        continue;
                    blocks.Add(new DocParagraph { Text = $"{order++}. {item.Trim()}" });
                }
                break;

            case IMarkdownTable table:
                AppendTable(blocks, table);
                break;
        }
    }

    static void AppendTable(List<DocIBlock> blocks, IMarkdownTable table)
    {
        var headers = table.Headers.Select(h => h.Trim()).Where(h => h.Length > 0).ToArray();
        var rows = table.Rows
            .Select(r => (IReadOnlyList<string>)r.Select(c => c.Trim()).ToArray())
            .Where(r => r.Count > 0)
            .ToArray();

        if (headers.Length == 0 && rows.Length == 0)
            return;

        blocks.Add(new TableBlock
        {
            Headers = headers,
            Rows = rows,
            ShowHeader = headers.Length > 0,
            DrawRules = true,
            RepeatHeaderOnPageBreak = true,
        });
    }

    static string? InferTitle(IMarkdownDocument document)
    {
        foreach (var section in document)
        {
            if (section is IMarkdownHeader { Level: 1 } h1 && !string.IsNullOrWhiteSpace(h1.Text))
                return h1.Text.Trim();
        }
        return null;
    }

    static string FlattenParagraph(IMarkdownParagraph paragraph)
    {
        var builder = new StringBuilder();
        string? pendingLinkText = null;

        foreach (var inline in paragraph.Items)
        {
            switch (inline.Type)
            {
                case MarkdownParagraphItemType.LinkText:
                    pendingLinkText = inline.Text;
                    break;
                case MarkdownParagraphItemType.Link:
                    builder.Append(pendingLinkText ?? string.Empty);
                    pendingLinkText = null;
                    break;
                case MarkdownParagraphItemType.NewLine:
                    builder.Append(' ');
                    break;
                default:
                    if (pendingLinkText is not null)
                    {
                        builder.Append(pendingLinkText);
                        pendingLinkText = null;
                    }
                    builder.Append(inline.Text);
                    break;
            }
        }

        if (pendingLinkText is not null)
            builder.Append(pendingLinkText);

        return builder.ToString().Trim();
    }
}
