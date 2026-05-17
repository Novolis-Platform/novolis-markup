// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

public interface IMarkdownCodeBlock : IMarkdownSection
{
    string Code { get; }
    
    string Language { get; }
}