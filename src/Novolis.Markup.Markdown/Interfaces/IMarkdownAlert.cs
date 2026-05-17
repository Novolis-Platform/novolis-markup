// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

public interface IMarkdownAlert : IMarkdownQuote
{
    MarkdownAlertLevel Level { get; }
}