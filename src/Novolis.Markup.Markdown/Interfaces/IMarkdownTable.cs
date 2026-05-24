// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

/// <summary>Represents IMarkdownTable.</summary>
public interface IMarkdownTable : IMarkdownSection
{
    /// <summary>Headers.</summary>
    IEnumerable<string> Headers { get; }
    /// <summary>Rows.</summary>
    
    IEnumerable<IEnumerable<string>> Rows { get; }
}
