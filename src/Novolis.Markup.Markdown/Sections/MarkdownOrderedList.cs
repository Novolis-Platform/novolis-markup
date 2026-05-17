// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

public class MarkdownOrderedList(IEnumerable<string> items, MarkdownOrderedListStyle style = MarkdownOrderedListStyle.Numbered) : IMarkdownOrderedList
{
    private readonly List<string> _items = items.ToList();
    
    public IEnumerable<string> Items => _items;

    public override string ToString() => style switch
    {
        MarkdownOrderedListStyle.Numbered => JoinNumberedList(),
        MarkdownOrderedListStyle.Alpha => JoinAlphaList(),
        _ => $"ERROR! Unknown ordered list style: {style}. Supported styles are: {string.Join(", ", Enum.GetNames(typeof(MarkdownOrderedListStyle)))}"
    };

    private string JoinNumberedList() => string.Join(IMarkdownSection.NewLine, _items.Select((x, i) => $"{i + 1}. {x}")) + IMarkdownSection.NewLine;
    
    private string JoinAlphaList() => string.Join(IMarkdownSection.NewLine, _items.Select((x, i) => $"{(char)('a' + i)}. {x}")) + IMarkdownSection.NewLine;
}