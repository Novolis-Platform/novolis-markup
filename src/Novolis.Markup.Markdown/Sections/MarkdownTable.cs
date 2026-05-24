// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

/// <summary>A Markdown table section with headers and rows.</summary>
/// <typeparam name="T">The row type when built from a sequence of objects.</typeparam>
public class MarkdownTable<T>(IEnumerable<string> headers, IEnumerable<IEnumerable<string>> rows) : IMarkdownTable
{
    /// <summary>Creates a table from object properties as columns.</summary>
    /// <param name="items">The items whose public properties define columns and cells.</param>
    public MarkdownTable(IEnumerable<T> items) : this(items.ToMarkdownTablePrecursors())
    {
    }

    /// <summary>Creates a table from precomputed headers and rows.</summary>
    /// <param name="markdownTablePrecursors">Header and row data from <see cref="EnumerableExtensions.ToMarkdownTablePrecursors{T}"/>.</param>
    public MarkdownTable((IEnumerable<string>, IEnumerable<IEnumerable<string>>) markdownTablePrecursors) : this(markdownTablePrecursors.Item1, markdownTablePrecursors.Item2)
    {
    }

    /// <inheritdoc />
    public IEnumerable<string> Headers { get; } = headers;

    /// <inheritdoc />
    public IEnumerable<IEnumerable<string>> Rows { get; } = rows;

    /// <inheritdoc />
    public override string ToString() => this.ToMarkdownTableString();
}
