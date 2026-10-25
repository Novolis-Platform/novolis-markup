using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Novolis.Documents;
using Novolis.Markup.Markdown;
using DocHeading = Novolis.Documents.HeadingBlock;
using DocIBlock = Novolis.Documents.IBlock;
using DocParagraph = Novolis.Documents.ParagraphBlock;

namespace Novolis.Markup.Markdown.Documents;

/// <summary>Maps <see cref="IMarkdownDocument"/> into a semantic <see cref="Document"/>.</summary>
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

    /// <summary>Maps a fluent Markdown document to a semantic section model.</summary>
    public static Document FromDocument(IMarkdownDocument document, MarkdownPagedExportOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(document);
        options ??= new MarkdownPagedExportOptions();
        var frontMatter = document.FrontMatter;
        var title = FirstNonEmpty(options.Title, frontMatter.Title) ?? InferTitle(document) ?? "Document";
        var body = MapBody(document, options);
        var semanticSections = body.SelectMany(ToSemanticSections).ToArray();

        var builder = Document.Create(title)
            .Meta(meta =>
            {
                meta.Subtitle(FirstNonEmpty(options.Subtitle, frontMatter.Get("Subtitle")))
                    .Series(FirstNonEmpty(options.Series, frontMatter.Get("Series")))
                    .Author(FirstNonEmpty(options.Author, frontMatter.Get("Author")))
                    .Rights(FirstNonEmpty(options.Rights, frontMatter.Get("Rights")))
                    .Contributors(frontMatter.Get("Contributors"))
                    .Publisher(frontMatter.Get("Publisher"))
                    .Subject(frontMatter.Get("Subject"))
                    .Description(frontMatter.Get("Description"))
                    .Identifier(frontMatter.Get("Identifier"))
                    .Language(frontMatter.Get("Language"))
                    .Version(frontMatter.Get("Version"));
                if (frontMatter.Tags.Count > 0)
                    meta.Keywords(frontMatter.Tags.ToArray());
                if (DateOnly.TryParse(frontMatter.Get("Date"), CultureInfo.InvariantCulture, out var date))
                    meta.Date(date);
            })
            .Page(page =>
            {
                page.TrimSize(options.Trim).Margins(options.Margin);
                if (!string.IsNullOrWhiteSpace(options.HeaderTemplate))
                {
                    page.Header(header => header
                        .Template(options.HeaderTemplate)
                        .IncludeBody()
                        .UseChapterTitle(options.UseChapterTitleHeader));
                }

                if (!string.IsNullOrWhiteSpace(options.FooterTemplate))
                {
                    page.Footer(footer => footer
                        .Template(options.FooterTemplate)
                        .IncludeBody()
                        .IncludeFirstPage(options.FooterOnFirstPage)
                        .IncludeToc(options.FooterOnToc)
                        .IncludeLastPage(options.FooterOnLastPage));
                }
            })
            .Typography(options.Typography)
            .Toc(options.IncludeToc);

        foreach (var section in semanticSections)
            builder.Add(section);
        builder.CompatibilityBody(body);

        builder.Body(bodyBuilder =>
        {
            if (options.IncludeCover || options.First is not null)
            {
                bodyBuilder.First(first =>
                {
                    if (options.First is { } configured)
                    {
                        first.Title(configured.Title)
                            .Subtitle(configured.Subtitle)
                            .Series(configured.Series)
                            .Author(configured.Author)
                            .Rights(configured.Rights)
                            .Lines(configured.Lines.ToArray())
                            .Blocks(content => content.AddRange(configured.Blocks));
                    }
                });
            }

            if (options.Last is { } last)
            {
                bodyBuilder.Last(closing => closing
                    .Title(last.Title)
                    .Lines(last.Lines.ToArray())
                    .Blocks(content => content.AddRange(last.Blocks)));
            }
        });

        if (options.Watermark is { } watermark)
            builder.Watermark(mark => mark
                .Text(watermark.Text)
                .Color(watermark.Color)
                .Opacity(watermark.Opacity)
                .Rotation(watermark.RotationDegrees)
                .On(watermark.Pages));

        return builder.Build();
    }

    static IEnumerable<ISection> ToSemanticSections(IBlock block)
    {
        switch (block)
        {
            case DocHeading heading:
                yield return new Section(
                    [new Heading(heading.Text, System.Math.Clamp(heading.Level, 1, 4))],
                    new SectionLayout { KeepWithNext = true });
                yield break;
            case DocParagraph paragraph:
                yield return new Section([new Text(paragraph.Text)]);
                yield break;
            case TableBlock table:
                yield return ToSemanticTable(table);
                yield break;
            case ColumnsBlock columns:
                yield return ToSemanticColumns(columns);
                yield break;
            case ImageBlock image when TryToSemanticImage(image, out var semanticImage):
                yield return new Section([semanticImage]);
                yield break;
            case CodeBlock code:
                yield return new Section(
                    [new Code(
                        string.Join(
                            Environment.NewLine,
                            code.ResolveLines().Select(static line =>
                                string.Concat(line.Spans.Select(static span => span.Text)))) )]);
                yield break;
            case TextBoxBlock box:
                yield return new Box(
                    box.Lines.Select(static line => (IContent)new Text(line)))
                {
                    Background = box.Background,
                    BorderColor = box.BorderColor,
                };
                yield break;
            case SceneBreakBlock scene:
                yield return new Section([new Rule()]);
                yield break;
            case LineBreakBlock:
                yield return new Section([new Spacer(8f)]);
                yield break;
            case PageBreakBlock:
                yield return new PageBreakSection();
                yield break;
            case BlankPageBlock:
                yield return new PageBreakSection();
                yield return new PageBreakSection();
                yield break;
            case CoverBlock or TocBlock:
                yield break;
        }
    }

    static IContent? ToSemanticContent(IBlock block) =>
        block switch
        {
            DocHeading heading => new Heading(heading.Text, System.Math.Clamp(heading.Level, 1, 4)),
            DocParagraph paragraph => new Text(paragraph.Text),
            TableBlock table => ToSemanticTable(table),
            ColumnsBlock columns => ToSemanticColumns(columns),
            ImageBlock image when TryToSemanticImage(image, out var semanticImage) => semanticImage,
            CodeBlock code => new Code(
                string.Join(
                    Environment.NewLine,
                    code.ResolveLines().Select(static line =>
                        string.Concat(line.Spans.Select(static span => span.Text))))),
            TextBoxBlock box => new Box(
                box.Lines.Select(static line => (IContent)new Text(line)))
            {
                Background = box.Background,
                BorderColor = box.BorderColor,
            },
            SceneBreakBlock => new Rule(),
            _ => null,
        };

    static ColumnsSection ToSemanticColumns(ColumnsBlock columns)
    {
        var result = new List<Column>();
        for (var index = 0; index < columns.Columns.Count; index++)
        {
            var contents = columns.Columns[index]
                .Select(ToSemanticContent)
                .Where(static content => content is not null)
                .Cast<IContent>()
                .ToArray();
            var width = columns.Fractions is { Count: > 0 } fractions
                && index < fractions.Count
                ? new ColumnWidth.Weight(System.Math.Max(0.01f, fractions[index]))
                : new ColumnWidth.Weight(1f);
            result.Add(new Column(contents, width));
        }

        return new ColumnsSection(result, columns.GapPt);
    }

    static TableSection ToSemanticTable(TableBlock table)
    {
        var count = table.Headers.Count;
        count = System.Math.Max(count, table.Rows.Select(static row => row.Count).DefaultIfEmpty().Max());
        var columns = Enumerable.Range(0, count)
            .Select(index => new TableColumn(
                new ColumnWidth.Weight(
                    table.ColumnWidths is { Count: > 0 } widths && index < widths.Count
                        ? System.Math.Max(0.01f, widths[index])
                        : 1f),
                table.ColumnAlignments is { Count: > 0 } alignments && index < alignments.Count
                    ? ToAlignment(alignments[index])
                    : HorizontalAlignment.Left))
            .ToArray();
        var rows = table.Rows.Select(row => new TableRow(
            Enumerable.Range(0, count)
                .Select(index => new TableCell(index < row.Count ? row[index] : string.Empty))))
            .ToArray();
        var header = table.Headers.Count == 0
            ? null
            : new TableRow(
                Enumerable.Range(0, count)
                    .Select(index => new TableCell(
                        index < table.Headers.Count ? table.Headers[index] : string.Empty)));
        return new TableSection(
            columns,
            rows,
            new TableOptions
            {
                Header = header,
                ShowHeader = table.ShowHeader && header is not null,
                RepeatHeader = table.RepeatHeaderOnPageBreak,
                HeaderBackground = table.HeaderBackground,
                RuleStyle = table.RuleStyle,
            });
    }

    static bool TryToSemanticImage(ImageBlock image, out Image semanticImage)
    {
        if (image.Data is { Length: > 0 } data)
        {
            semanticImage = new Image(data, image.WidthPt, image.HeightPt);
            return true;
        }

        if (!string.IsNullOrWhiteSpace(image.Path))
        {
            semanticImage = new Image(image.Path, image.WidthPt, image.HeightPt);
            return true;
        }

        semanticImage = null!;
        return false;
    }

    static HorizontalAlignment ToAlignment(CellAlign alignment) => alignment switch
    {
        CellAlign.Center => HorizontalAlignment.Center,
        CellAlign.Right => HorizontalAlignment.Right,
        _ => HorizontalAlignment.Left,
    };

    /// <summary>
    /// Maps raw Markdown via <see cref="MarkdownDocument.Parse"/> (Novolis Markdown, not Markdig).
    /// Prefer <see cref="FromDocument"/> for fluent / faithful documents.
    /// </summary>
    public static Document FromMarkdown(string markdown, MarkdownPagedExportOptions? options = null) =>
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

    static string? FirstNonEmpty(string? preferred, string? fallback) =>
        !string.IsNullOrWhiteSpace(preferred) ? preferred : fallback;

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
