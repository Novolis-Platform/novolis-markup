// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

/// <summary>Represents IMarkdownOrderedList.</summary>
public interface IMarkdownOrderedList : IMarkdownSection
{
    /// <summary>Items.</summary>
    IEnumerable<string> Items { get; }
}
