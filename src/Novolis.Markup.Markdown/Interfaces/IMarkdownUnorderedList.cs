// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

/// <summary>
/// Represents an interface for a markdown unordered list.
/// </summary>
public interface IMarkdownUnorderedList : IMarkdownSection
{
    /// <summary>
    /// Gets the collection of items.
    /// </summary>
    /// <remarks>
    /// This property returns an enumerable collection of strings representing the items.
    /// </remarks>
    /// <returns>
    /// The collection of items.
    /// </returns>
    IEnumerable<string> Items { get; }
}