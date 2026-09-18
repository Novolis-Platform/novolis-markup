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

    static readonly Regex PublicDatelineValueRegex = new(
        @"^(\d{4}\.\d{1,4}(?:\s+\d{1,2}:\d{2})?|\d{4}-\d{2}-\d{2}(?:\s+\d{1,2}:\d{2})?|TK|TBD)$",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    static readonly Regex AdmonitionLabelRegex = new(
        @"^\*{0,2}(?<label>Warning|Tip|Fun Fact|Note|Best Practice|Rigorous Detail)\*{0,2}\s*[:\-–—]\s*(?<body>.*)$",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    static readonly DocumentColor CodeAccent = DocumentColor.Parse("#4a90e2");
    static readonly DocumentColor CodeBorder = DocumentColor.Parse("#cccccc");
    static readonly DocumentColor CodeFill = DocumentColor.Parse("#f8f8f8");
    static readonly DocumentColor NoteAccent = DocumentColor.Parse("#4a90e2");
    static readonly DocumentColor NoteFill = DocumentColor.Parse("#e8f4f8");
    static readonly DocumentColor WarningAccent = DocumentColor.Parse("#e67e22");
    static readonly DocumentColor WarningFill = DocumentColor.Parse("#fef5e7");
    static readonly DocumentColor TipAccent = DocumentColor.Parse("#27ae60");
    static readonly DocumentColor TipFill = DocumentColor.Parse("#e8f8f5");
    static readonly DocumentColor BodyInk = DocumentColor.Parse("#1a1a1a");

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
        // Dateline box only for consecutive quotes immediately after an H1 (or document start).
        var allowDatelineBox = options.EnableChapterDatelineBoxes;

        void FlushMeta()
        {
            if (pendingMeta.Count == 0)
                return;
            var lines = BuildDatelineLines(pendingMeta);
            pendingMeta.Clear();
            if (lines.Count == 0)
                return;

            // Reader-facing dateline panel → fundamental TextBox (style from export options).
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
            if (section is IMarkdownHeader { Level: 1 })
            {
                FlushMeta();
                AppendSection(blocks, section, options);
                allowDatelineBox = options.EnableChapterDatelineBoxes;
                continue;
            }

            if (allowDatelineBox && TryConsumeDatelineQuotes(section, pendingMeta))
                continue;

            allowDatelineBox = false;
            FlushMeta();
            AppendSection(blocks, section, options);
        }

        FlushMeta();
        return blocks;
    }

    /// <summary>
    /// Legacy <c>[!tag] value</c>, or plain public mirror once a dateline block has started
    /// (or the first plain line is a stardate / TK).
    /// </summary>
    static bool TryConsumeDatelineQuotes(IMarkdownSection section, List<(string Tag, string Value)> pendingMeta)
    {
        if (section is IMarkdownQuote quote)
        {
            var parsed = new List<(string Tag, string Value)>();
            foreach (var line in quote.Text)
            {
                if (!TryParseDatelineQuote(new MarkdownQuote(line), pendingMeta.Count > 0 || parsed.Count > 0, out var tag, out var value))
                    return false;
                parsed.Add((tag, value));
            }

            if (parsed.Count == 0)
                return false;

            pendingMeta.AddRange(parsed);
            return true;
        }

        if (!TryParseDatelineQuote(section, pendingMeta.Count > 0, out var sectionTag, out var sectionValue))
            return false;

        pendingMeta.Add((sectionTag, sectionValue));
        return true;
    }

    static bool TryParseDatelineQuote(
        IMarkdownSection section,
        bool blockAlreadyStarted,
        out string tag,
        out string value)
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
        if (text.Length == 0)
            return blockAlreadyStarted; // blank > spacer only inside an open block

        // Pedagogical labels are never fiction datelines (even mid-block).
        if (AdmonitionLabelRegex.IsMatch(text))
            return false;

        var m = CalloutRegex.Match(text);
        if (m.Success)
        {
            tag = m.Groups[1].Value;
            value = m.Groups[2].Value.Trim();
            return true;
        }

        if (!blockAlreadyStarted && !PublicDatelineValueRegex.IsMatch(text))
            return false;

        tag = "line";
        value = text;
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

    static void AppendSection(List<DocIBlock> blocks, IMarkdownSection section, MarkdownPagedExportOptions options)
    {
        switch (section)
        {
            case IMarkdownHeader header:
                var level = System.Math.Clamp(header.Level, 1, 4);
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
                    var language = string.IsNullOrWhiteSpace(code.Language) ? null : code.Language;
                    IReadOnlyList<CodeLine>? styled = null;
                    if (options.HighlightCode)
                        styled = CodeSyntaxHighlighter.Highlight(code.Code.Replace("\r\n", "\n").TrimEnd(), language);

                    if (options.UseTextbookChrome)
                    {
                        blocks.Add(new CodeBlock
                        {
                            Lines = lines,
                            StyledLines = styled,
                            Language = language,
                            ShowLineNumbers = options.ShowCodeLineNumbers,
                            LineNumberColor = DocumentColor.Parse("#6a737d"),
                            Background = CodeFill,
                            BorderStrokePt = 0.6f,
                            BorderColor = CodeBorder,
                            AccentBorderLeftPt = 3f,
                            AccentColor = CodeAccent,
                            FontSizePt = 9f,
                            LineHeight = 1.35f,
                            TextColor = BodyInk,
                        });
                    }
                    else
                    {
                        blocks.Add(new CodeBlock
                        {
                            Lines = lines,
                            StyledLines = styled,
                            Language = language,
                            ShowLineNumbers = options.ShowCodeLineNumbers,
                        });
                    }
                }
                break;

            case IMarkdownAlert alert:
                var alertText = string.Join(" ", alert.Text).Trim();
                if (!string.IsNullOrWhiteSpace(alertText))
                    blocks.Add(new DocParagraph { Text = alertText });
                break;

            case IMarkdownQuote quote:
                if (options.UseTextbookChrome && TryMapAdmonition(quote, out var admonition))
                {
                    blocks.Add(admonition);
                    break;
                }

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

    static bool TryMapAdmonition(IMarkdownQuote quote, out TextBoxBlock box)
    {
        box = null!;
        var parts = quote.Text.Select(t => t.Trim()).Where(t => t.Length > 0).ToArray();
        if (parts.Length == 0)
            return false;

        var match = AdmonitionLabelRegex.Match(parts[0]);
        if (!match.Success)
            return false;

        var label = CanonicalAdmonitionLabel(match.Groups["label"].Value);
        var body = match.Groups["body"].Value.Trim();
        if (parts.Length > 1)
        {
            var rest = string.Join(' ', parts.Skip(1));
            body = string.IsNullOrWhiteSpace(body) ? rest : body + " " + rest;
        }

        var (accent, fill) = AdmonitionColors(label);
        var line = string.IsNullOrWhiteSpace(body) ? label : $"{label} — {body}";
        box = new TextBoxBlock
        {
            Lines = [line],
            PaddingPt = 8f,
            BorderStrokePt = 0.5f,
            BorderColor = accent,
            Background = fill,
            AccentBorderLeftPt = 3f,
            AccentColor = accent,
            FontSizePt = 10f,
            LineHeight = 1.35f,
            LineGapPt = 1f,
            TextColor = BodyInk,
        };
        return true;
    }

    static string CanonicalAdmonitionLabel(string raw)
    {
        var t = raw.Trim();
        if (t.Equals("Warning", StringComparison.OrdinalIgnoreCase))
            return "Warning";
        if (t.Equals("Tip", StringComparison.OrdinalIgnoreCase))
            return "Tip";
        if (t.Equals("Fun Fact", StringComparison.OrdinalIgnoreCase))
            return "Fun Fact";
        if (t.Equals("Note", StringComparison.OrdinalIgnoreCase))
            return "Note";
        if (t.Equals("Best Practice", StringComparison.OrdinalIgnoreCase))
            return "Best Practice";
        if (t.Equals("Rigorous Detail", StringComparison.OrdinalIgnoreCase))
            return "Rigorous Detail";
        return t;
    }

    static (DocumentColor Accent, DocumentColor Fill) AdmonitionColors(string label) =>
        label switch
        {
            "Warning" => (WarningAccent, WarningFill),
            "Tip" or "Fun Fact" => (TipAccent, TipFill),
            _ => (NoteAccent, NoteFill),
        };

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
