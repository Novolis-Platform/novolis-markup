using System.Text;

// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

/// <summary>Represents MarkdownParagraphItem.</summary>
public class MarkdownParagraphItem(string text, MarkdownParagraphItemType type) : IMarkdownParagraphItem
{
    /// <summary>Text.</summary>
    public string Text { get; } = text;
/// <summary>ToString operation.</summary>

    public MarkdownParagraphItemType Type { get; } = type;
/// <summary>ToString operation.</summary>

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(this.GetPrefix());
        sb.Append(Text);
        sb.Append(this.GetSuffix());
        return sb.ToString();
    }
}
