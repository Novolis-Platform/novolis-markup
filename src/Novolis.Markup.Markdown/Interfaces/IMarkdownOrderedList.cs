// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

public interface IMarkdownOrderedList : IMarkdownSection
{
    IEnumerable<string> Items { get; }
}