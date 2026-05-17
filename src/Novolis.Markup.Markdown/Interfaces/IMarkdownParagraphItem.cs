// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

public interface IMarkdownParagraphItem
{
    string Text { get; }
    
    MarkdownParagraphItemType Type { get; }
}