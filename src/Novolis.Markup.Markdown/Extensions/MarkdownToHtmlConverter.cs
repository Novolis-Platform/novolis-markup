using Novolis.Markup.Html;

// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

/// <summary>Converts fluent Markdown documents to HTML via <see cref="HtmlMarkup"/>.</summary>
public static class MarkdownToHtmlConverter
{
    /// <summary>Converts a Markdown document to an HTML fragment string.</summary>
    public static string Convert(
        IMarkdownDocument document,
        MarkdownHtmlSectionRenderer? sectionRenderer = null,
        MarkdownHtmlActionSink? actions = null) =>
        ConvertNodes(document, sectionRenderer, actions).ToString();

    /// <summary>Converts a Markdown document to an HTML fragment.</summary>
    public static HtmlFragment ConvertNodes(
        IMarkdownDocument document,
        MarkdownHtmlSectionRenderer? sectionRenderer = null,
        MarkdownHtmlActionSink? actions = null)
    {
        ArgumentNullException.ThrowIfNull(document);
        var fragment = HtmlMarkup.Fragment();
        foreach (var section in document)
        {
            fragment.Child(ConvertSection(section, sectionRenderer, actions));
        }

        return fragment;
    }

    private static IHtmlNode? ConvertSection(
        IMarkdownSection section,
        MarkdownHtmlSectionRenderer? sectionRenderer,
        MarkdownHtmlActionSink? actions) =>
        sectionRenderer?.Invoke(section) ?? ConvertBuiltInSection(section, actions);

    private static IHtmlNode? ConvertBuiltInSection(IMarkdownSection section, MarkdownHtmlActionSink? actions) => section switch
    {
        IMarkdownCodeBlock code => actions is null
            ? HtmlMarkup.PreCode(code.Code, string.IsNullOrWhiteSpace(code.Language) ? null : code.Language)
            : MarkdownHtmlChrome.CodeBlock(
                code.Code,
                string.IsNullOrWhiteSpace(code.Language) ? null : code.Language,
                actions),
        IMarkdownAlert alert => ConvertAlert(alert),
        IMarkdownHeader header => ConvertHeader(header, actions),
        IMarkdownParagraph paragraph => ConvertParagraph(paragraph, actions),
        IMarkdownQuote quote => ConvertQuote(quote, actions),
        IMarkdownTable table => ConvertTable(table, actions),
        IMarkdownUnorderedList list => ConvertList(HtmlMarkup.Ul(), list.Items, actions),
        IMarkdownOrderedList list => ConvertList(HtmlMarkup.Ol(), list.Items, actions),
        IMarkdownHorizontalRule => HtmlMarkup.Hr(),
        _ => null,
    };

    private static HtmlElement ConvertHeader(IMarkdownHeader header, MarkdownHtmlActionSink? actions) =>
        HtmlMarkup.H((int)header.Level, h => AppendInlines(h, MarkdownDocument.ParseInlineParagraph(header.Text), actions));

    private static HtmlElement ConvertQuote(IMarkdownQuote quote, MarkdownHtmlActionSink? actions)
    {
        return HtmlMarkup.Blockquote(blockquote =>
        {
            var first = true;
            foreach (var line in quote.Text)
            {
                if (!first)
                    blockquote.Br();
                first = false;
                AppendInlines(blockquote, MarkdownDocument.ParseInlineParagraph(line), actions);
            }
        });
    }

    private static HtmlElement ConvertTable(IMarkdownTable table, MarkdownHtmlActionSink? actions)
    {
        return HtmlMarkup.Table(markup =>
        {
            markup.Thead(thead => thead.Tr(tr =>
            {
                foreach (var header in table.Headers)
                    tr.Th(th => AppendInlines(th, MarkdownDocument.ParseInlineParagraph(header), actions));
            }));
            markup.Tbody(tbody =>
            {
                foreach (var row in table.Rows)
                {
                    tbody.Tr(tr =>
                    {
                        foreach (var cell in row)
                            tr.Td(td => AppendInlines(td, MarkdownDocument.ParseInlineParagraph(cell), actions));
                    });
                }
            });
        });
    }

    private static HtmlElement ConvertAlert(IMarkdownAlert alert)
    {
        var level = alert.Level.ToString();
        var text = string.Concat(alert.Text);
        return HtmlMarkup.Alert(level, text);
    }

    private static HtmlElement ConvertList(HtmlElement list, IEnumerable<string> items, MarkdownHtmlActionSink? actions)
    {
        var decodedItems = items
            .Select(item =>
            {
                MarkdownDocument.DecodeNestDepth(item, out var body);
                return body;
            })
            .ToArray();

        if (decodedItems.Any(HasTaskMarker))
            list.Class("contains-task-list");

        foreach (var body in decodedItems)
        {
            list.Li(li =>
            {
                if (TryReadTaskMarker(body, out var isChecked, out var taskBody))
                {
                    li.Class("task-list-item");
                    var checkbox = li.Input(input => input
                        .Type("checkbox")
                        .Class("task-list-item-checkbox")
                        .Attr("disabled")
                        .Attr("aria-label", isChecked ? "Completed" : "Not completed"));
                    if (isChecked)
                        checkbox.Attr("checked");
                    li.Text(" ");
                    AppendInlines(li, MarkdownDocument.ParseInlineParagraph(taskBody), actions);
                    return;
                }

                AppendInlines(li, MarkdownDocument.ParseInlineParagraph(body), actions);
            });
        }

        return list;
    }

    private static bool HasTaskMarker(string body) =>
        TryReadTaskMarker(body, out _, out _);

    private static bool TryReadTaskMarker(string body, out bool isChecked, out string taskBody)
    {
        isChecked = false;
        taskBody = body;

        if (body.StartsWith("[]", StringComparison.Ordinal))
        {
            taskBody = body[2..].TrimStart();
            return true;
        }

        if (body.Length < 3 || body[0] != '[' || body[2] != ']')
            return false;

        if (body[1] is not (' ' or 'x' or 'X'))
            return false;

        isChecked = body[1] is 'x' or 'X';
        taskBody = body[3..].TrimStart();
        return true;
    }

    private static HtmlElement ConvertParagraph(IMarkdownParagraph paragraph, MarkdownHtmlActionSink? actions)
    {
        var p = HtmlMarkup.P();
        AppendInlines(p, paragraph, actions);
        return p;
    }

    private static void AppendInlines(HtmlElement host, IMarkdownParagraph paragraph, MarkdownHtmlActionSink? actions)
    {
        string? pendingLinkText = null;
        string? pendingImageAlt = null;

        foreach (var inline in paragraph.Items)
        {
            switch (inline.Type)
            {
                case MarkdownParagraphItemType.Text:
                case MarkdownParagraphItemType.Indent:
                case MarkdownParagraphItemType.NewLine:
                    FlushPending(host, ref pendingLinkText, ref pendingImageAlt);
                    host.Text(inline.Text);
                    break;
                case MarkdownParagraphItemType.Bold:
                    FlushPending(host, ref pendingLinkText, ref pendingImageAlt);
                    host.Strong(inline.Text);
                    break;
                case MarkdownParagraphItemType.Italic:
                    FlushPending(host, ref pendingLinkText, ref pendingImageAlt);
                    host.Em(inline.Text);
                    break;
                case MarkdownParagraphItemType.Strikethrough:
                    FlushPending(host, ref pendingLinkText, ref pendingImageAlt);
                    host.Child(HtmlMarkup.Del(inline.Text));
                    break;
                case MarkdownParagraphItemType.Underline:
                    FlushPending(host, ref pendingLinkText, ref pendingImageAlt);
                    host.Child(HtmlMarkup.U(inline.Text));
                    break;
                case MarkdownParagraphItemType.Code:
                    FlushPending(host, ref pendingLinkText, ref pendingImageAlt);
                    host.Code(inline.Text);
                    break;
                case MarkdownParagraphItemType.LinkText:
                    FlushPending(host, ref pendingLinkText, ref pendingImageAlt);
                    pendingLinkText = inline.Text;
                    break;
                case MarkdownParagraphItemType.Link:
                    host.Child(HtmlMarkup.A(inline.Text, pendingLinkText ?? string.Empty));
                    pendingLinkText = null;
                    break;
                case MarkdownParagraphItemType.ImageAlt:
                    FlushPending(host, ref pendingLinkText, ref pendingImageAlt);
                    pendingImageAlt = inline.Text;
                    break;
                case MarkdownParagraphItemType.Image:
                    AppendImage(host, pendingImageAlt ?? string.Empty, inline.Text, actions);
                    pendingImageAlt = null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(paragraph), inline.Type, "Unknown paragraph item type.");
            }
        }

        FlushPending(host, ref pendingLinkText, ref pendingImageAlt);
    }

    private static void AppendImage(
        HtmlElement host,
        string alt,
        string url,
        MarkdownHtmlActionSink? actions)
    {
        var embedded = MarkdownLocalImage.TryEmbed(url, actions?.SourceDirectory);
        if (embedded is not null && actions is not null)
        {
            host.Child(MarkdownHtmlChrome.Media(embedded, string.IsNullOrWhiteSpace(alt) ? "Image" : alt, actions, "markdown-image"));
            return;
        }

        if (embedded is not null)
        {
            host.Img(embedded, alt);
            return;
        }

        host.Child(HtmlMarkup.A(url, string.IsNullOrWhiteSpace(alt) ? url : alt));
    }

    private static void FlushPending(HtmlElement paragraph, ref string? pendingLinkText, ref string? pendingImageAlt)
    {
        if (pendingLinkText is not null)
        {
            paragraph.Text(pendingLinkText);
            pendingLinkText = null;
        }

        if (pendingImageAlt is not null)
        {
            paragraph.Text(pendingImageAlt);
            pendingImageAlt = null;
        }
    }
}
