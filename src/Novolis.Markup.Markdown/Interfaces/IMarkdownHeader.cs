// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

public interface IMarkdownHeader : IMarkdownSection
{
    string Text { get; }
    
    int Level { get; }
}