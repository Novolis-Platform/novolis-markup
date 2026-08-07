using Novolis.Markup.Html;

// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

/// <summary>Converts fluent Markdown documents to HTML via <see cref="HtmlMarkup"/>.</summary>
public static class MarkdownToHtmlConverter
{
    /// <summary>Converts a Markdown document to an HTML fragment string.</summary>
    public static string Convert(IMarkdownDocument document) => ConvertNodes(document).ToString();

    /// <summary>Converts a Markdown document to an HTML fragment.</summary>
    public static HtmlFragment ConvertNodes(IMarkdownDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        var fragment = HtmlMarkup.Fragment();
        foreach (var section in document)
        {
            fragment.Child(ConvertSection(section));
        }

        return fragment;
    }

    private static IHtmlNode? ConvertSection(IMarkdownSection section) => section switch
    {
        IMarkdownCodeBlock code => HtmlMarkup.PreCode(code.Code, string.IsNullOrWhiteSpace(code.Language) ? null : code.Language),
        IMarkdownAlert alert => ConvertAlert(alert),
        IMarkdownHeader header => HtmlMarkup.H((int)header.Level, header.Text),
        IMarkdownParagraph paragraph => ConvertParagraph(paragraph),
        IMarkdownQuote quote => HtmlMarkup.Blockquote(quote.Text),
        IMarkdownTable table => HtmlMarkup.Table(table.Headers, table.Rows),
        IMarkdownUnorderedList list => HtmlMarkup.Ul(list.Items),
        IMarkdownOrderedList list => HtmlMarkup.Ol(list.Items),
        IMarkdownHorizontalRule => HtmlMarkup.Hr(),
        _ => null,
    };

    private static HtmlElement ConvertAlert(IMarkdownAlert alert)
    {
        var level = alert.Level.ToString();
        var text = string.Concat(alert.Text);
        return HtmlMarkup.Alert(level, text);
    }

    private static HtmlElement ConvertParagraph(IMarkdownParagraph paragraph)
    {
        var p = HtmlMarkup.P();
        string? pendingLinkText = null;

        foreach (var inline in paragraph.Items)
        {
            switch (inline.Type)
            {
                case MarkdownParagraphItemType.Text:
                case MarkdownParagraphItemType.Indent:
                case MarkdownParagraphItemType.NewLine:
                    FlushPendingLink(p, ref pendingLinkText);
                    p.Text(inline.Text);
                    break;
                case MarkdownParagraphItemType.Bold:
                    FlushPendingLink(p, ref pendingLinkText);
                    p.Strong(inline.Text);
                    break;
                case MarkdownParagraphItemType.Italic:
                    FlushPendingLink(p, ref pendingLinkText);
                    p.Em(inline.Text);
                    break;
                case MarkdownParagraphItemType.Strikethrough:
                    FlushPendingLink(p, ref pendingLinkText);
                    p.Child(HtmlMarkup.Del(inline.Text));
                    break;
                case MarkdownParagraphItemType.Underline:
                    FlushPendingLink(p, ref pendingLinkText);
                    p.Child(HtmlMarkup.U(inline.Text));
                    break;
                case MarkdownParagraphItemType.Code:
                    FlushPendingLink(p, ref pendingLinkText);
                    p.Code(inline.Text);
                    break;
                case MarkdownParagraphItemType.LinkText:
                    FlushPendingLink(p, ref pendingLinkText);
                    pendingLinkText = inline.Text;
                    break;
                case MarkdownParagraphItemType.Link:
                    p.Child(HtmlMarkup.A(inline.Text, pendingLinkText ?? string.Empty));
                    pendingLinkText = null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(paragraph), inline.Type, "Unknown paragraph item type.");
            }
        }

        FlushPendingLink(p, ref pendingLinkText);
        return p;
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
