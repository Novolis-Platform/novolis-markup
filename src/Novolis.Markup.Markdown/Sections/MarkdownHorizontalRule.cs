// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

public class MarkdownHorizontalRule : IMarkdownHorizontalRule
{
    public override string ToString() => new string('-', 3);
}