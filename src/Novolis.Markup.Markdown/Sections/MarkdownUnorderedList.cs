// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

/// <summary>Represents MarkdownUnorderedList.</summary>
public class MarkdownUnorderedList(IEnumerable<string> items) : IMarkdownUnorderedList
{
    private readonly List<string> _items = items.ToList();
/// <summary>ToString operation.</summary>

    public IEnumerable<string> Items => _items;

    /// <summary>ToString operation.</summary>
    public override string ToString() =>
        string.Join(IMarkdownSection.NewLine, _items.Select(static x =>
        {
            var depth = MarkdownDocument.DecodeNestDepth(x, out var body);
            return new string(' ', depth * 2) + "- " + body;
        })) + IMarkdownSection.NewLine;
}
