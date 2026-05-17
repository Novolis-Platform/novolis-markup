// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

internal static class MarkdownParagraphItemExtensions
{
    public static string GetPrefix(this IMarkdownParagraphItem item)
    {
        return item.Type switch
        {
            MarkdownParagraphItemType.Bold => "**",
            MarkdownParagraphItemType.Italic => "*",
            MarkdownParagraphItemType.Strikethrough => "~~",
            MarkdownParagraphItemType.Underline => "__",
            MarkdownParagraphItemType.Text => "",
            MarkdownParagraphItemType.Link => "[",
            MarkdownParagraphItemType.LinkText => "(",
            MarkdownParagraphItemType.Code => "`",
            MarkdownParagraphItemType.Indent => "",
            MarkdownParagraphItemType.NewLine => "",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    public static string GetSuffix(this IMarkdownParagraphItem item)
    {
        return item.Type switch
        {
            MarkdownParagraphItemType.Bold => "**",
            MarkdownParagraphItemType.Italic => "*",
            MarkdownParagraphItemType.Strikethrough => "~~",
            MarkdownParagraphItemType.Underline => "__",
            MarkdownParagraphItemType.Text => "",
            MarkdownParagraphItemType.Link => "]",
            MarkdownParagraphItemType.LinkText => ")",
            MarkdownParagraphItemType.Code => "`",
            MarkdownParagraphItemType.Indent => "",
            MarkdownParagraphItemType.NewLine => "",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}