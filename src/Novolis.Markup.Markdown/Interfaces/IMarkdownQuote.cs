// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

/// <summary>Represents IMarkdownQuote.</summary>
public interface IMarkdownQuote : IMarkdownSection
{
    /// <summary>Text.</summary>
    IEnumerable<string> Text { get; }
}
