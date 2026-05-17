// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

public class MarkdownUnorderedList(IEnumerable<string> items) : IMarkdownUnorderedList
{
    private readonly List<string> _items = items.ToList();

    public IEnumerable<string> Items => _items;

    public override string ToString() => string.Join(IMarkdownSection.NewLine, _items.Select(x => $"- {x}")) + IMarkdownSection.NewLine;
}