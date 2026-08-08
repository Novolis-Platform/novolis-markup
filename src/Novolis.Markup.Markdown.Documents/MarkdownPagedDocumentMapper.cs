using System.Text;
using System.Text.RegularExpressions;
using Novolis.Documents;
using Novolis.Markup.Markdown;
using DocHeading = Novolis.Documents.HeadingBlock;
using DocIBlock = Novolis.Documents.IBlock;
using DocParagraph = Novolis.Documents.ParagraphBlock;

namespace Novolis.Markup.Markdown.Documents;

/// <summary>Maps <see cref="IMarkdownDocument"/> into a <see cref="PagedDocument"/>.</summary>
public static class MarkdownPagedDocumentMapper
{
    static readonly Regex CalloutRegex = new(
        @"^\[!([A-Za-z0-9_-]+)\]\s*(.*)$",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);

    /// <summary>Maps a fluent Markdown document to a paged document model.</summary>
    public static PagedDocument FromDocument(IMarkdownDocument document, MarkdownPagedExportOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(document);
        options ??= new MarkdownPagedExportOptions();
        var title = options.Title ?? InferTitle(document) ?? "Document";
        var body = MapBody(document, options);

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
            Header = string.IsNullOrWhiteSpace(options.HeaderTemplate)
                ? null
                : new Header
                {
                    Template = options.HeaderTemplate,
                    IncludeBody = true,
                    UseChapterTitle = options.UseChapterTitleHeader,
                },
            Footer = string.IsNullOrWhiteSpace(options.FooterTemplate)
                ? null
                : new Footer
                {
                    Template = options.FooterTemplate,
                    IncludeBody = true,
                    IncludeFirstPage = options.FooterOnFirstPage,
                    IncludeToc = options.FooterOnToc,
                    IncludeLastPage = options.FooterOnLastPage,
                },
            Body = body,
        };
    }

    /// <summary>
    /// Maps raw Markdown via <see cref="MarkdownDocument.Parse"/> (Novolis Markdown, not Markdig).
    /// Prefer <see cref="FromDocument"/> for fluent / faithful documents.
    /// </summary>
    public static PagedDocument FromMarkdown(string markdown, MarkdownPagedExportOptions? options = null) =>
        FromDocument(MarkdownDocument.Parse(markdown ?? string.Empty), options);

    static IReadOnlyList<DocIBlock> MapBody(IMarkdownDocument document, MarkdownPagedExportOptions options)
    {
        var blocks = new List<DocIBlock>();
        var pendingMeta = new List<(string Tag, string Value)>();
        var style = options.TextBox;

        void FlushMeta()
        {
            if (pendingMeta.Count == 0)
                return;
            var lines = BuildDatelineLines(pendingMeta);
            pendingMeta.Clear();
            if (lines.Count == 0)
                return;

            // Reader-facing callout panel → fundamental TextBox (style from export options).
            blocks.Add(new TextBoxBlock
            {
                Lines = lines,
                PaddingPt = style.PaddingPt,
                BorderStrokePt = style.BorderStrokePt,
                BorderColor = style.BorderColor,
                Background = style.Background,
                FontSizePt = style.FontSizePt,
                LineHeight = style.LineHeight,
                LineGapPt = style.LineGapPt,
                TextColor = style.TextColor,
            });
        }

        foreach (var section in document)
        {
            if (TryParseMetadataCallout(section, out var tag, out var value))
            {
                pendingMeta.Add((tag, value));
                continue;
            }

            FlushMeta();
            AppendSection(blocks, section);
        }

        FlushMeta();
        return blocks;
    }

    static bool TryParseMetadataCallout(IMarkdownSection section, out string tag, out string value)
    {
        tag = string.Empty;
        value = string.Empty;
        var text = section switch
        {
            IMarkdownAlert alert => string.Join(' ', alert.Text).Trim(),
            IMarkdownQuote quote => string.Join(' ', quote.Text).Trim(),
            _ => null,
        };
        if (text is null)
            return false;

        var m = CalloutRegex.Match(text);
        if (!m.Success)
            return false;

        tag = m.Groups[1].Value;
        value = m.Groups[2].Value.Trim();
        return true;
    }

    /// <summary>Reader-style dateline: merge adjacent date+time; omit empty values; drop tag labels.</summary>
    internal static List<string> BuildDatelineLines(IReadOnlyList<(string Tag, string Value)> rows)
    {
        var lines = new List<string>();
        var i = 0;
        while (i < rows.Count)
        {
            var (tag, val) = rows[i];
            if (string.IsNullOrWhiteSpace(val))
            {
                i++;
                continue;
            }

            var tl = tag.ToLowerInvariant();
            if (tl == "date" && i + 1 < rows.Count
                && rows[i + 1].Tag.Equals("time", StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrWhiteSpace(rows[i + 1].Value))
            {
                lines.Add($"{val} {rows[i + 1].Value}");
                i += 2;
            }
            else if (tl == "time" && i + 1 < rows.Count
                     && rows[i + 1].Tag.Equals("date", StringComparison.OrdinalIgnoreCase)
                     && !string.IsNullOrWhiteSpace(rows[i + 1].Value))
            {
                lines.Add($"{rows[i + 1].Value} {val}");
                i += 2;
            }
            else
            {
                lines.Add(val);
                i++;
            }
        }

        return lines;
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
                {
                    var lines = code.Code.Replace("\r\n", "\n").TrimEnd().Split('\n');
                    blocks.Add(new CodeBlock
                    {
                        Lines = lines,
                        Language = string.IsNullOrWhiteSpace(code.Language) ? null : code.Language,
                    });
                }
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
                    if (string.IsNullOrWhiteSpace(item))
                        continue;
                    var depth = MarkdownDocument.DecodeNestDepth(item, out var body);
                    if (string.IsNullOrWhiteSpace(body))
                        continue;
                    // Hanging indent: 3 spaces per nest level before the bullet.
                    blocks.Add(new DocParagraph { Text = new string(' ', depth * 3) + "• " + body.Trim() });
                }
                break;

            case IMarkdownOrderedList list:
                var order = 1;
                foreach (var item in list.Items)
                {
                    if (string.IsNullOrWhiteSpace(item))
                        continue;
                    var depth = MarkdownDocument.DecodeNestDepth(item, out var body);
                    if (string.IsNullOrWhiteSpace(body))
                        continue;
                    blocks.Add(new DocParagraph { Text = new string(' ', depth * 3) + $"{order++}. {body.Trim()}" });
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
            RuleStyle = TableRuleStyle.Grid,
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
