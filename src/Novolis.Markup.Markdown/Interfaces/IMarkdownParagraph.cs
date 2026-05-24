// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

/// <summary>Represents IMarkdownParagraph.</summary>
public interface IMarkdownParagraph : IMarkdownSection
{
    /// <summary>Items.</summary>
    IEnumerable<IMarkdownParagraphItem> Items { get; }
}
