// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

public class MarkdownHeader(string text, int level = 1) : IMarkdownHeader
{
    public string Text { get; } = text;

    public int Level { get; } = level;

    public override string ToString() => $"{new string('#', Level)} {Text}{new string(IMarkdownHeader.NewLine, 1)}";
}