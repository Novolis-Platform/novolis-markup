using System.Text;

// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

/// <summary>Represents MarkdownToHtmlConverter.</summary>
public static class MarkdownToHtmlConverter
{
    /// <summary>Convert operation.</summary>
    public static string Convert(IMarkdownDocument document)
    {
        var builder = new StringBuilder();

        foreach (var section in document)
        {
            builder.Append(section switch
            {
                IMarkdownCodeBlock markdownCodeBlock => Convert(markdownCodeBlock),
                IMarkdownAlert markdownAlert => Convert(markdownAlert),
                IMarkdownHeader markdownHeader => $"<h{(int)markdownHeader.Level}>{markdownHeader.Text}</h{(int)markdownHeader.Level}>",
                IMarkdownParagraph paragraph => Convert(paragraph),
                IMarkdownQuote markdownQuote => Convert(markdownQuote),
                IMarkdownTable markdownTable => Convert(markdownTable),
                IMarkdownUnorderedList markdownUnorderedList => Convert(markdownUnorderedList),
                IMarkdownHorizontalRule horizontalRule => "<hr />",
                IMarkdownOrderedList markdownOrderedList => Convert(markdownOrderedList),
                _ => string.Empty,
            });
        }

        return builder.ToString();
    }

    private static string? Convert(IMarkdownOrderedList markdownUnorderedList)
    {
        var builder = new StringBuilder();
        builder.Append("<ol>");
        foreach (var item in markdownUnorderedList.Items)
        {
            builder.Append("<li>");
            builder.Append(item);
            builder.Append("</li>");
        }
        builder.Append("</ol>");
        return builder.ToString();
    }

    private static string? Convert(IMarkdownUnorderedList markdownUnorderedList)
    {
        var builder = new StringBuilder();
        builder.Append("<ul>");
        foreach (var item in markdownUnorderedList.Items)
        {
            builder.Append("<li>");
            builder.Append(item);
            builder.Append("</li>");
        }
        builder.Append("</ul>");
        return builder.ToString();
    }

    private static string? Convert(IMarkdownTable markdownTable)
    {
        var builder = new StringBuilder();
        builder.Append("<table>");
        builder.Append("<thead>");
        builder.Append("<tr>");
        foreach (var header in markdownTable.Headers)
        {
            builder.Append($"<th>{header}</th>");
        }
        builder.Append("</tr>");
        builder.Append("</thead>");
        builder.Append("<tbody>");
        foreach (var row in markdownTable.Rows)
        {
            builder.Append("<tr>");
            foreach (var cell in row)
            {
                builder.Append($"<td>{cell}</td>");
            }
            builder.Append("</tr>");
        }
        builder.Append("</tbody>");
        builder.Append("</table>");
        return builder.ToString();
    }

    private static string? Convert(IMarkdownQuote markdownQuote)
    {
        var builder = new StringBuilder();
        foreach (var inline in markdownQuote.Text) builder.Append(inline);
        return $"<blockquote>{builder}</blockquote>";
    }

    private static string? Convert(IMarkdownAlert markdownAlert)
    {
        var builder = new StringBuilder();
        foreach (var inline in markdownAlert.Text) builder.Append(inline);
        return $"""
                <div class="alert alert-{markdownAlert.Level}">
                    <p><strong>{markdownAlert.Level}</strong></p>
                    <p>{builder}</p>
                </div>
                """;
    }

    private static string? Convert(IMarkdownCodeBlock markdownCodeBlock) => $"<pre><code>{markdownCodeBlock.Code}</code></pre>";

    private static string Convert(IMarkdownParagraph paragraph)
    {
        var builder = new StringBuilder();

        foreach (var inline in paragraph.Items)
        {
            builder.Append(inline.Type switch
            {
                MarkdownParagraphItemType.Text => inline.Text,
                MarkdownParagraphItemType.Bold => $"<strong>{inline.Text}</strong>",
                MarkdownParagraphItemType.Italic => $"<em>{inline.Text}</em>",
                MarkdownParagraphItemType.Strikethrough => $"<del>{inline.Text}</del>",
                MarkdownParagraphItemType.Link => $"<a href=\"{inline.Text}\">",
                MarkdownParagraphItemType.LinkText => $"{inline.Text}</a>",
                MarkdownParagraphItemType.Code => $"<code>{inline.Text}</code>",
                MarkdownParagraphItemType.Indent => inline.Text,
                MarkdownParagraphItemType.NewLine => inline.Text,
                MarkdownParagraphItemType.Underline => $"<u>{inline.Text}</u>",
                _ => throw new ArgumentOutOfRangeException()
            });
        }

        return $"<p>{builder}</p>";
    }
}
