// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

public class MarkdownTable<T>(IEnumerable<string> headers, IEnumerable<IEnumerable<string>> rows) : IMarkdownTable
{
    public MarkdownTable(IEnumerable<T> items) : this(items.ToMarkdownTablePrecursors())
    {
    }

    public MarkdownTable((IEnumerable<string>, IEnumerable<IEnumerable<string>>) markdownTablePrecursors) : this(markdownTablePrecursors.Item1, markdownTablePrecursors.Item2)
    {
    }

    public IEnumerable<string> Headers { get; } = headers;

    public IEnumerable<IEnumerable<string>> Rows { get; } = rows;
    
    public override string ToString() => this.ToMarkdownTableString();
}