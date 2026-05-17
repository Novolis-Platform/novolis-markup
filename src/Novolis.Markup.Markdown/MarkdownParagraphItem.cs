using System.Text;

// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

public class MarkdownParagraphItem(string text, MarkdownParagraphItemType type) : IMarkdownParagraphItem
{
    public string Text { get; } = text;

    public MarkdownParagraphItemType Type { get; } = type;

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(this.GetPrefix());
        sb.Append(Text);
        sb.Append(this.GetSuffix());
        return sb.ToString();
    }
}