using Novolis.Markup.Html;

// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

/// <summary>Converts fluent Markdown documents to HTML via <see cref="HtmlMarkup"/>.</summary>
public static class MarkdownToHtmlConverter
{
    /// <summary>Converts a Markdown document to an HTML fragment string.</summary>
    public static string Convert(IMarkdownDocument document, MarkdownHtmlSectionRenderer? sectionRenderer = null) =>
        ConvertNodes(document, sectionRenderer).ToString();

    /// <summary>Converts a Markdown document to an HTML fragment.</summary>
    public static HtmlFragment ConvertNodes(IMarkdownDocument document, MarkdownHtmlSectionRenderer? sectionRenderer = null)
    {
        ArgumentNullException.ThrowIfNull(document);
        var fragment = HtmlMarkup.Fragment();
        foreach (var section in document)
        {
            fragment.Child(ConvertSection(section, sectionRenderer));
        }

        return fragment;
    }

    private static IHtmlNode? ConvertSection(IMarkdownSection section, MarkdownHtmlSectionRenderer? sectionRenderer) =>
        sectionRenderer?.Invoke(section) ?? ConvertBuiltInSection(section);

    private static IHtmlNode? ConvertBuiltInSection(IMarkdownSection section) => section switch
    {
        IMarkdownCodeBlock code => HtmlMarkup.PreCode(code.Code, string.IsNullOrWhiteSpace(code.Language) ? null : code.Language),
        IMarkdownAlert alert => ConvertAlert(alert),
        IMarkdownHeader header => ConvertHeader(header),
        IMarkdownParagraph paragraph => ConvertParagraph(paragraph),
        IMarkdownQuote quote => ConvertQuote(quote),
        IMarkdownTable table => ConvertTable(table),
        IMarkdownUnorderedList list => ConvertList(HtmlMarkup.Ul(), list.Items),
        IMarkdownOrderedList list => ConvertList(HtmlMarkup.Ol(), list.Items),
        IMarkdownHorizontalRule => HtmlMarkup.Hr(),
        _ => null,
    };

    private static HtmlElement ConvertHeader(IMarkdownHeader header) =>
        HtmlMarkup.H((int)header.Level, h => AppendInlines(h, MarkdownDocument.ParseInlineParagraph(header.Text)));

    private static HtmlElement ConvertQuote(IMarkdownQuote quote)
    {
        return HtmlMarkup.Blockquote(blockquote =>
        {
            var first = true;
            foreach (var line in quote.Text)
            {
                if (!first)
                    blockquote.Br();
                first = false;
                AppendInlines(blockquote, MarkdownDocument.ParseInlineParagraph(line));
            }
        });
    }

    private static HtmlElement ConvertTable(IMarkdownTable table)
    {
        return HtmlMarkup.Table(markup =>
        {
            markup.Thead(thead => thead.Tr(tr =>
            {
                foreach (var header in table.Headers)
                    tr.Th(th => AppendInlines(th, MarkdownDocument.ParseInlineParagraph(header)));
            }));
            markup.Tbody(tbody =>
            {
                foreach (var row in table.Rows)
                {
                    tbody.Tr(tr =>
                    {
                        foreach (var cell in row)
                            tr.Td(td => AppendInlines(td, MarkdownDocument.ParseInlineParagraph(cell)));
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

    private static HtmlElement ConvertList(HtmlElement list, IEnumerable<string> items)
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
                    AppendInlines(li, MarkdownDocument.ParseInlineParagraph(taskBody));
                    return;
                }

                AppendInlines(li, MarkdownDocument.ParseInlineParagraph(body));
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

    private static HtmlElement ConvertParagraph(IMarkdownParagraph paragraph)
    {
        var p = HtmlMarkup.P();
        AppendInlines(p, paragraph);
        return p;
    }

    private static void AppendInlines(HtmlElement host, IMarkdownParagraph paragraph)
    {
        string? pendingLinkText = null;

        foreach (var inline in paragraph.Items)
        {
            switch (inline.Type)
            {
                case MarkdownParagraphItemType.Text:
                case MarkdownParagraphItemType.Indent:
                case MarkdownParagraphItemType.NewLine:
                    FlushPendingLink(host, ref pendingLinkText);
                    host.Text(inline.Text);
                    break;
                case MarkdownParagraphItemType.Bold:
                    FlushPendingLink(host, ref pendingLinkText);
                    host.Strong(inline.Text);
                    break;
                case MarkdownParagraphItemType.Italic:
                    FlushPendingLink(host, ref pendingLinkText);
                    host.Em(inline.Text);
                    break;
                case MarkdownParagraphItemType.Strikethrough:
                    FlushPendingLink(host, ref pendingLinkText);
                    host.Child(HtmlMarkup.Del(inline.Text));
                    break;
                case MarkdownParagraphItemType.Underline:
                    FlushPendingLink(host, ref pendingLinkText);
                    host.Child(HtmlMarkup.U(inline.Text));
                    break;
                case MarkdownParagraphItemType.Code:
                    FlushPendingLink(host, ref pendingLinkText);
                    host.Code(inline.Text);
                    break;
                case MarkdownParagraphItemType.LinkText:
                    FlushPendingLink(host, ref pendingLinkText);
                    pendingLinkText = inline.Text;
                    break;
                case MarkdownParagraphItemType.Link:
                    host.Child(HtmlMarkup.A(inline.Text, pendingLinkText ?? string.Empty));
                    pendingLinkText = null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(paragraph), inline.Type, "Unknown paragraph item type.");
            }
        }

        FlushPendingLink(host, ref pendingLinkText);
    }

    private static void FlushPendingLink(HtmlElement paragraph, ref string? pendingLinkText)
    {
        if (pendingLinkText is null)
        {
            return;
        }

        paragraph.Text(pendingLinkText);
        pendingLinkText = null;
    }
}
