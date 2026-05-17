// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

public interface IMarkdownSection
{
    static char NewLine => '\n';
    
    string ToString();
}