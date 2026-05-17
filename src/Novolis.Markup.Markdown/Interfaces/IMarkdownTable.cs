// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

public interface IMarkdownTable : IMarkdownSection
{
    IEnumerable<string> Headers { get; }
    
    IEnumerable<IEnumerable<string>> Rows { get; }
}