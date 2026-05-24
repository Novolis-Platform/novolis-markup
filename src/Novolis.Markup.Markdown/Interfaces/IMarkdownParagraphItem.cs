// ReSharper disable CheckNamespace
namespace Novolis.Markup.Markdown;

/// <summary>Represents IMarkdownParagraphItem.</summary>
public interface IMarkdownParagraphItem
{
    /// <summary>Text.</summary>
    string Text { get; }
    /// <summary>Type.</summary>
    
    MarkdownParagraphItemType Type { get; }
}
