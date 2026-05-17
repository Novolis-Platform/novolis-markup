// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

public interface IMarkdownParagraph : IMarkdownSection
{
    IEnumerable<IMarkdownParagraphItem> Items { get; }
}