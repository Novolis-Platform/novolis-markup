// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

public interface IMarkdownQuote : IMarkdownSection
{
    IEnumerable<string> Text { get; }
}